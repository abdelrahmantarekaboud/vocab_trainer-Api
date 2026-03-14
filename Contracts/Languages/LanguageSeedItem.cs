namespace VocabTrainer.Api.Contracts.Languages;

public class LanguageSeedItem
{
    public string NameEn { get; set; } = default!;
    public string Locale { get; set; } = default!;
    public List<string> Aliases { get; set; } = new();

}
