namespace VocabTrainer.Api.Contracts.Words
{
    public record CreateWordRequest(
        string LanguageId,
        string Text,
        string? Translation,
        string? Example,
        string? Topic
    );
}