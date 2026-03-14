namespace VocabTrainer.Api.Contracts.Words
{
    public record WordsQuery(string? LanguageId, string? Search, string? Topic);
}