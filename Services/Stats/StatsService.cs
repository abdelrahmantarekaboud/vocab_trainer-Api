using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Stats;
using VocabTrainer.Api.Entities;
using VocabTrainer.Api.Errors;

namespace VocabTrainer.Api.Services.Stats
{
    public class StatsService : IStatsService
    {
        private readonly ApplicationDbContext _db;
        public StatsService(ApplicationDbContext db) => _db = db;

        public async Task<Result<StatsSummaryResponse>> Summary(Guid userId, string languageId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result<StatsSummaryResponse>.Failure(UserErrors.NotFound, "User not found");

            if (!user.IsGuest && !user.AllowedLanguages.Contains(languageId))
                return Result<StatsSummaryResponse>.Failure(UserErrors.ForbiddenLanguage, "Language not allowed");

            var stats = await _db.UserLanguageStats
                .FirstOrDefaultAsync(s => s.UserId == userId && s.LanguageId == languageId);

            if (stats == null)
            {
                stats = await BuildAndSaveStats(userId, languageId);
            }
            else
            {
                await RefreshWordsStats(stats, userId, languageId);
            }

            return Result<StatsSummaryResponse>.Success(new StatsSummaryResponse(
                stats.TotalWords,
                stats.DueWords,
                stats.TotalQuestions,
                stats.CorrectAnswers,
                stats.Accuracy,
                stats.LastQuizAtUtc,
                stats.TranslatedWords,
                stats.UntranslatedWords
            ));
        }

        private async Task<UserLanguageStats> BuildAndSaveStats(Guid userId, string languageId)
        {
            var words = await _db.Words
                .Where(w => w.OwnerId == userId && w.LanguageId == languageId)
                .ToListAsync();

            var due = words.Count(w => w.NextReviewAt != null && w.NextReviewAt <= DateTime.UtcNow);

            var translated = words.Count(w => !string.IsNullOrWhiteSpace(w.Translation));
            var untranslated = words.Count - translated;

            var results = await _db.QuizItemResults
                .Where(r => r.UserId == userId && r.LanguageId == languageId)
                .ToListAsync();

            var totalQ = results.Count;
            var correct = results.Count(r => r.IsCorrect);
            var acc = totalQ == 0 ? 0 : (double)correct / totalQ * 100;

            var stats = new UserLanguageStats
            {
                UserId = userId,
                LanguageId = languageId,

                TotalWords = words.Count,
                DueWords = due,

                TranslatedWords = translated,
                UntranslatedWords = untranslated,

                TotalQuestions = totalQ,
                CorrectAnswers = correct,
                Accuracy = acc,

                UpdatedAtUtc = DateTime.UtcNow
            };

            _db.UserLanguageStats.Add(stats);
            await _db.SaveChangesAsync();
            return stats;
        }

        private async Task RefreshWordsStats(UserLanguageStats stats, Guid userId, string languageId)
        {
            var words = await _db.Words
                .Where(w => w.OwnerId == userId && w.LanguageId == languageId)
                .ToListAsync();

            var translated = words.Count(w => !string.IsNullOrWhiteSpace(w.Translation));
            var untranslated = words.Count - translated;

            stats.TotalWords = words.Count;
            stats.DueWords = words.Count(w => w.NextReviewAt != null && w.NextReviewAt <= DateTime.UtcNow);

            stats.TranslatedWords = translated;
            stats.UntranslatedWords = untranslated;

            stats.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAfterQuiz(Guid userId, string languageId, int quizTotal, int quizCorrect)
        {
            var stats = await _db.UserLanguageStats
                .FirstOrDefaultAsync(s => s.UserId == userId && s.LanguageId == languageId);

            if (stats == null)
            {
                stats = new UserLanguageStats
                {
                    UserId = userId,
                    LanguageId = languageId
                };
                _db.UserLanguageStats.Add(stats);
            }

            var words = await _db.Words
                .Where(w => w.OwnerId == userId && w.LanguageId == languageId)
                .ToListAsync();

            var translated = words.Count(w => !string.IsNullOrWhiteSpace(w.Translation));
            var untranslated = words.Count - translated;

            stats.TotalWords = words.Count;
            stats.DueWords = words.Count(w => w.NextReviewAt != null && w.NextReviewAt <= DateTime.UtcNow);

            stats.TranslatedWords = translated;
            stats.UntranslatedWords = untranslated;

            stats.TotalQuestions += quizTotal;
            stats.CorrectAnswers += quizCorrect;
            stats.Accuracy = stats.TotalQuestions == 0
                ? 0
                : (double)stats.CorrectAnswers / stats.TotalQuestions * 100;

            stats.LastQuizAtUtc = DateTime.UtcNow;
            stats.LastQuizTotalQuestions = quizTotal;
            stats.LastQuizCorrectAnswers = quizCorrect;
            stats.LastQuizAccuracy = quizTotal == 0
                ? 0
                : (double)quizCorrect / quizTotal * 100;

            stats.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}
