using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Quiz;

namespace VocabTrainer.Api.Services.Quiz
{
    public interface IQuizService
    {
        Task<Result<Guid>> CreateSession(Guid userId, CreateQuizSessionRequest req);
        Task<Result<NextQuizItemResponse>> Next(Guid userId, Guid sessionId);
        Task<Result<AnswerQuizResponse>> Answer(Guid userId, Guid sessionId, AnswerQuizRequest req);
        Task<Result<QuizSummaryResponse>> Summary(Guid userId, Guid sessionId);
        Task<Result<QuizSummaryResponse>> CloseSession(Guid userId, Guid sessionId);

    }
}