using Newtonsoft.Json;

namespace WinHubX
{
    public class ThemeConfig
    {
        public bool DarkTheme { get; set; } = false;
        public bool ThemeManuallySet { get; set; } = false;
        public string Language { get; set; } = "en";
        public bool LanguageManuallySet { get; set; } = false;

        private static readonly string ConfigPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WinHubX", "Impostazioni", "Tema.json");

        public static ThemeConfig Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                    var def = new ThemeConfig();
                    def.Save();
                    return def;
                }

                string json = File.ReadAllText(ConfigPath);
                return JsonConvert.DeserializeObject<ThemeConfig>(json) ?? new ThemeConfig();
            }
            catch
            {
                return new ThemeConfig();
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
            File.WriteAllText(ConfigPath, json);
        }
    }
}