namespace VocabTrainer.Api.Contracts.Words
{
    public record WordDto(
        Guid Id,
        string LanguageId,
        string Text,
        string? Translation,
        string? Example,
        string? Topic,
        string? RecordingUrl,
        int ReviewCount,
        int CorrectCount,
        int WrongCount,
        int IntervalDays,
        double EaseFactor,
        DateTime? NextReviewAt
    );
}