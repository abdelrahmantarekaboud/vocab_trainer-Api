namespace VocabTrainer.Api.Contracts.Languages
{
    public record LanguageDto(string Id, string NameEn, string NameAr, string Locale, bool IsActive);
}