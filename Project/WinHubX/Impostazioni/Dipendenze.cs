using Newtonsoft.Json;

namespace WinHubX.Impostazioni
{
    internal static class Dipendenze
    {
        public const string GitHubConfigUrl = "https://raw.githubusercontent.com/LightYagami28/WinHubX-Resource/refs/heads/main/Dipendenze.json";
    }

    public static class OfficeSettings
    {
        public static bool SalvaFile { get; set; } = true;
        public static bool Installa { get; set; } = true;

        public static string? LastDownloadedFile { get; set; }
        public static bool HasPendingInstallation { get; set; }
        public static string InstallationType { get; set; } = string.Empty; // "Offline" o "Online"
    }

    public class HardwareInfo
    {
        public string Timestamp { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string Architettura { get; set; } = string.Empty;
        public Hardware Hardware { get; set; } = new();
        public Activation Activation { get; set; } = new();
    }

    public class Hardware
    {
        public string CPU { get; set; } = string.Empty;
        public string RAM { get; set; } = string.Empty;
        public string Disk { get; set; } = string.Empty;
    }

    public class Activation
    {
        public string Windows { get; set; } = string.Empty;
        public string Office { get; set; } = string.Empty;
    }

    public class OfficeVersion
    {
        public string Nome { get; set; } = string.Empty; // Office2019, Office2021, ecc.
        public Dictionary<string, Dictionary<string, string>> Lingue { get; set; } = new();
    }

    public static class AppConfig
    {
        public static string CurrentVersion { get; set; } = "2.5.0.0";

        public static bool CheckUpdatesOnStartup { get; set; } = true;

        private static string SettingsFolder =>
            Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "WinHubX", "Impostazioni");

        private static string SettingsFile =>
            Path.Join(SettingsFolder, "Aggiornamenti.json");

        // -------------------------
        // SALVATAGGIO IMPOSTAZIONI
        // -------------------------
        public static void SaveSettings()
        {
            try
            {
                if (!Directory.Exists(SettingsFolder))
                    Directory.CreateDirectory(SettingsFolder);

                var data = new
                {
                    CheckUpdatesOnStartup = CheckUpdatesOnStartup
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or ArgumentException
                or NotSupportedException)
            {
                MessageBox.Show($"Errore nel salvataggio delle impostazioni: {ex.Message}",
                    "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------
        // CARICAMENTO IMPOSTAZIONI
        // -------------------------
        public static void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsFile))
                    return;

                string json = File.ReadAllText(SettingsFile);
                Newtonsoft.Json.Linq.JObject? data = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
                CheckUpdatesOnStartup = data?["CheckUpdatesOnStartup"]?.ToObject<bool>() ?? true;
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or Newtonsoft.Json.JsonException
                or ArgumentException
                or NotSupportedException
                or FormatException)
            {
                MessageBox.Show($"Errore nel caricamento delle impostazioni: {ex.Message}",
                    "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
