using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Personalizzazione_office
{
    public partial class FormAggiungiRimuoviAppOffice : Form
    {
        public string officeVersion;
        public string platform;
        public string product;
        public string culture;
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RegOpenKeyEx(
    IntPtr hKey,
    string subKey,
    int ulOptions,
    int samDesired,
    out IntPtr phkResult);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(IntPtr hKey);

        private const int KEY_WOW64_64KEY = 0x0100;
        private const int KEY_QUERY_VALUE = 0x0001;
        private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(unchecked((int)0x80000002));

        private Dictionary<string, string> officeApps = new Dictionary<string, string>()
        {
            {"word", "Word"},
            {"excel", "Excel"},
            {"powerpoint", "PowerPoint"},
            {"outlook", "Outlook"},
            {"access", "Access"},
            {"onenote", "OneNote"},
            {"groove", "Groove"},
            {"lync", "Skype"},
            {"onedrive", "OneDrive"},
            {"teams", "M.Teams"}
        };
        private Form1 form1;
        private FormOffice formoffice;
        public FormAggiungiRimuoviAppOffice(Form1 form1, FormOffice formoffice)
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            CheckOfficeInstallation();
            DisplayInstallationInfo();
            this.form1 = form1;
            this.formoffice = formoffice;
            flowPanelApps.AutoScroll = true;
            flowPanelApps.WrapContents = true;
            flowPanelApps.FlowDirection = FlowDirection.LeftToRight;
            flowPanelApps.Padding = new Padding(10);
            flowPanelApps.WrapContents = true;
            flowPanelApps.AutoSize = false;
            tableLayoutPanel2.Controls.Add(progressBar1, 0, 1);
            tableLayoutPanel2.SetColumnSpan(progressBar1, 2);
            flowPanelAppsInstall.AutoScroll = true;
            flowPanelAppsInstall.WrapContents = true;
            flowPanelAppsInstall.FlowDirection = FlowDirection.LeftToRight;
            flowPanelAppsInstall.Padding = new Padding(10);
            flowPanelAppsInstall.WrapContents = true;
            flowPanelAppsInstall.AutoSize = false;
            btn_avviaVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Avvia",
                "en" => "  Start",
                _ => btn_avviaVerdi.Content
            };
        }

        private void CheckOfficeInstallation()
        {
            officeVersion = GetRegistryValue(
                @"SOFTWARE\Microsoft\Office\ClickToRun\Configuration",
                "VersionToReport");

            if (string.IsNullOrEmpty(officeVersion))
            {
                officeVersion = GetRegistryValue(
                    @"SOFTWARE\Microsoft\Office\16.0\Common\InstallRoot",
                    "Version");
            }

            if (!string.IsNullOrEmpty(officeVersion) && officeVersion.StartsWith("16.") && IsOfficeWithPublisher())
            {
                if (!officeApps.ContainsKey("publisher"))
                    officeApps.Add("publisher", "Publisher");
            }
        }
        private Dictionary<string, string> DetectInstalledOfficeApps()
        {
            var apps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string[] possibleApps = new[]
            {
        "WINWORD",   
        "EXCEL",      
        "POWERPNT",   
        "MSACCESS",   
        "OUTLOOK",    
        "MSPUB",     
        "ONENOTE",   
        "VISIO",      
        "WINPROJ",    
        "ONEDRIVE"    
    };

            try
            {
                using (var appPathsKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                           .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths"))
                {
                    if (appPathsKey != null)
                    {
                        foreach (var app in possibleApps)
                        {
                            string appKey = $"{app}.exe";
                            using (var appKeyPath = appPathsKey.OpenSubKey(appKey))
                            {
                                if (appKeyPath != null)
                                {
                                    string exePath = appKeyPath.GetValue("")?.ToString();
                                    if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                                    {
                                        string appName = GetFriendlyAppName(app);
                                        apps[appName.ToLower()] = appName;
                                    }
                                }
                            }
                        }
                    }
                }
                if (apps.Count == 0)
                {
                    using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                               .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                    {
                        if (key != null)
                        {
                            string installPath = key.GetValue("InstallPath")?.ToString() ?? "";
                            string clientFolder = key.GetValue("ClientFolder")?.ToString() ?? "";

                            string basePath = !string.IsNullOrEmpty(clientFolder) ? clientFolder : installPath;

                            if (!string.IsNullOrEmpty(basePath))
                            {
                                foreach (var app in possibleApps)
                                {
                                    string appName = GetFriendlyAppName(app);
                                    string exeName = GetExeName(app);
                                    string exePath = Path.Combine(basePath, exeName);

                                    if (File.Exists(exePath))
                                    {
                                        apps[appName.ToLower()] = appName;
                                    }
                                    else
                                    {
                                        string foundPath = SearchFileInDirectory(basePath, exeName);
                                        if (!string.IsNullOrEmpty(foundPath))
                                        {
                                            apps[appName.ToLower()] = appName;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (apps.Count == 0)
                {
                    using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                               .OpenSubKey(@"SOFTWARE\Microsoft\Office"))
                    {
                        if (key != null)
                        {
                            foreach (var version in key.GetSubKeyNames().Where(name => name.Contains(".")))
                            {
                                using (var subKey = key.OpenSubKey($@"{version}\Word\InstallRoot"))
                                {
                                    string path = subKey?.GetValue("Path")?.ToString();
                                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                                    {
                                        foreach (var app in possibleApps)
                                        {
                                            string appName = GetFriendlyAppName(app);
                                            string exeName = GetExeName(app);
                                            string exePath = Path.Combine(path, exeName);

                                            if (File.Exists(exePath))
                                            {
                                                apps[appName.ToLower()] = appName;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (apps.Count == 0)
                {
                    using (var appPathsKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
                               .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths"))
                    {
                        if (appPathsKey != null)
                        {
                            foreach (var app in possibleApps)
                            {
                                string appKey = $"{app}.exe";
                                using (var appKeyPath = appPathsKey.OpenSubKey(appKey))
                                {
                                    if (appKeyPath != null)
                                    {
                                        string exePath = appKeyPath.GetValue("")?.ToString();
                                        if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                                        {
                                            string appName = GetFriendlyAppName(app);
                                            apps[appName.ToLower()] = appName;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (apps.Count == 0)
                {
                    apps = FallbackOfficeDetection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il rilevamento delle app Office: {ex.Message}", "DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Error);
                apps = FallbackOfficeDetection();
            }
            return apps;
        }

        private Dictionary<string, string> FallbackOfficeDetection()
        {
            var apps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] exeNames = {
        "WINWORD.EXE", "EXCEL.EXE", "POWERPNT.EXE", "OUTLOOK.EXE",
        "MSACCESS.EXE", "ONENOTE.EXE", "MSPUB.EXE", "VISIO.EXE", "WINPROJ.EXE"
    };

            string[] possiblePaths = {
        @"C:\Program Files\Microsoft Office\root\Office16",
        @"C:\Program Files\Microsoft Office\root\Office15",
        @"C:\Program Files (x86)\Microsoft Office\root\Office16",
        @"C:\Program Files (x86)\Microsoft Office\root\Office15",
        @"C:\Program Files\Microsoft Office\Office16",
        @"C:\Program Files\Microsoft Office\Office15",
        @"C:\Program Files (x86)\Microsoft Office\Office16",
        @"C:\Program Files (x86)\Microsoft Office\Office15"
    };

            foreach (var path in possiblePaths)
            {
                if (!Directory.Exists(path))
                    continue;

                foreach (var exe in exeNames)
                {
                    string exePath = Path.Combine(path, exe);
                    if (File.Exists(exePath))
                    {
                        string appName = exe switch
                        {
                            "WINWORD.EXE" => "Word",
                            "EXCEL.EXE" => "Excel",
                            "POWERPNT.EXE" => "PowerPoint",
                            "OUTLOOK.EXE" => "Outlook",
                            "MSACCESS.EXE" => "Access",
                            "ONENOTE.EXE" => "OneNote",
                            "MSPUB.EXE" => "Publisher",
                            "VISIO.EXE" => "Visio",
                            "WINPROJ.EXE" => "Project",
                            _ => exe
                        };

                        if (!apps.ContainsKey(appName.ToLower()))
                        {
                            apps[appName.ToLower()] = appName;
                        }
                    }
                }
            }

            return apps;
        }

        private string GetFriendlyAppName(string appCode)
        {
            var nameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {"WINWORD", "Word"},
        {"EXCEL", "Excel"},
        {"POWERPNT", "PowerPoint"},
        {"MSACCESS", "Access"},
        {"OUTLOOK", "Outlook"},
        {"MSPUB", "Publisher"},
        {"ONENOTE", "OneNote"},
        {"VISIO", "Visio"},
        {"WINPROJ", "Project"},
        {"ONEDRIVE", "OneDrive"}
    };

            return nameMap.ContainsKey(appCode) ? nameMap[appCode] : appCode;
        }

        private string GetExeName(string appCode)
        {
            var exeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {"WINWORD", "WINWORD.EXE"},
        {"EXCEL", "EXCEL.EXE"},
        {"POWERPNT", "POWERPNT.EXE"},
        {"MSACCESS", "MSACCESS.EXE"},
        {"OUTLOOK", "OUTLOOK.EXE"},
        {"MSPUB", "MSPUB.EXE"},
        {"ONENOTE", "ONENOTE.EXE"},
        {"VISIO", "VISIO.EXE"},
        {"WINPROJ", "WINPROJ.EXE"},
        {"ONEDRIVE", "ONEDRIVE.EXE"}
    };

            return exeMap.ContainsKey(appCode) ? exeMap[appCode] : $"{appCode}.EXE";
        }

        private string SearchFileInDirectory(string directory, string fileName)
        {
            try
            {
                var files = Directory.GetFiles(directory, fileName, SearchOption.AllDirectories);
                return files.Length > 0 ? files[0] : null;
            }
            catch
            {
                return null;
            }
        }

        private bool IsOfficeWithPublisher()
        {
            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                           .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                {
                    var release = key?.GetValue("ProductReleaseIds")?.ToString()?.ToLower();
                    if (release == null) return false;

                    return release.Contains("365") ||
                           release.Contains("2021") ||
                           release.Contains("2019");
                }
            }
            catch
            {
                return false;
            }
        }

        private List<string> selectedAppsInstall = new List<string>();
        private List<string> selectedAppsRemove = new List<string>();  

        private void DisplayInstalledOfficeApps(string product)
        {
            var installedApps = DetectInstalledOfficeApps(); 

            flowPanelApps.Controls.Clear();
            flowPanelAppsInstall.Controls.Clear();
            flowPanelAppsRimuovi.Controls.Clear();

            Image GetImage(string name)
            {
                bool is365 = product.Contains("365");
                return is365 ? name switch
                {
                    "word" => Properties.Resources.Microsoft_Office_Word_2025present,
                    "excel" => Properties.Resources.Microsoft_Office_Excel_2025present,
                    "powerpoint" => Properties.Resources.Microsoft_Office_PowerPoint_2025present,
                    "access" => Properties.Resources.Microsoft_Office_Access_20192025,
                    "onedrive" => Properties.Resources.Microsoft_OneDrive_Icon_2025present,
                    "onenote" => Properties.Resources.Microsoft_OneNote_Icon_2025present,
                    "outlook" => Properties.Resources.Microsoft_Outlook_Icon_2025present,
                    "publisher" => Properties.Resources.Microsoft_Office_Publisher_2019present,
                    "visio" => Properties.Resources.Microsoft_Office_Visio_2019,
                    "project" => Properties.Resources.Microsoft_Project_2019present,
                    _ => null
                }
                :
                name switch
                {
                    "word" => Properties.Resources.Microsoft_Office_Word_20192025,
                    "excel" => Properties.Resources.Microsoft_Office_Excel_20192025,
                    "powerpoint" => Properties.Resources.Microsoft_Office_PowerPoint_20192025,
                    "access" => Properties.Resources.Microsoft_Office_Access_20192025,
                    "onedrive" => Properties.Resources.Microsoft_Office_OneDrive_20192025,
                    "onenote" => Properties.Resources.Microsoft_Office_OneNote_20192025,
                    "outlook" => Properties.Resources.Microsoft_Office_Outlook_20182024,
                    "publisher" => Properties.Resources.Microsoft_Office_Publisher_2019present,
                    "visio" => Properties.Resources.Microsoft_Office_Visio_2019,
                    "project" => Properties.Resources.Microsoft_Project_2019present,
                    _ => null
                };
            }

            foreach (var app in installedApps)
            {
                var image = GetImage(app.Key);
                if (image == null) continue;

                var item = new AppItem();
                item.SetApp(image, app.Value);
                flowPanelApps.Controls.Add(item);
            }

            foreach (var app in installedApps)
            {
                var image = GetImage(app.Key);
                if (image == null) continue;

                var item = new AppItem();
                item.SetApp(image, app.Value);
                item.Cursor = Cursors.Hand;
                item.BorderStyle = BorderStyle.None;

                item.Click += (s, e) =>
                {
                    if (item.BorderStyle == BorderStyle.None)
                    {
                        item.BorderStyle = BorderStyle.Fixed3D;
                        if (!selectedAppsRemove.Contains(app.Key))
                            selectedAppsRemove.Add(app.Key);
                    }
                    else
                    {
                        item.BorderStyle = BorderStyle.None;
                        selectedAppsRemove.Remove(app.Key);
                    }
                };

                flowPanelAppsRimuovi.Controls.Add(item);
            }

            var missingApps = officeApps.Where(a => !installedApps.ContainsKey(a.Key));

            foreach (var app in missingApps)
            {
                var image = GetImage(app.Key);
                if (image == null) continue;

                var item = new AppItem();
                item.SetApp(image, app.Value);
                item.Cursor = Cursors.Hand;
                item.BorderStyle = BorderStyle.None;

                item.Click += (s, e) =>
                {
                    if (item.BorderStyle == BorderStyle.None)
                    {
                        item.BorderStyle = BorderStyle.Fixed3D;
                        if (!selectedAppsInstall.Contains(app.Key))
                            selectedAppsInstall.Add(app.Key);
                    }
                    else
                    {
                        item.BorderStyle = BorderStyle.None;
                        selectedAppsInstall.Remove(app.Key);
                    }
                };

                flowPanelAppsInstall.Controls.Add(item);
            }
        }

        public static string? GetRegistryValue(string subKey, string valueName)
        {
            IntPtr hKey = IntPtr.Zero;
            try
            {
                int result = RegOpenKeyEx(
                    HKEY_LOCAL_MACHINE,
                    subKey,
                    0,
                    KEY_QUERY_VALUE | KEY_WOW64_64KEY,
                    out hKey);

                if (result == 0 && hKey != IntPtr.Zero)
                {
                    using (RegistryKey key = RegistryKey.FromHandle(new Microsoft.Win32.SafeHandles.SafeRegistryHandle(hKey, true)))
                    {
                        return key.GetValue(valueName)?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
            return null;
        }

        private async void BtnInstall_Click(object sender, EventArgs e)
        {
            string c2rExe = @"C:\Program Files\Common Files\Microsoft Shared\ClickToRun\OfficeClickToRun.exe";
            string arch = Environment.Is64BitOperatingSystem ? "x64" : "x32";
            string version = "unknown";
            string targetEdition = "unknown";

            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                           .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                {
                    if (key != null)
                    {
                        version = key.GetValue("VersionToReport")?.ToString() ?? "unknown";

                        var productValue = key.GetValue("ProductReleaseIds");
                        if (productValue is string str)
                            targetEdition = str;
                        else if (productValue is Array arr)
                            targetEdition = string.Join(", ", arr);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Impossibile leggere le impostazioni di Office.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string lang = NormalizeLanguageCode(culture);
            string updch = GetUpdateChannel(targetEdition);
            string allLangs = GetInstalledOfficeLangs(lang);
            var installedApps = DetectInstalledOfficeApps().Keys.ToList();
            var keepOrInstall = new HashSet<string>(
                installedApps.Concat(selectedAppsInstall)
            );
            foreach (var app in selectedAppsRemove)
                keepOrInstall.Remove(app);
            var excludeList = new StringBuilder();
            foreach (var app in officeApps.Keys)
            {
                if (!keepOrInstall.Contains(app))
                    excludeList.Append($",{app}");
            }
            string c2rCommand = $"\"{c2rExe}\" " +
                                $"platform={arch} culture={lang} " +
                                $"productstoadd={targetEdition}.16_{allLangs} " +
                                $"cdnbaseurl.16=http://officecdn.microsoft.com/pr/{updch} " +
                                $"baseurl.16=http://officecdn.microsoft.com/pr/{updch} " +
                                $"version.16={version} mediatype.16=CDN sourcetype.16=CDN " +
                                $"deliverymechanism={updch} " +
                                $"{targetEdition}.excludedapps.16=groove{excludeList} " +
                                "flt.useteamsaddon=disabled flt.usebingaddononinstall=disabled flt.usebingaddononupdate=disabled";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = c2rExe,
                    Arguments = c2rCommand.Substring(c2rCommand.IndexOf("platform=")),
                    UseShellExecute = false,
                    Verb = "runas",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                progressBar1.Value = 35;

                using (var process = Process.Start(psi))
                {
                    await Task.Run(() => process.WaitForExit());
                }
                progressBar1.Value = 100;
                MessageBox.Show("Operazione completata.\nLe app di Office sono state aggiornate con successo.",
                    "Office Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                progressBar1.Value = 0;

                MessageBox.Show($"Errore durante l'operazione:\n{ex.Message}", "Office Installer",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string NormalizeLanguageCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "it-it";

            input = input.ToLower();

            return input switch
            {
                "it" => "it-it",
                "en" => "en-gb",
                "fr" => "fr-fr",
                "es" => "es-es",
                "de" => "de-de",
                "pt" => "pt-pt",
                "ru" => "ru-ru",
                "ja" => "ja-jp",
                "zh" => "zh-cn",
                _ => input.Contains("-") ? input : $"{input}-{input}"
            };
        }

        private string GetUpdateChannel(string edition)
        {
            string? audienceId = null;
            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                           .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                {
                    audienceId = key?.GetValue("AudienceId")?.ToString();
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(audienceId))
            {
                try
                {
                    using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                               .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                    {
                        audienceId = key?.GetValue("AudienceId")?.ToString();
                    }
                }
                catch { }
            }

            return audienceId;
        }

        private string GetInstalledOfficeLangs(string baseLang)
        {
            var langs = new List<string> { baseLang };

            string officeRegBase = @"SOFTWARE\Microsoft\Office\ClickToRun\Configuration";
            string productReleasePath = @"SOFTWARE\Microsoft\Office\ClickToRun\ProductReleaseIDs";
            string proofLang = baseLang + ".proof";

            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey configKey = baseKey.OpenSubKey(officeRegBase))
                {
                    if (configKey != null)
                    {
                        using (RegistryKey productKey = baseKey.OpenSubKey(productReleasePath))
                        {
                            if (productKey != null)
                            {
                                foreach (string subKeyName in productKey.GetSubKeyNames())
                                {
                                    using (RegistryKey subKey = productKey.OpenSubKey(subKeyName))
                                    {
                                        if (subKey != null)
                                        {
                                            string? modifier = subKey.GetValue("Modifier") as string;
                                            string? original = subKey.GetValue("Original") as string;

                                            if ((!string.IsNullOrEmpty(modifier) && modifier.Contains(proofLang, StringComparison.OrdinalIgnoreCase)) ||
                                                (!string.IsNullOrEmpty(original) && original.Contains(proofLang, StringComparison.OrdinalIgnoreCase)))
                                            {
                                                langs.Add(proofLang);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            langs.Add("x-none");
            return string.Join("_", langs);
        }
        private void DisplayInstallationInfo()
        {
            Version requiredVersion = new Version("16.0.9029.2167");
            Version installedVersion;

            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                       .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                {
                    if (key != null)
                    {
                        culture = key.GetValue("ClientCulture")?.ToString().ToLower() ?? "it-it";
                        var productValue = key.GetValue("ProductReleaseIds");
                        if (productValue != null)
                        {
                            if (productValue is string)
                            {
                                product = productValue.ToString();
                            }
                            else if (productValue is Array)
                            {
                                product = string.Join(", ", (Array)productValue);
                            }
                        }

                        platform = key.GetValue("Platform")?.ToString() ?? "Unknown";
                        officeVersion = key.GetValue("VersionToReport")?.ToString() ?? "";
                    }
                }
            }
            catch
            {
            }

            string versionText = LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "office_non_trovato");

            if (!string.IsNullOrEmpty(officeVersion) && Version.TryParse(officeVersion, out installedVersion))
            {
                if (installedVersion >= requiredVersion)
                {
                    versionText =
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "versione_office"), officeVersion) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "piattaforma"), platform) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "prodotto"), product) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "lingua"), culture);
                }
                else
                {
                    versionText =
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "versione_non_compatibile"), officeVersion) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "versione_richiesta"), requiredVersion.ToString());
                }
            }

            lblversioneoffice.Text = versionText;

            if (!string.IsNullOrEmpty(product))
            {
                DisplayInstalledOfficeApps(product);
            }
            else
            {
                MessageBox.Show(
                    LanguageManager.GetTranslation("FormOffice", "no_office_detected_msg"),
                    LanguageManager.GetTranslation("FormOffice", "no_office_detected_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            if (!string.IsNullOrEmpty(product))
            {
                DisplayInstalledOfficeApps(product);
            }
        }
    }
}