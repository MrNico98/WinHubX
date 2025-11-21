using Newtonsoft.Json;

namespace WinHubX.Impostazioni
{
    internal static class Dipendenze
    {
        public const string GitHubConfigUrl = "https://raw.githubusercontent.com/MrNico98/WinHubX-Resource/refs/heads/main/Dipendenze.json";
    }

    public static class OfficeSettings
    {
        public static bool SalvaFile { get; set; } = true;
        public static bool Installa { get; set; } = true;

        public static string LastDownloadedFile { get; set; }
        public static bool HasPendingInstallation { get; set; }
        public static string InstallationType { get; set; } // "Offline" o "Online"
    }

    public class HardwareInfo
    {
        public string Timestamp { get; set; }
        public string OperatingSystem { get; set; }
        public string Architettura { get; set; }
        public Hardware Hardware { get; set; }
        public Activation Activation { get; set; }
    }

    public class Hardware
    {
        public string CPU { get; set; }
        public string RAM { get; set; }
        public string Disk { get; set; }
    }

    public class Activation
    {
        public string Windows { get; set; }
        public string Office { get; set; }
    }

    public class OfficeVersion
    {
        public string Nome { get; set; } // Office2019, Office2021, ecc.
        public Dictionary<string, Dictionary<string, string>> Lingue { get; set; }
    }

    public static class AppConfig
    {
        public static string CurrentVersion { get; set; } = "2.5.0.0";

        public static bool CheckUpdatesOnStartup { get; set; } = true;

        private static string SettingsFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "WinHubX", "Impostazioni");

        private static string SettingsFile =>
            Path.Combine(SettingsFolder, "Aggiornamenti.json");

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
            catch (Exception ex)
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
                dynamic data = JsonConvert.DeserializeObject(json);

                CheckUpdatesOnStartup = data.CheckUpdatesOnStartup ?? true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel caricamento delle impostazioni: {ex.Message}",
                    "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}