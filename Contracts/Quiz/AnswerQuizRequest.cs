namespace VocabTrainer.Api.Contracts.Quiz
{
    public record AnswerQuizRequest(
        Guid WordId,
        string SelectedAnswer,
        int ResponseTimeMs
    );
}
