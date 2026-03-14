namespace VocabTrainer.Api.Contracts.Stats
{
    public record StatsSummaryResponse(
    int TotalWords,
    int DueWords,
    int TotalQuestions,
    int CorrectAnswers,
    double Accuracy,
    DateTime? LastQuizAtUtc,
    int TranslatedWords,
    int UntranslatedWords

    );
}