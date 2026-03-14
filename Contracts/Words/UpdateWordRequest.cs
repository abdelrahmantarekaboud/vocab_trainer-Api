namespace VocabTrainer.Api.Contracts.Words
{
    public record UpdateWordRequest(
        string? Text,
        string? Translation,
        string? Example,
        string? Topic
    );
}