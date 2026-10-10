using Newtonsoft.Json;

using System.Diagnostics;
using System.Text;

namespace WinHubX
{
    public class ThemeConfig
    {
        public bool DarkTheme { get; set; } = false;
        public bool ThemeManuallySet { get; set; } = false;
        public string Language { get; set; } = "en";
        public bool LanguageManuallySet { get; set; } = false;

        private static readonly string ConfigPath =
            Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WinHubX", "Impostazioni", "Tema.json");

        public static ThemeConfig Load() => Load(ConfigPath);

        internal static ThemeConfig Load(string path)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(path);
                if (!File.Exists(path))
                {
                    var defaults = new ThemeConfig();
                    defaults.Save(path);
                    return defaults;
                }

                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<ThemeConfig>(json) ?? new ThemeConfig();
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or Newtonsoft.Json.JsonException
                or ArgumentException
                or NotSupportedException)
            {
                Debug.WriteLine($"Unable to read theme settings; defaults will be used: {ex}");
                return new ThemeConfig();
            }
        }

        public void Save() => Save(ConfigPath);

        internal void Save(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            string fullPath = Path.GetFullPath(path);
            string directory = Path.GetDirectoryName(fullPath)!;
            Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            string temporaryPath = Path.Join(directory, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

            try
            {
                File.WriteAllText(temporaryPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                File.Move(temporaryPath, fullPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }
    }
}
