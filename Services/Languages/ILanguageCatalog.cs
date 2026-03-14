namespace VocabTrainer.Api.Services.Languages;

public interface ILanguageCatalog
{
    bool TryGetLocale(string nameEn, out string locale);
    IReadOnlyList<string> GetAllNames();

}