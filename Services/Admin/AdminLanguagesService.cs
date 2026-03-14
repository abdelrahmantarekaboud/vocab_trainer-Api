using Microsoft.EntityFrameworkCore;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Admin;
using VocabTrainer.Api.Contracts.Languages;
using VocabTrainer.Api.Entities;
using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Mapping;

namespace VocabTrainer.Api.Services.Admin
{
    public class AdminLanguagesService : IAdminLanguagesService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITranslationProvider _translator;
        private readonly ILanguageCatalog _catalog;

        public AdminLanguagesService(
            ApplicationDbContext db,
            ITranslationProvider translator,
            ILanguageCatalog catalog)
        {
            _db = db;
            _translator = translator;
            _catalog = catalog;
        }

        public async Task<Result<List<LanguageDto>>> ListAll()
        {
            var all = await _db.Languages.ToListAsync();
            return Result<List<LanguageDto>>.Success(all.Select(x => x.ToDto()).ToList());
        }

        public async Task<Result> Toggle(string id, bool active)
        {
            var lang = await _db.Languages.FirstOrDefaultAsync(l => l.Id == id);
            if (lang == null)
                return Result.Failure(LanguageErrors.NotFound, "Language not found");

            lang.IsActive = active;
            await _db.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<LanguageDto>> AddLanguageByName(AddLanguageByNameRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NameEn))
                return Result<LanguageDto>.Failure(LanguageErrors.Invalid, "English name is required");

            var nameEn = request.NameEn.Trim();

            if (!_catalog.TryGetLocale(nameEn, out var locale))
            {
                var suggestions = _catalog.GetAllNames()
                    .Where(n => n.Contains(nameEn, StringComparison.OrdinalIgnoreCase))
                    .Take(5)
                    .ToList();

                var msg = suggestions.Any()
                    ? $"Unknown language name. Did you mean: {string.Join(", ", suggestions)} ?"
                    : "Unknown language name. Check spelling or update languages.json.";

                return Result<LanguageDto>.Failure(LanguageErrors.Invalid, msg);
            }

            var id = locale.ToLowerInvariant();

            var exists = await _db.Languages.AnyAsync(l => l.Id == id || l.Locale == locale);
            if (exists)
                return Result<LanguageDto>.Failure(LanguageErrors.Duplicate, "Language already exists");

            // تأكد إن Google Translate بيدعم الـ locale
            try
            {
                await _translator.TranslateAsync("test", "en", locale);
            }
            catch
            {
                return Result<LanguageDto>.Failure(LanguageErrors.Invalid, "Locale not supported by translator");
            }

            // ترجمة اسم اللغة للعربي
            string nameAr;
            try
            {
                nameAr = await _translator.TranslateAsync(nameEn, "en", "ar");
            }
            catch
            {
                nameAr = nameEn; // fallback
            }

            var lang = new Language
            {
                Id = id,
                Locale = locale,
                NameEn = nameEn,
                NameAr = nameAr,
                IsActive = request.IsActive // لو nullable خليه: request.IsActive ?? true
            };

            _db.Languages.Add(lang);
            await _db.SaveChangesAsync();

            return Result<LanguageDto>.Success(lang.ToDto());
        }
        public async Task<Result> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Result.Failure(LanguageErrors.Invalid, "Language id is required");

            id = id.Trim().ToLowerInvariant();

            var lang = await _db.Languages.FirstOrDefaultAsync(l => l.Id == id);
            if (lang == null)
                return Result.Failure(LanguageErrors.NotFound, "Language not found");

            // ✅ حماية: لو اللغة مستخدمة في كلمات
            var usedInWords = await _db.Words.AnyAsync(w => w.LanguageId == id);
            if (usedInWords)
                return Result.Failure(LanguageErrors.InUse, "Language is used in words and cannot be deleted");

            // ✅ حماية: لو مستخدمة في QuizSessions
            var usedInSessions = await _db.QuizSessions.AnyAsync(s => s.LanguageId == id);
            if (usedInSessions)
                return Result.Failure(LanguageErrors.InUse, "Language is used in quiz sessions and cannot be deleted");

            _db.Languages.Remove(lang);
            await _db.SaveChangesAsync();

            return Result.Success();
        }
    }
}
