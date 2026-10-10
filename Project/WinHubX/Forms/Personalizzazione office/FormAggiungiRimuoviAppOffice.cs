using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Personalizzazione_office
{
    public partial class FormAggiungiRimuoviAppOffice : Form
    {
        private static bool IsExpectedOfficeDiscoveryFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or InvalidOperationException
                or NotSupportedException
                or ArgumentException;

        private static readonly Dictionary<string, string> FriendlyApplicationNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["WINWORD"] = "Word",
            ["EXCEL"] = "Excel",
            ["POWERPNT"] = "PowerPoint",
            ["MSACCESS"] = "Access",
            ["OUTLOOK"] = "Outlook",
            ["MSPUB"] = "Publisher",
            ["ONENOTE"] = "OneNote",
            ["VISIO"] = "Visio",
            ["WINPROJ"] = "Project",
            ["ONEDRIVE"] = "OneDrive"
        };
        private static readonly Dictionary<string, string> ExecutableNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["WINWORD"] = "WINWORD.EXE",
            ["EXCEL"] = "EXCEL.EXE",
            ["POWERPNT"] = "POWERPNT.EXE",
            ["MSACCESS"] = "MSACCESS.EXE",
            ["OUTLOOK"] = "OUTLOOK.EXE",
            ["MSPUB"] = "MSPUB.EXE",
            ["ONENOTE"] = "ONENOTE.EXE",
            ["VISIO"] = "VISIO.EXE",
            ["WINPROJ"] = "WINPROJ.EXE",
            ["ONEDRIVE"] = "ONEDRIVE.EXE"
        };

        public string officeVersion = string.Empty;
        public string platform = string.Empty;
        public string product = string.Empty;
        public string culture = string.Empty;
        private readonly Dictionary<string, string> officeApps = new Dictionary<string, string>()
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
        private readonly Form1 form1;
        private readonly FormOffice formoffice;
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
                "VersionToReport") ?? string.Empty;

            if (string.IsNullOrEmpty(officeVersion))
            {
                officeVersion = GetRegistryValue(
                    @"SOFTWARE\Microsoft\Office\16.0\Common\InstallRoot",
                    "Version") ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(officeVersion) && officeVersion.StartsWith("16.", StringComparison.Ordinal) && IsOfficeWithPublisher())
                officeApps.TryAdd("publisher", "Publisher");
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
                                    string? exePath = appKeyPath.GetValue("")?.ToString();
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
                                HashSet<string>? nestedExecutableNames = null;
                                foreach (var app in possibleApps)
                                {
                                    string appName = GetFriendlyAppName(app);
                                    string exeName = GetExeName(app);
                                    string exePath = Path.Join(basePath, exeName);

                                    if (File.Exists(exePath))
                                    {
                                        apps[appName.ToLower()] = appName;
                                    }
                                    else
                                    {
                                        nestedExecutableNames ??= FindOfficeExecutableNames(basePath);
                                        if (nestedExecutableNames.Contains(exeName))
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
                                    string? path = subKey?.GetValue("Path")?.ToString();
                                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                                    {
                                        foreach (var app in possibleApps)
                                        {
                                            string appName = GetFriendlyAppName(app);
                                            string exeName = GetExeName(app);
                                            string exePath = Path.Join(path, exeName);

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
                                            string? exePath = appKeyPath.GetValue("")?.ToString();
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
            catch (Exception ex) when (IsExpectedOfficeDiscoveryFailure(ex))
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

            foreach (string path in possiblePaths.Where(Directory.Exists))
            {
                foreach (var exe in exeNames)
                {
                    string exePath = Path.Join(path, exe);
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
            return FriendlyApplicationNames.TryGetValue(appCode, out string? friendlyName)
                ? friendlyName
                : appCode;
        }

        private string GetExeName(string appCode)
        {
            return ExecutableNames.TryGetValue(appCode, out string? executableName)
                ? executableName
                : $"{appCode}.EXE";
        }

        private static HashSet<string> FindOfficeExecutableNames(string directory)
        {
            var executableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var options = new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    AttributesToSkip = FileAttributes.ReparsePoint,
                    ReturnSpecialDirectories = false
                };

                foreach (string path in Directory.EnumerateFiles(directory, "*.exe", options))
                    executableNames.Add(Path.GetFileName(path));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                Debug.WriteLine($"Scansione eseguibili Office in '{directory}' incompleta: {ex}");
            }

            return executableNames;
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
            catch (Exception ex) when (IsExpectedOfficeDiscoveryFailure(ex))
            {
                Debug.WriteLine($"Rilevamento versione Office non riuscito: {ex}");
                return false;
            }
        }

        private readonly List<string> selectedAppsInstall = new List<string>();
        private readonly List<string> selectedAppsRemove = new List<string>();

        private void DisplayInstalledOfficeApps(string productCode)
        {
            var installedApps = DetectInstalledOfficeApps(); 

            flowPanelApps.Controls.Clear();
            flowPanelAppsInstall.Controls.Clear();
            flowPanelAppsRimuovi.Controls.Clear();

            Image? GetImage(string name)
            {
                bool is365 = productCode.Contains("365", StringComparison.OrdinalIgnoreCase);
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
            try
            {
                using RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                using RegistryKey? key = baseKey.OpenSubKey(subKey, writable: false);
                return key?.GetValue(valueName)?.ToString();
            }
            catch (Exception ex) when (ex is ArgumentException
                or IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException)
            {
                Debug.WriteLine($"Lettura del valore Registro HKLM\\{subKey}\\{valueName} non riuscita: {ex}");
                _ = MessageBox.Show($"Lettura del Registro non riuscita: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private async void BtnInstall_Click(object? sender, EventArgs e)
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
            catch (Exception ex) when (IsExpectedOfficeDiscoveryFailure(ex))
            {
                Debug.WriteLine($"Lettura configurazione Office non riuscita: {ex}");
                MessageBox.Show("Impossibile leggere le impostazioni di Office.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string lang = NormalizeLanguageCode(culture);
            string updch = GetUpdateChannel(targetEdition) ?? string.Empty;
            string allLangs = GetInstalledOfficeLangs(lang);
            var installedApps = DetectInstalledOfficeApps().Keys.ToList();
            var keepOrInstall = new HashSet<string>(
                installedApps.Concat(selectedAppsInstall)
            );
            foreach (var app in selectedAppsRemove)
                keepOrInstall.Remove(app);
            var excludeList = new StringBuilder();
            foreach (var app in officeApps.Keys.Where(app => !keepOrInstall.Contains(app)))
            {
                excludeList.Append($",{app}");
            }
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = c2rExe,
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                psi.ArgumentList.Add($"platform={arch}");
                psi.ArgumentList.Add($"culture={lang}");
                psi.ArgumentList.Add($"productstoadd={targetEdition}.16_{allLangs}");
                psi.ArgumentList.Add($"cdnbaseurl.16=https://officecdn.microsoft.com/pr/{updch}");
                psi.ArgumentList.Add($"baseurl.16=https://officecdn.microsoft.com/pr/{updch}");
                psi.ArgumentList.Add($"version.16={version}");
                psi.ArgumentList.Add("mediatype.16=CDN");
                psi.ArgumentList.Add("sourcetype.16=CDN");
                psi.ArgumentList.Add($"deliverymechanism={updch}");
                psi.ArgumentList.Add($"{targetEdition}.excludedapps.16=groove{excludeList}");
                psi.ArgumentList.Add("flt.useteamsaddon=disabled");
                psi.ArgumentList.Add("flt.usebingaddononinstall=disabled");
                psi.ArgumentList.Add("flt.usebingaddononupdate=disabled");
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                progressBar1.Value = 35;

                using (var process = Process.Start(psi)
                    ?? throw new InvalidOperationException("Impossibile avviare l'installer Office."))
                {
                    await process.WaitForExitAsync();
                }
                progressBar1.Value = 100;
                MessageBox.Show("Operazione completata.\nLe app di Office sono state aggiornate con successo.",
                    "Office Installer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) when (IsExpectedOfficeDiscoveryFailure(ex))
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

        private string? GetUpdateChannel(string edition)
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
            catch (Exception ex) when (IsExpectedOfficeDiscoveryFailure(ex))
            {
                Debug.WriteLine($"Lettura AudienceId Office in vista registro a 32 bit non riuscita: {ex}");
            }
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
                catch (Exception ex) when (IsExpectedOfficeDiscoveryFailure(ex))
                {
                    Debug.WriteLine($"Lettura AudienceId Office in vista registro a 64 bit non riuscita: {ex}");
                }
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
                using (RegistryKey? configKey = baseKey.OpenSubKey(officeRegBase))
                {
                    if (configKey != null)
                    {
                        using (RegistryKey? productKey = baseKey.OpenSubKey(productReleasePath))
                        {
                            if (productKey != null)
                            {
                                foreach (string subKeyName in productKey.GetSubKeyNames())
                                {
                                    using (RegistryKey? subKey = productKey.OpenSubKey(subKeyName))
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
            Version? installedVersion;

            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                       .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                {
                    if (key != null)
                    {
                        culture = key.GetValue("ClientCulture")?.ToString()?.ToLowerInvariant() ?? "it-it";
                        var productValue = key.GetValue("ProductReleaseIds");
                        if (productValue != null)
                        {
                            if (productValue is string)
                            {
                                product = productValue as string ?? string.Empty;
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
            catch (Exception ex) when (IsExpectedOfficeDiscoveryFailure(ex))
            {
                Debug.WriteLine($"Lettura configurazione Click-to-Run non riuscita: {ex}");
            }

            string versionText = LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "office_non_trovato");

            if (!string.IsNullOrEmpty(officeVersion) && Version.TryParse(officeVersion, out installedVersion))
            {
                versionText = installedVersion >= requiredVersion
                    ?
                        LanguageManager.FormatTranslation("FormOfficeAggiungiRimuovi", "versione_office", officeVersion) + "\n" +
                        LanguageManager.FormatTranslation("FormOfficeAggiungiRimuovi", "piattaforma", platform) + "\n" +
                        LanguageManager.FormatTranslation("FormOfficeAggiungiRimuovi", "prodotto", product) + "\n" +
                        LanguageManager.FormatTranslation("FormOfficeAggiungiRimuovi", "lingua", culture)
                    :
                        LanguageManager.FormatTranslation("FormOfficeAggiungiRimuovi", "versione_non_compatibile", officeVersion) + "\n" +
                        LanguageManager.FormatTranslation("FormOfficeAggiungiRimuovi", "versione_richiesta", requiredVersion);
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
        }
    }
}
