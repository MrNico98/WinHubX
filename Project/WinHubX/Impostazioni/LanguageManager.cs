using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

public static class LanguageManager
{
    private sealed record LanguageSnapshot(
        string Language,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Catalog);

    private static LanguageSnapshot currentSnapshot = new("it", LoadEmbeddedCatalog("it"));

    public static string CurrentLanguage => Volatile.Read(ref currentSnapshot).Language;

    public static void LoadLanguageFromSettings()
    {
        WinHubX.ThemeConfig settings = WinHubX.ThemeConfig.Load();
        string language = NormalizeLanguage(settings.Language);
        ApplyCulture(language);
        SetLanguage(language);
    }

    public static void ApplyCulture(string language)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(NormalizeLanguage(language));
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
    }

    public static void LoadTranslations() => SetLanguage(CurrentLanguage);

    public static void SetLanguage(string language)
    {
        string supportedLanguage = NormalizeLanguage(language);
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> loadedCatalog;

        try
        {
            loadedCatalog = LoadEmbeddedCatalog(supportedLanguage);
        }
        catch (Exception ex) when (ex is IOException
            or InvalidDataException
            or Newtonsoft.Json.JsonException
            or ArgumentException)
        {
            Debug.WriteLine($"Unable to load the {supportedLanguage} localization resource: {ex}");
            supportedLanguage = "it";

            try
            {
                loadedCatalog = LoadEmbeddedCatalog(supportedLanguage);
            }
            catch (Exception fallbackException) when (fallbackException is IOException
                or InvalidDataException
                or Newtonsoft.Json.JsonException
                or ArgumentException)
            {
                Debug.WriteLine($"Unable to load the fallback localization resource: {fallbackException}");
                loadedCatalog = new ReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>(
                    new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal));
            }
        }

        Volatile.Write(ref currentSnapshot, new LanguageSnapshot(supportedLanguage, loadedCatalog));
    }

    public static string GetTranslation(string formName, string key)
    {
        LanguageSnapshot snapshot = Volatile.Read(ref currentSnapshot);

        if (snapshot.Catalog.TryGetValue(formName, out IReadOnlyDictionary<string, string>? section) &&
            section.TryGetValue(key, out string? value))
        {
            return value;
        }

        return key;
    }

    public static string FormatTranslation(string formName, string key, params object?[] arguments)
    {
        return Format(GetTranslation(formName, key), arguments);
    }

    public static string Format(string format, params object?[] arguments)
    {
        ArgumentNullException.ThrowIfNull(format);
        ArgumentNullException.ThrowIfNull(arguments);

        CompositeFormat compositeFormat = CompositeFormat.Parse(format);
        if (compositeFormat.MinimumArgumentCount != arguments.Length)
        {
            throw new FormatException(
                $"Format string expects {compositeFormat.MinimumArgumentCount} arguments, but received {arguments.Length}.");
        }

        return string.Format(CultureInfo.CurrentCulture, compositeFormat, arguments);
    }

    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadEmbeddedCatalog(string language)
    {
        string resourceName = $"WinHubX.Resources.{language}.json";
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidDataException($"Embedded localization resource '{resourceName}' was not found.");
        using StreamReader reader = new(stream);

        Dictionary<string, Dictionary<string, string>> catalog =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(reader.ReadToEnd())
            ?? throw new InvalidDataException($"Embedded localization resource '{resourceName}' is empty or invalid.");

        var immutableSections = catalog.ToDictionary(
            entry => entry.Key,
            entry => (IReadOnlyDictionary<string, string>)new ReadOnlyDictionary<string, string>(entry.Value),
            StringComparer.Ordinal);

        return new ReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>(immutableSections);
    }

    private static string NormalizeLanguage(string? language) =>
        string.Equals(language, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "it";
}
