using Microsoft.EntityFrameworkCore;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Abstractions.Storage;
using VocabTrainer.Api.Contracts.Words;
using VocabTrainer.Api.Entities;
using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Mapping;

namespace VocabTrainer.Api.Services.Words
{
    public class WordsService : IWordsService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITranslationProvider _translator;
        private readonly IFileStorage _storage;

        public WordsService(ApplicationDbContext db, ITranslationProvider translator, IFileStorage storage)
        {
            _db = db;
            _translator = translator;
            _storage = storage;
        }

        // ✅ Normalize Topic: يخزنها Uppercase ويشيل المسافات
        private static string? NormalizeTopic(string? topic)
        {
            if (string.IsNullOrWhiteSpace(topic)) return null;
            return topic.Trim().ToUpperInvariant();
        }

        public async Task<Result<WordDto>> Create(Guid userId, CreateWordRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result<WordDto>.Failure(UserErrors.NotFound, "User not found");

            var lang = await _db.Languages.FirstOrDefaultAsync(l => l.Id == req.LanguageId);
            if (lang == null)
                return Result<WordDto>.Failure(LanguageErrors.NotFound, "Language not found");

            if (!lang.IsActive)
                return Result<WordDto>.Failure(LanguageErrors.Disabled, "Language disabled");

            if (!user.IsGuest && !user.AllowedLanguages.Contains(req.LanguageId))
                return Result<WordDto>.Failure(UserErrors.ForbiddenLanguage, "Language not allowed");

            var text = req.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return Result<WordDto>.Failure(WordErrors.Duplicate, "Text is required");
            // لو عندك WordErrors.InvalidText اعملها بدل دي

            var exists = await _db.Words.AnyAsync(w =>
                w.OwnerId == userId &&
                w.LanguageId == req.LanguageId &&
                w.Text.ToLower() == text.ToLower());

            if (exists)
                return Result<WordDto>.Failure(WordErrors.Duplicate, "Duplicate word");

            // ✅ Auto Translate لو الترجمة فاضية
            string? translation = req.Translation?.Trim();
            if (string.IsNullOrWhiteSpace(translation))
            {
                try
                {
                    var target = string.IsNullOrWhiteSpace(user.UiLanguage) ? "ar" : user.UiLanguage.Trim().ToLower();

                    // هنا req.LanguageId هو كود اللغة (Id = locale)
                    if (_translator.IsSupported(req.LanguageId) && _translator.IsSupported(target))
                    {
                        translation = await _translator.TranslateAsync(
                            text,
                            from: req.LanguageId,
                            to: target
                        );
                    }
                }
                catch
                {
                    translation = null; // فشل الترجمة ما يوقعش العملية
                }
            }

            var topic = NormalizeTopic(req.Topic);

            var word = new Word
            {
                OwnerId = userId,
                LanguageId = req.LanguageId,
                Text = text,
                Translation = translation,
                Example = req.Example?.Trim(),
                Topic = topic,                      // ✅ normalized
                NextReviewAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _db.Words.Add(word);
            await _db.SaveChangesAsync();

            return Result<WordDto>.Success(word.ToDto());
        }

        public async Task<Result<List<WordDto>>> List(Guid userId, WordsQuery q)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result<List<WordDto>>.Failure(UserErrors.NotFound, "User not found");

            IQueryable<Word> query = _db.Words.Where(w => w.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(q.LanguageId))
            {
                var langId = q.LanguageId.Trim();

                var lang = await _db.Languages.FirstOrDefaultAsync(l => l.Id == langId);
                if (lang == null)
                    return Result<List<WordDto>>.Failure(LanguageErrors.NotFound, "Language not found");

                if (!lang.IsActive)
                    return Result<List<WordDto>>.Failure(LanguageErrors.Disabled, "Language disabled");

                if (!user.IsGuest && !user.AllowedLanguages.Contains(langId))
                    return Result<List<WordDto>>.Failure(UserErrors.ForbiddenLanguage, "Language not allowed");

                query = query.Where(w => w.LanguageId == langId);
            }

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                query = query.Where(w =>
                    w.Text.Contains(s) ||
                    (w.Translation ?? "").Contains(s));
            }

            var qTopic = NormalizeTopic(q.Topic);
            if (qTopic != null)
                query = query.Where(w => w.Topic == qTopic);

            var list = await query
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return Result<List<WordDto>>.Success(list.Select(x => x.ToDto()).ToList());
        }



        public async Task<Result<WordDto>> Update(Guid userId, Guid wordId, UpdateWordRequest req)
        {
            var word = await _db.Words.FirstOrDefaultAsync(w => w.Id == wordId && w.OwnerId == userId);
            if (word == null)
                return Result<WordDto>.Failure(WordErrors.NotFound, "Word not found");

            if (req.Text != null) word.Text = req.Text.Trim();
            if (req.Translation != null) word.Translation = req.Translation.Trim();
            if (req.Example != null) word.Example = req.Example.Trim();

            // ✅ normalize topic on update
            if (req.Topic != null) word.Topic = NormalizeTopic(req.Topic);

            word.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Result<WordDto>.Success(word.ToDto());
        }

        public async Task<Result> Delete(Guid userId, Guid wordId)
        {
            var word = await _db.Words.FirstOrDefaultAsync(w => w.Id == wordId && w.OwnerId == userId);
            if (word == null)
                return Result.Failure(WordErrors.NotFound, "Word not found");

            _db.Words.Remove(word);
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        // ✅ ميثود تمسح كل كلمات اليوزر
        public async Task<Result<int>> DeleteAllForUser(Guid userId)
        {
            // لو عايز تتحقق إن اليوزر موجود:
            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return Result<int>.Failure(UserErrors.NotFound, "User not found");

            var words = await _db.Words.Where(w => w.OwnerId == userId).ToListAsync();
            if (words.Count == 0)
                return Result<int>.Success(0);

            _db.Words.RemoveRange(words);
            var deleted = await _db.SaveChangesAsync();

            return Result<int>.Success(deleted);
        }

        public async Task<Result<WordDto>> SaveRecording(Guid userId, SaveRecordingRequest req)
        {
            var word = await _db.Words
                .FirstOrDefaultAsync(w => w.Id == req.WordId && w.OwnerId == userId);

            if (word == null)
                return Result<WordDto>.Failure(WordErrors.NotFound, "Word not found");

            if (string.IsNullOrWhiteSpace(word.Text))
                return Result<WordDto>.Failure(WordErrors.Duplicate, "Word text is required before recording");

            if (string.IsNullOrWhiteSpace(req.Base64Audio))
                return Result<WordDto>.Failure(WordErrors.Duplicate, "Audio is required");

            var ext = req.FileExt?.Trim('.').ToLower();
            if (ext is not ("webm" or "wav" or "mp3"))
                ext = "webm";

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(req.Base64Audio);
            }
            catch
            {
                return Result<WordDto>.Failure(WordErrors.Duplicate, "Invalid base64 audio");
            }

            var relDir = Path.Combine("recordings", userId.ToString());
            var absDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relDir);
            Directory.CreateDirectory(absDir);

            var fileName = $"{word.Id}.{ext}";
            var absPath = Path.Combine(absDir, fileName);

            await File.WriteAllBytesAsync(absPath, bytes);

            word.RecordingUrl = "/" + Path.Combine(relDir, fileName).Replace("\\", "/");
            word.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Result<WordDto>.Success(word.ToDto());
        }

        public async Task<Result<(byte[] bytes, string contentType)>> GetRecording(Guid userId, Guid wordId)
        {
            var word = await _db.Words
                .FirstOrDefaultAsync(w => w.Id == wordId && w.OwnerId == userId);

            if (word == null || string.IsNullOrWhiteSpace(word.RecordingUrl))
                return Result<(byte[], string)>.Failure(WordErrors.NotFound, "Recording not found");

            var relPath = word.RecordingUrl.TrimStart('/');
            var absPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relPath);

            if (!File.Exists(absPath))
                return Result<(byte[], string)>.Failure(WordErrors.NotFound, "File not found");

            var bytes = await File.ReadAllBytesAsync(absPath);

            var ext = Path.GetExtension(absPath).ToLower();
            var contentType = ext switch
            {
                ".webm" => "audio/webm",
                ".wav" => "audio/wav",
                ".mp3" => "audio/mpeg",
                _ => "application/octet-stream"
            };

            return Result<(byte[], string)>.Success((bytes, contentType));
        }
        public async Task<Result<WordDto>> DeleteRecording(Guid userId, Guid wordId)
        {
            var word = await _db.Words.FirstOrDefaultAsync(w => w.Id == wordId && w.OwnerId == userId);
            if (word == null)
                return Result<WordDto>.Failure(WordErrors.NotFound, "Word not found");

            if (string.IsNullOrWhiteSpace(word.RecordingUrl))
                return Result<WordDto>.Failure(WordErrors.NotFound, "Recording not found");

            // امسح الملف من wwwroot
            try
            {
                var relPath = word.RecordingUrl.TrimStart('/');
                var absPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relPath);
                if (File.Exists(absPath))
                    File.Delete(absPath);
            }
            catch
            {
                // لو فشل مسح الملف، مش لازم يوقع العملية
            }

            word.RecordingUrl = null;
            word.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Result<WordDto>.Success(word.ToDto());
        }



    }
}
