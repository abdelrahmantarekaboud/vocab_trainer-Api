using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Abstractions.Enums;
using VocabTrainer.Api.Contracts.Quiz;
using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Helpers;
using VocabTrainer.Api.Mapping;

namespace VocabTrainer.Api.Services.Quiz
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _db;
        private readonly IStatsService _statsService;
        private const int PointsPerCorrect = 10;

        public QuizService(ApplicationDbContext db, IStatsService statsService)
        {
            _db = db;
            _statsService = statsService;
        }

        private static string? NormalizeTopic(string? topic)
        {
            if (string.IsNullOrWhiteSpace(topic)) return null;
            return topic.Trim().ToUpperInvariant();
        }

        public async Task<Result<Guid>> CreateSession(Guid userId, CreateQuizSessionRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result<Guid>.Failure(UserErrors.NotFound, "User not found");

            if (!user.IsGuest && !user.AllowedLanguages.Contains(req.LanguageId))
                return Result<Guid>.Failure(UserErrors.ForbiddenLanguage, "Language not allowed");

            var normalizedTopic = NormalizeTopic(req.TopicFilter);

            var baseQ = _db.Words.Where(w => w.OwnerId == userId && w.LanguageId == req.LanguageId);

            if (normalizedTopic != null)
                baseQ = baseQ.Where(w => w.Topic == normalizedTopic);

            if (req.Mode == QuizMode.Due)
                baseQ = baseQ.Where(w => w.NextReviewAt != null && w.NextReviewAt <= DateTime.UtcNow);
            else if (req.Mode == QuizMode.New)
                baseQ = baseQ.Where(w => w.ReviewCount == 0);

            var totalItems = await baseQ.CountAsync();

            var s = new Entities.QuizSession
            {
                OwnerId = userId,
                LanguageId = req.LanguageId,
                Mode = req.Mode,
                TopicFilter = normalizedTopic,
                TotalItems = totalItems,
                CorrectCount = 0,
                WrongCount = 0,
                Score = 0
            };

            _db.QuizSessions.Add(s);
            await _db.SaveChangesAsync();

            return Result<Guid>.Success(s.Id);
        }

        // ✅ Helper موحّد لإنهاء السيشن + تحديث Stats + بناء Summary
        private async Task<QuizSummaryResponse> EndSessionAndBuildSummary(Guid userId, Entities.QuizSession session)
        {
            if (session.EndedAt == null)
            {
                session.EndedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            var totalQ = await _db.QuizItemResults
                .CountAsync(r => r.SessionId == session.Id && r.Attempt == 1);

            var correctQ = await _db.QuizItemResults
                .CountAsync(r => r.SessionId == session.Id && r.Attempt == 1 && r.IsCorrect);

            await _statsService.UpdateAfterQuiz(
                userId,
                session.LanguageId,
                totalQ,
                correctQ
            );

            return await BuildSummary(userId, session.Id, session.TotalItems);
        }

        public async Task<Result<NextQuizItemResponse>> Next(Guid userId, Guid sessionId)
        {
            var session = await _db.QuizSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.OwnerId == userId);

            if (session == null)
                return Result<NextQuizItemResponse>.Failure(QuizErrors.SessionNotFound, "Session not found");

            // لو السيشن اتقفلت قبل كده
            if (session.EndedAt != null)
            {
                var summary = await BuildSummary(userId, sessionId, session.TotalItems);

                return Result<NextQuizItemResponse>.Success(
                    new NextQuizItemResponse(
                        null,
                        new(),
                        null,
                        0,
                        session.TotalItems,      // Total
                        session.TotalItems,      // Answered
                        summary.CorrectCount,
                        summary.WrongCount,
                        summary.Score,
                        true
                    )
                );
            }

            // ✅ لو مفيش أسئلة من البداية، اقفل رسميًا وارجّع Finished
            if (session.TotalItems == 0)
            {
                var summary = await EndSessionAndBuildSummary(userId, session);

                return Result<NextQuizItemResponse>.Success(
                    new NextQuizItemResponse(
                        null,
                        new(),
                        null,
                        0,
                        session.TotalItems,  // 0
                        session.TotalItems,  // 0
                        summary.CorrectCount,
                        summary.WrongCount,
                        summary.Score,
                        true
                    )
                );
            }

            var baseQ = _db.Words.Where(w => w.OwnerId == userId && w.LanguageId == session.LanguageId);

            if (!string.IsNullOrWhiteSpace(session.TopicFilter))
                baseQ = baseQ.Where(w => w.Topic == session.TopicFilter);

            if (session.Mode == QuizMode.Due)
                baseQ = baseQ.Where(w => w.NextReviewAt != null && w.NextReviewAt <= DateTime.UtcNow);
            else if (session.Mode == QuizMode.New)
                baseQ = baseQ.Where(w => w.ReviewCount == 0);

            var allIds = await baseQ.Select(w => w.Id).ToListAsync();

            var askedFirstRoundIds = await _db.QuizItemResults
                .Where(r => r.SessionId == sessionId && r.Attempt == 1)
                .Select(r => r.WordId)
                .ToListAsync();

            var remainingFirstRound = allIds.Except(askedFirstRoundIds).ToList();

            Guid pickId;
            int attempt;

            if (remainingFirstRound.Any())
            {
                pickId = remainingFirstRound.OrderBy(_ => Guid.NewGuid()).First();
                attempt = 1;
            }
            else
            {
                var lastAttempts = await _db.QuizItemResults
                    .Where(r => r.SessionId == sessionId)
                    .GroupBy(r => r.WordId)
                    .Select(g => new
                    {
                        WordId = g.Key,
                        LastAttempt = g.OrderByDescending(x => x.Attempt).FirstOrDefault()
                    })
                    .ToListAsync();

                var pendingWrongIds = lastAttempts
                    .Where(x => x.LastAttempt != null && !x.LastAttempt.IsCorrect)
                    .Select(x => x.WordId)
                    .ToList();

                // ✅ مفيش غلطات = النهاية الرسمية
                if (!pendingWrongIds.Any())
                {
                    var summary = await EndSessionAndBuildSummary(userId, session);

                    return Result<NextQuizItemResponse>.Success(
                        new NextQuizItemResponse(
                            null,
                            new(),
                            null,
                            0,
                            session.TotalItems,
                            session.TotalItems,   // أول راند كله اتسأل
                            summary.CorrectCount,
                            summary.WrongCount,
                            summary.Score,
                            true
                        )
                    );
                }

                pickId = pendingWrongIds.OrderBy(_ => Guid.NewGuid()).First();

                var lastAttemptForPick = lastAttempts.First(x => x.WordId == pickId)
                    .LastAttempt!.Attempt;

                attempt = lastAttemptForPick + 1;
                if (attempt > 5) attempt = 5;
            }

            var pick = await _db.Words.FirstOrDefaultAsync(w => w.Id == pickId);
            if (pick == null)
                return Result<NextQuizItemResponse>.Failure(WordErrors.NotFound, "Word not found");

            var dto = pick.ToDto();
            var correctAnswer = dto.Translation ?? dto.Text;

            var distractors = await _db.Words
                .Where(w => w.OwnerId == userId
                            && w.LanguageId == session.LanguageId
                            && w.Id != pick.Id)
                .OrderBy(_ => Guid.NewGuid())
                .Take(3)
                .Select(w => w.Translation ?? w.Text)
                .ToListAsync();

            var choices = distractors.Append(correctAnswer)
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            var answeredFirstRound = askedFirstRoundIds.Count;

            return Result<NextQuizItemResponse>.Success(
                new NextQuizItemResponse(
                    dto,
                    choices,
                    correctAnswer,
                    attempt,
                    session.TotalItems,
                    answeredFirstRound,
                    session.CorrectCount,
                    session.WrongCount,
                    session.Score,
                    false
                )
            );
        }

        public async Task<Result<AnswerQuizResponse>> Answer(Guid userId, Guid sessionId, AnswerQuizRequest req)
        {
            var session = await _db.QuizSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.OwnerId == userId);

            if (session == null)
                return Result<AnswerQuizResponse>.Failure(QuizErrors.SessionNotFound, "Session not found");

            if (session.EndedAt != null)
                return Result<AnswerQuizResponse>.Failure(QuizErrors.SessionNotFound, "Session already ended");

            var word = await _db.Words
                .FirstOrDefaultAsync(w => w.Id == req.WordId && w.OwnerId == userId);

            if (word == null)
                return Result<AnswerQuizResponse>.Failure(WordErrors.NotFound, "Word not found");

            var correctAnswer = word.Translation ?? word.Text;
            var selected = (req.SelectedAnswer ?? "").Trim();

            var isCorrect = string.Equals(
                selected,
                correctAnswer,
                StringComparison.OrdinalIgnoreCase
            );

            var lastAttempt = await _db.QuizItemResults
                .Where(r => r.SessionId == sessionId && r.WordId == word.Id)
                .MaxAsync(r => (int?)r.Attempt) ?? 0;

            var attempt = lastAttempt + 1;
            if (attempt > 5) attempt = 5;

            if (attempt == 1)
            {
                SpacedRepetitionHelper.ApplyAnswer(word, isCorrect);

                // ✅ تحديث آخر نشاط للكلمة
                word.UpdatedAt = DateTime.UtcNow;
                // word.LastReviewedAt = DateTime.UtcNow;
            }

            _db.QuizItemResults.Add(new Entities.QuizItemResult
            {
                SessionId = sessionId,
                WordId = word.Id,

                UserId = userId,
                LanguageId = session.LanguageId,

                IsCorrect = isCorrect,
                ResponseTimeMs = req.ResponseTimeMs,
                Attempt = attempt,
                SelectedAnswer = selected
            });

            if (attempt == 1)
            {
                if (isCorrect) session.CorrectCount++;
                else session.WrongCount++;

                session.Score = session.CorrectCount * PointsPerCorrect;
            }

            await _db.SaveChangesAsync();

            var askedFirstRound = await _db.QuizItemResults
                .CountAsync(r => r.SessionId == sessionId && r.Attempt == 1);

            bool firstRoundDone = askedFirstRound >= session.TotalItems;

            if (firstRoundDone)
            {
                var lastAttempts = await _db.QuizItemResults
                    .Where(r => r.SessionId == sessionId)
                    .GroupBy(r => r.WordId)
                    .Select(g => g.OrderByDescending(x => x.Attempt).First())
                    .ToListAsync();

                bool stillWrongExists = lastAttempts.Any(x => !x.IsCorrect);

                if (!stillWrongExists)
                {
                    var summary = await EndSessionAndBuildSummary(userId, session);

                    return Result<AnswerQuizResponse>.Success(
                        new AnswerQuizResponse(
                            isCorrect,
                            correctAnswer,
                            summary.CorrectCount,
                            summary.WrongCount,
                            summary.Score,
                            true,
                            summary
                        )
                    );
                }
            }

            return Result<AnswerQuizResponse>.Success(
                new AnswerQuizResponse(
                    isCorrect,
                    correctAnswer,
                    session.CorrectCount,
                    session.WrongCount,
                    session.Score,
                    false,
                    null
                )
            );
        }

        private async Task<QuizSummaryResponse> BuildSummary(Guid userId, Guid sessionId, int total)
        {
            var correctCount = await _db.QuizItemResults
                .CountAsync(r => r.SessionId == sessionId && r.Attempt == 1 && r.IsCorrect);

            var wrongCount = await _db.QuizItemResults
                .CountAsync(r => r.SessionId == sessionId && r.Attempt == 1 && !r.IsCorrect);

            var score = correctCount * PointsPerCorrect;

            var wrongIds = await _db.QuizItemResults
                .Where(r => r.SessionId == sessionId && r.Attempt == 1 && !r.IsCorrect)
                .Select(r => r.WordId)
                .Distinct()
                .ToListAsync();

            var wrongItems = await _db.Words
                .Where(w => w.OwnerId == userId && wrongIds.Contains(w.Id))
                .ToListAsync();

            return new QuizSummaryResponse(
                total,
                correctCount,
                wrongCount,
                score,
                wrongItems.Select(x => x.ToDto()).ToList()
            );
        }

        public async Task<Result<QuizSummaryResponse>> Summary(Guid userId, Guid sessionId)
        {
            var session = await _db.QuizSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.OwnerId == userId);

            if (session == null)
                return Result<QuizSummaryResponse>.Failure(QuizErrors.SessionNotFound, "Session not found");

            var summary = await BuildSummary(userId, sessionId, session.TotalItems);
            return Result<QuizSummaryResponse>.Success(summary);
        }

        public async Task<Result<QuizSummaryResponse>> CloseSession(Guid userId, Guid sessionId)
        {
            var session = await _db.QuizSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.OwnerId == userId);

            if (session == null)
                return Result<QuizSummaryResponse>.Failure(QuizErrors.SessionNotFound, "Session not found");

            var summary = await EndSessionAndBuildSummary(userId, session);
            return Result<QuizSummaryResponse>.Success(summary);
        }
    }
}
