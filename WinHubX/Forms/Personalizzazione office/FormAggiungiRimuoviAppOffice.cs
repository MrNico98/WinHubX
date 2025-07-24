using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;

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
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            CheckOfficeInstallation();
            DisplayInstallationInfo();
            CreateAppSelectionControls();
            this.form1 = form1;
            this.formoffice = formoffice;
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
                if (hKey != IntPtr.Zero)
                {
                    //RegCloseKey(hKey);
                }
            }
            return null;
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void CreateAppSelectionControls()
        {
            int yPos = 20;
            foreach (var app in officeApps.Keys)
            {
                CheckBox chk = new CheckBox();
                chk.Text = officeApps[app];
                chk.Tag = app;
                chk.AutoSize = true;
                chk.Location = new System.Drawing.Point(20, yPos);
                chk.CheckedChanged += CheckBox_CheckedChanged;
                yPos += 30;
                panelSelection.Controls.Add(chk);
            }
            UpdateLabels();
        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }
        private void UpdateLabels()
        {
            List<string> aggiungiList = new List<string>();
            List<string> rimuoviList = new List<string>();

            foreach (Control ctrl in panelSelection.Controls)
            {
                if (ctrl is CheckBox chk)
                {
                    string appName = chk.Text;

                    if (chk.Checked)
                        aggiungiList.Add(appName);
                    else
                        rimuoviList.Add(appName);
                }
            }

            string aggiungiLabel = LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "aggiungi");
            string rimuoviLabel = LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "rimuovi");
            string nessunaLabel = LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "nessuna");

            labelAggiungi.Text = $"{aggiungiLabel}: " + (aggiungiList.Count > 0 ? string.Join(", ", aggiungiList) : nessunaLabel);
            labelRimuovi.Text = $"{rimuoviLabel}: " + (rimuoviList.Count > 0 ? string.Join(", ", rimuoviList) : nessunaLabel);
        }
        private async void BtnInstall_Click(object sender, EventArgs e)
        {
            string c2rExe = @"C:\Program Files\Common Files\Microsoft Shared\ClickToRun\OfficeClickToRun.exe";
            string arch = Environment.Is64BitOperatingSystem ? "x64" : "x32";
            string version = "unknown";
            string lang = "it-it";
            string targetEdition = "unknown";

            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                           .OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration"))
                {
                    if (key != null)
                    {
                        version = key.GetValue("VersionToReport")?.ToString() ?? "unknown";
                        lang = key.GetValue("ClientCulture")?.ToString().ToLower() ?? "it-it";

                        var productValue = key.GetValue("ProductReleaseIds");
                        if (productValue is string str)
                            targetEdition = str;
                        else if (productValue is Array arr)
                            targetEdition = string.Join(", ", arr);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
            string updch = GetUpdateChannel(targetEdition);
            string allLangs = GetInstalledOfficeLangs(lang);
            string excludeList = "";
            foreach (Control ctrl in panelSelection.Controls)
            {
                if (ctrl is CheckBox chk && !chk.Checked)
                {
                    string? app = chk.Tag?.ToString();
                    if (!string.IsNullOrWhiteSpace(app))
                        excludeList += $",{app}";
                }
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

                using (var process = Process.Start(psi))
                {
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    progressBar1.MarqueeAnimationSpeed = 30;
                    progressBar1.Visible = true;

                    await Task.Run(() => process.WaitForExit());

                    progressBar1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            btnAvvia.Visible = false;

            if (!string.IsNullOrEmpty(officeVersion) && Version.TryParse(officeVersion, out installedVersion))
            {
                if (installedVersion >= requiredVersion)
                {
                    versionText =
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "versione_office"), officeVersion) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "piattaforma"), platform) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "prodotto"), product) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "lingua"), culture);

                    btnAvvia.Visible = true;
                    if (product.Contains("365"))
                    {
                        pictureBox1.Image = Properties.Resources.png365;
                    }
                    else if (product.Contains("2021") || product.Contains("2019"))
                    {
                        pictureBox1.Image = Properties.Resources.pngOffice;
                    }
                    else if (product.Contains("2024"))
                    {
                        pictureBox1.Image = Properties.Resources.pngOfficeHome;
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
                else
                {
                    versionText =
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "versione_non_compatibile"), officeVersion) + "\n" +
                        string.Format(LanguageManager.GetTranslation("FormOfficeAggiungiRimuovi", "versione_richiesta"), requiredVersion.ToString());
                }
            }

            lblVersion.Text = versionText;
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Office";
            form1.PnlFormLoader.Controls.Clear();
            formoffice = new FormOffice(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formoffice);
            ThemeManager.ApplyThemeToControl(formoffice, ThemeManager.IsDarkTheme);
            formoffice.Show();
        }
    }
}