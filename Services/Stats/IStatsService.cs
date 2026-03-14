using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Stats;

namespace VocabTrainer.Api.Services.Stats
{
    public interface IStatsService
    {
        Task<Result<StatsSummaryResponse>> Summary(Guid userId, string languageId);
        Task UpdateAfterQuiz(Guid userId, string languageId, int quizTotal, int quizCorrect);

    }
}