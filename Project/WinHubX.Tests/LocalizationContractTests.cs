using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace WinHubX.Tests;

public sealed class LocalizationContractTests
{
    private static readonly Dictionary<string, Dictionary<string, string>> Italian = LoadCatalog("it");
    private static readonly Dictionary<string, Dictionary<string, string>> English = LoadCatalog("en");

    [Fact]
    public void LanguageManager_UsesEmbeddedCatalogsAndFallsBackForUnsupportedLanguages()
    {
        LanguageManager.SetLanguage("en");
        Assert.Equal("en", LanguageManager.CurrentLanguage);
        Assert.Equal("File {0} could not be extracted correctly.",
            LanguageManager.GetTranslation("FormOffice", "fileextracterror"));

        LanguageManager.SetLanguage("it");
        Assert.Equal("it", LanguageManager.CurrentLanguage);
        Assert.Equal("Il file {0} non è stato estratto correttamente.",
            LanguageManager.GetTranslation("FormOffice", "fileextracterror"));

        LanguageManager.SetLanguage("en-US");
        Assert.Equal("it", LanguageManager.CurrentLanguage);
        Assert.Equal("unknown_key", LanguageManager.GetTranslation("UnknownForm", "unknown_key"));
    }

    [Fact]
    public void Catalogs_ContainTheSameSectionsAndKeys()
    {
        string[] missingSections = Italian.Keys.Except(English.Keys, StringComparer.Ordinal).ToArray();
        string[] extraSections = English.Keys.Except(Italian.Keys, StringComparer.Ordinal).ToArray();
        var missingKeys = new List<string>();
        var extraKeys = new List<string>();

        foreach (string section in Italian.Keys.Intersect(English.Keys, StringComparer.Ordinal))
        {
            missingKeys.AddRange(Italian[section].Keys
                .Except(English[section].Keys, StringComparer.Ordinal)
                .Select(key => $"{section}.{key}"));
            extraKeys.AddRange(English[section].Keys
                .Except(Italian[section].Keys, StringComparer.Ordinal)
                .Select(key => $"{section}.{key}"));
        }

        Assert.True(missingSections.Length == 0 && extraSections.Length == 0 &&
                    missingKeys.Count == 0 && extraKeys.Count == 0,
            $"Localization catalogs differ. Missing sections: {string.Join(", ", missingSections)}; " +
            $"extra sections: {string.Join(", ", extraSections)}; " +
            $"missing keys: {string.Join(", ", missingKeys)}; extra keys: {string.Join(", ", extraKeys)}.");
    }

    [Fact]
    public void Catalogs_DoNotContainDuplicateJsonProperties()
    {
        string[] duplicates = new[] { FindDuplicateProperties("it"), FindDuplicateProperties("en") }
            .Where(result => result.Length > 0)
            .ToArray();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void Catalogs_PreserveFormattingPlaceholdersAndNonEmptyTranslations()
    {
        var failures = new List<string>();

        foreach ((string section, Dictionary<string, string> italianEntries) in Italian)
        {
            foreach ((string key, string italianValue) in italianEntries)
            {
                string englishValue = English[section][key];
                if (!string.IsNullOrWhiteSpace(italianValue) && string.IsNullOrWhiteSpace(englishValue))
                    failures.Add($"{section}.{key} has no English translation");

                string[] italianPlaceholders = GetPlaceholders(italianValue);
                string[] englishPlaceholders = GetPlaceholders(englishValue);
                if (!italianPlaceholders.SequenceEqual(englishPlaceholders, StringComparer.Ordinal))
                    failures.Add($"{section}.{key} placeholders differ: IT [{string.Join(",", italianPlaceholders)}], EN [{string.Join(",", englishPlaceholders)}]");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Theory]
    [InlineData("FormHome", "cpu", 1)]
    [InlineData("FormHome", "ram", 1)]
    [InlineData("FormHome", "disco", 1)]
    [InlineData("FormHome", "os", 1)]
    [InlineData("FormHome", "windows", 1)]
    [InlineData("FormHome", "office", 1)]
    [InlineData("FormMonitoraggio", "upload", 1)]
    [InlineData("FormMonitoraggio", "download", 1)]
    [InlineData("FormMonitoraggio", "velocita", 1)]
    [InlineData("FormCreazioneISO", "successo2", 1)]
    [InlineData("FormImpostazioni", "aggiornamento_disponibile_msg", 2)]
    [InlineData("FormImpostazioni", "labelversione", 1)]
    [InlineData("FormOfficeAggiungiRimuovi", "versione_office", 1)]
    [InlineData("FormOfficeAggiungiRimuovi", "piattaforma", 1)]
    [InlineData("FormOfficeAggiungiRimuovi", "prodotto", 1)]
    [InlineData("FormOfficeAggiungiRimuovi", "lingua", 1)]
    [InlineData("FormOfficeAggiungiRimuovi", "versione_non_compatibile", 1)]
    [InlineData("FormOfficeAggiungiRimuovi", "versione_richiesta", 1)]
    [InlineData("FormRipristinoSO", "scansioneInCorsoConTempo", 1)]
    [InlineData("FormRipristinoSO", "tempoRimanente", 1)]
    [InlineData("FormSettaggi", "exportsuccess", 1)]
    [InlineData("FormSettaggi", "exporterrorcode", 1)]
    [InlineData("FormSettaggi", "exportexception", 1)]
    public void FormatTranslation_UsesAllArgumentsInBothCatalogs(string section, string key, int argumentCount)
    {
        foreach (string language in new[] { "it", "en" })
        {
            LanguageManager.SetLanguage(language);
            object?[] arguments = Enumerable.Range(0, argumentCount).Select(index => (object?)$"value-{index}").ToArray();

            string formatted = LanguageManager.FormatTranslation(section, key, arguments);

            Assert.DoesNotContain("{0}", formatted, StringComparison.Ordinal);
            if (argumentCount > 1)
                Assert.DoesNotContain("{1}", formatted, StringComparison.Ordinal);
        }

        LanguageManager.SetLanguage("it");
    }

    private static Dictionary<string, Dictionary<string, string>> LoadCatalog(string language)
    {
        string resourceName = $"WinHubX.Resources.{language}.json";
        using Stream stream = typeof(LocalizationContractTests).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded localization resource '{resourceName}' was not found.");

        return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(stream)
            ?? throw new InvalidDataException($"Localization resource '{resourceName}' is empty or invalid.");
    }

    private static string FindDuplicateProperties(string language)
    {
        string resourceName = $"WinHubX.Resources.{language}.json";
        using Stream stream = typeof(LocalizationContractTests).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded localization resource '{resourceName}' was not found.");
        using JsonDocument document = JsonDocument.Parse(stream);
        var duplicates = new List<string>();

        void Visit(JsonElement element, string path)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                var names = new HashSet<string>(StringComparer.Ordinal);
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    if (!names.Add(property.Name))
                        duplicates.Add($"{path}.{property.Name}");

                    Visit(property.Value, $"{path}.{property.Name}");
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                int index = 0;
                foreach (JsonElement item in element.EnumerateArray())
                    Visit(item, $"{path}[{index++}]");
            }
        }

        Visit(document.RootElement, language);
        return string.Join(", ", duplicates);
    }

    private static string[] GetPlaceholders(string value) => Regex.Matches(
            value,
            @"\{\d+\}",
            RegexOptions.None,
            TimeSpan.FromSeconds(1))
        .Select(match => match.Value)
        .OrderBy(placeholder => placeholder, StringComparer.Ordinal)
        .ToArray();
}
