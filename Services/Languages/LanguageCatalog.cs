using System.Globalization;
using System.Text;
using System.Text.Json;
using VocabTrainer.Api.Contracts.Languages;

namespace VocabTrainer.Api.Services.Languages;

public class LanguageCatalog : ILanguageCatalog
{
    private readonly Dictionary<string, string> _map;
    private readonly HashSet<string> _allNames;

    public LanguageCatalog(IWebHostEnvironment env)
    {
        var path = Path.Combine(env.ContentRootPath, "languages.json");
        var json = File.ReadAllText(path);

        var items = JsonSerializer.Deserialize<List<LanguageSeedItem>>(
     json,
             new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
         ) ?? new List<LanguageSeedItem>();


        _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var it in items)
        {
            if (string.IsNullOrWhiteSpace(it.NameEn) || string.IsNullOrWhiteSpace(it.Locale))
                continue;

            var locale = it.Locale.Trim();

            var mainKey = NormalizeName(it.NameEn);
            _map[mainKey] = locale;
            _allNames.Add(it.NameEn.Trim());

            if (it.Aliases != null)
            {
                foreach (var a in it.Aliases)
                {
                    if (string.IsNullOrWhiteSpace(a)) continue;
                    var aliasKey = NormalizeName(a);
                    _map[aliasKey] = locale;
                    _allNames.Add(a.Trim());
                }
            }
        }
    }

    public bool TryGetLocale(string nameEn, out string locale)
    {
        locale = "";
        if (string.IsNullOrWhiteSpace(nameEn)) return false;

        return _map.TryGetValue(NormalizeName(nameEn), out locale);
    }

    public IReadOnlyList<string> GetAllNames()
        => _allNames.OrderBy(x => x).ToList();

    private static string NormalizeName(string input)
    {
        input = input.Trim().ToLowerInvariant();

        // multiple spaces -> one
        input = string.Join(' ', input.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        // remove diacritics (accents)
        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
