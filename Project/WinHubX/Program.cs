using Microsoft.Win32;
using System.Globalization;
using WinHubX.Impostazioni;

namespace WinHubX
{
    static class Program
    {
        private static ThemeConfig Config;

        [STAThread]
        static void Main(string[] args)
        {
            // Carica configurazione
            Config = ThemeConfig.Load();

            // Imposta lingua in base a Windows se non impostata manualmente
            if (!Config.LanguageManuallySet)
            {
                string systemLang = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
                Config.Language = systemLang.Equals("it", StringComparison.OrdinalIgnoreCase) ? "it" : "en";
                Config.Save();
            }

            // Applica la lingua globalmente
            LanguageManager.LoadLanguageFromSettings();
            LanguageManager.SetLanguage(Config.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Config.Language);

            // Imposta tema in base a Windows se non impostato manualmente
            if (!Config.ThemeManuallySet)
            {
                bool darkTheme = IsSystemInDarkMode();
                Config.DarkTheme = darkTheme;
                Config.Save();
            }

            // Applica tema
            ThemeManager.SetTheme(Config.DarkTheme);

            // Rileva cambiamenti di tema o lingua in Windows
            SystemEvents.UserPreferenceChanged += (s, e) =>
            {
                if (e.Category == UserPreferenceCategory.General && !Config.ThemeManuallySet)
                {
                    bool darkNow = IsSystemInDarkMode();
                    if (darkNow != ThemeManager.IsDarkTheme)
                    {
                        ThemeManager.SetTheme(darkNow);
                        Config.DarkTheme = darkNow;
                        Config.Save();
                    }
                }

                if (e.Category == UserPreferenceCategory.Locale && !Config.LanguageManuallySet)
                {
                    string newLang = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
                    newLang = newLang.Equals("it", StringComparison.OrdinalIgnoreCase) ? "it" : "en";

                    if (newLang != Config.Language)
                    {
                        Config.Language = newLang;
                        Config.Save();
                        LanguageManager.SetLanguage(newLang);
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(newLang);
                    }
                }
            };

            // Avvia app
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LanguageManager.SetLanguage(Config.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Config.Language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Config.Language);
            Application.Run(new Form1());
        }

        public static bool IsSystemInDarkMode()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    object value = key?.GetValue("AppsUseLightTheme");
                    if (value is int v)
                        return v == 0; // 0 = dark, 1 = light
                }
            }
            catch { }
            return false;
        }
    }
}