namespace VocabTrainer.Api.Contracts.Stats
{
    public record StatsByLanguageResponse(string LanguageId, int TotalWords, int DueWords);
}