using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
public static class LanguageManager
{
    private static Dictionary<string, Dictionary<string, string>>? translations;
    public static string CurrentLanguage { get; private set; } = "it";

    private static readonly string LocalAppDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WinHubX", "Lingue");

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WinHubX", "Impostazioni", "Tema.json");

    // Ensure directories exist on static initialization
    static LanguageManager()
    {
        EnsureDirectoriesExist();
    }

    private static void EnsureDirectoriesExist()
    {
        try
        {
            Directory.CreateDirectory(LocalAppDataPath);
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        }
        catch (Exception ex)
        {
        }
    }

    public static void LoadLanguageFromSettings()
    {
        try
        {
            EnsureDirectoriesExist(); // Double-check directories exist

            if (!File.Exists(SettingsPath))
            {
                // Create default settings if they don't exist
                CreateDefaultSettings();
                return;
            }

            string json = File.ReadAllText(SettingsPath);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            if (settings == null || !settings.ContainsKey("Language"))
            {
                return;
            }

            string lang = settings["Language"].ToString() ?? "it";
            ApplyCulture(lang);
            SetLanguage(lang);
        }
        catch (Exception ex)
        {

        }
    }

    private static void CreateDefaultSettings()
    {
        try
        {
            var defaultSettings = new Dictionary<string, object>
            {
                ["Language"] = "it",
                ["DarkTheme"] = false,
                ["LanguageManuallySet"] = false,
                ["ThemeManuallySet"] = false
            };

            string json = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
        }
    }

    public static void ApplyCulture(string lang)
    {
        try
        {
            var culture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }
        catch (Exception ex)
        {
        }
    }

    public static void LoadTranslations()
    {
        try
        {
            EnsureDirectoriesExist(); // Ensure directory exists

            string filePath = Path.Combine(LocalAppDataPath, $"{CurrentLanguage}.json");

            if (!File.Exists(filePath))
            {
                ExtractEmbeddedResource($"WinHubX.Resources.{CurrentLanguage}.json", filePath);
            }

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                translations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            }
            else
            {
                LoadFromEmbeddedResource();
            }
        }
        catch (Exception ex)
        {
            LoadFromEmbeddedResource();
        }
    }

    private static void LoadFromEmbeddedResource()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"WinHubX.Resources.{CurrentLanguage}.json";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                translations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            }
        }
        catch (Exception ex)
        {
        }
    }

    private static void ExtractEmbeddedResource(string resourceName, string outputPath)
    {
        try
        {
            EnsureDirectoriesExist(); // Ensure directory exists before creating file

            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {

                return;
            }

            using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static void SetLanguage(string lang)
    {
        try
        {
            EnsureDirectoriesExist();

            string newLangPath = Path.Combine(LocalAppDataPath, $"{lang}.json");
            string embeddedName = $"WinHubX.Resources.{lang}.json";

            if (!File.Exists(newLangPath))
            {
                ExtractEmbeddedResource(embeddedName, newLangPath);
            }

            if (File.Exists(newLangPath) || TryLoadFromEmbedded(embeddedName))
            {
                CurrentLanguage = lang;
                LoadTranslations();
            }
            else
            {
                if (lang != "it")
                {
                    SetLanguage("it");
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    private static bool TryLoadFromEmbedded(string resourceName)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            return stream != null;
        }
        catch
        {
            return false;
        }
    }

    public static string GetTranslation(string formName, string key)
    {
        if (translations != null &&
            translations.ContainsKey(formName) &&
            translations[formName].ContainsKey(key))
        {
            return translations[formName][key];
        }

        return key; // fallback
    }
}