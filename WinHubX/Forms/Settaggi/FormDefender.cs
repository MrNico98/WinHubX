using Microsoft.Win32;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using WinHubX.Forms.Base;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormDefender : Form
    {
        private Form1 form1;
        private FormSettaggi formSettaggi;
        private int totalSteps = 0;
        private int tIndex = -1;
        public FormDefender(FormSettaggi formSettaggi, Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
            LoadCheckboxStates();
            //loadmsginziale();
            DisabilitaDefender.MouseMove += new MouseEventHandler(checkedListBox1_MouseMove);
            AbilitaDefender.MouseMove += new MouseEventHandler(checkedListBox2_MouseMove);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private void checkedListBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int index = DisabilitaDefender.IndexFromPoint(e.Location);
            if (tIndex != index)
            {
                tIndex = index;
                if (tIndex > -1)
                {
                    string tooltipText = GetTooltipTextDisa(tIndex);
                    toolTip1.SetToolTip(DisabilitaDefender, tooltipText);
                }
            }
        }

        private void checkedListBox2_MouseMove(object sender, MouseEventArgs e)
        {
            int index = AbilitaDefender.IndexFromPoint(e.Location);
            if (tIndex != index)
            {
                tIndex = index;
                if (tIndex > -1)
                {
                    string tooltipText = GetTooltipTextAbil(tIndex);
                    toolTip1.SetToolTip(AbilitaDefender, tooltipText);
                }
            }
        }

        private string GetTooltipTextDisa(int index)
        {
            string key = $"desc{index}";
            return LanguageManager.GetTranslation("FormDefender", key);

        }


        private string GetTooltipTextAbil(int index)
        {
            string key = $"abil{index}";
            return LanguageManager.GetTranslation("FormDefender", key);
        }


        private void loadmsginziale()
        {
            _ = MessageBox.Show(
                LanguageManager.GetTranslation("FormDefender", "msgDisinstallaDefender"),
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            form1.lblPanelTitle.Text = "Settaggi";
            form1.PnlFormLoader.Controls.Clear();
            formSettaggi = new FormSettaggi(form1)
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            form1.PnlFormLoader.Controls.Add(formSettaggi);
            formSettaggi.Show();
        }

        private void SetCheckboxState(string itemName, bool isChecked)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\WinHubX"))
            {
                key.SetValue(itemName, isChecked ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        private bool GetCheckboxState(string itemName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\WinHubX"))
            {
                if (key != null)
                {
                    object value = key.GetValue(itemName);
                    if (value != null)
                    {
                        return (int)value == 1;
                    }
                }
            }
            return false;
        }


        private void LoadCheckboxStates()
        {
            int index = DisabilitaDefender.Items.IndexOf("Disabilita Controllo Accesso Cartella");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaControlloAccessoCartella"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Isolamento Core");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaIsolamentoCore"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Applicazione Defender Guard");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaApplicazioneDefenderGuard"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Protezione Account Warning");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaProtezioneAccountWarning"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Blocco Download Files");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaBloccoDownloadFiles"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Windows Script Host");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaWindowsScriptHost"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita .NET Strong Cryptography");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaNETStrongCryptography"));
            }
            index = DisabilitaDefender.Items.IndexOf("Livello Minimo UAC");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("LivelloMinimoUAC"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Implicit Administrative Sheres");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaImplicitAdministrativeSheres"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Windows Firewall");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaWindowsFirewall"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Windows Defender CLoud");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaWindowsDefenderCLoud"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Windows Defender SysTray");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaWindowsDefenderSysTray"));
            }
            index = DisabilitaDefender.Items.IndexOf("Disabilita Windows Defender Services");
            if (index != -1)
            {
                DisabilitaDefender.SetItemChecked(index, GetCheckboxState("DisabilitaWindowsDefenderServices"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Controllo Accesso Cartella");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaControlloAccessoCartella"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Isolamento Core");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaIsolamentoCore"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Applicazione Defender Guard");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaApplicazioneDefenderGuard"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Protezione Account Warning");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaProtezioneAccountWarning"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Blocco Download Files");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaBloccoDownloadFiles"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Windows Script Host");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaWindowsScriptHost"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita .NET Strong Cryptography");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaNETStrongCryptography"));
            }
            index = AbilitaDefender.Items.IndexOf("Livello Massimo UAC");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("LivelloMassimoUAC"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Implicit Administrative Sheres");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaImplicitAdministrativeSheres"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Windows Firewall");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaWindowsFirewall"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Windows Defender CLoud");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaWindowsDefenderCLoud"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Windows Defender SysTray");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaWindowsDefenderSysTray"));
            }
            index = AbilitaDefender.Items.IndexOf("Abilita Windows Defender Services");
            if (index != -1)
            {
                AbilitaDefender.SetItemChecked(index, GetCheckboxState("AbilitaWindowsDefenderServices"));
            }
        }

        private void btnAvviaSelezionatiDef_Click(object sender, EventArgs e)
        {
            totalSteps = 0;
            foreach (var item in DisabilitaDefender.CheckedItems)
            {
                totalSteps++;
            }
            foreach (var item in AbilitaDefender.CheckedItems)
            {
                totalSteps++;
            }

            if (totalSteps == 0)
            {
                totalSteps = 1;
            }

            progressBar1.Maximum = totalSteps;
            progressBar1.Value = 0;

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void btnRipristinaDefender_Click(object sender, EventArgs e)
        {
            try
            {
                SetMpPreference("EnableControlledFolderAccess", false);
                DeleteRegistryKey3arg(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", RegistryView.Registry64);
                DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry64);
                DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry32);
                DeleteRegistryKey3arg(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry32);
                DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\QualityCompat", "cadca5fe-87d3-4b96-b7fb-a231484277cc", RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1, RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry32);
            }
            catch (Exception)
            {

            }
            string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");
            _ = MessageBox.Show(
                messaggio,
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        static void DeleteRegistryKey(string keyPath, RegistryView registryView)
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    baseKey.DeleteSubKeyTree(keyPath);

                }
            }
            catch (Exception)
            {

            }
        }

        static void DeleteRegistryKey3arg(string keyPath, string subKeyName, RegistryView registryView)
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    baseKey.DeleteSubKey(Path.Combine(keyPath, subKeyName), throwOnMissingSubKey: false);
                }
            }
            catch (Exception)
            {

            }
        }

        static void SetDwordRegistryValue(string keyPath, string valueName, int value, RegistryView registryView)
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).CreateSubKey(keyPath, writable: true))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, RegistryValueKind.DWord);

                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        static void RemoveRegistryValue(string keyPath, string valueName)
        {
            try
            {
                using (RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(keyPath, writable: true))
                {
                    key64?.DeleteValue(valueName, throwOnMissingValue: false);
                }
                using (RegistryKey key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(keyPath, writable: true))
                {
                    key32?.DeleteValue(valueName, throwOnMissingValue: false);
                }
            }
            catch (Exception)
            {

            }
        }

        void TakeOwnership(string keyPath, RegistryView registryView)
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(keyPath, writable: true))
                {
                    if (key != null)
                    {
                        RegistrySecurity security = key.GetAccessControl();
                        WindowsIdentity identity = WindowsIdentity.GetCurrent();
                        SecurityIdentifier sid = identity.User;
                        security.AddAccessRule(new RegistryAccessRule(sid, RegistryRights.TakeOwnership, AccessControlType.Allow));
                        key.SetAccessControl(security);
                        key.SetAccessControl(new RegistrySecurity { });
                        key.SetAccessControl(new RegistrySecurity());

                    }
                    else
                    {

                    }
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception)
            {

            }
        }
        private void SetStringRegistryValue(string keyPath, string name, string value, RegistryView view)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
            using (var key = baseKey.CreateSubKey(keyPath))
            {
                key.SetValue(name, value, RegistryValueKind.ExpandString);
            }
        }

        private void TakeOwnRegistry(string keyPath)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c takeown /f \"HKEY_LOCAL_MACHINE\\{keyPath}\" /a",
                Verb = "runas",
                UseShellExecute = true
            };
            var process = System.Diagnostics.Process.Start(psi);
            process.WaitForExit();
        }

        private void SetMpPreference(string preference, bool enabled)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command Set-MpPreference -{preference} {(enabled ? "Enabled" : "Disabled")}",
                Verb = "runas",
                UseShellExecute = true
            };
            var process = System.Diagnostics.Process.Start(psi);
            process.WaitForExit();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int currentStep = 0;
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Controllo Accesso Cartella"))
            {
                SetCheckboxState("DisabilitaControlloAccessoCartella", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetMpPreference("EnableControlledFolderAccess", true);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaControlloAccessoCartella", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Isolamento Core"))
            {
                SetCheckboxState("DisabilitaIsolamentoCore", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", RegistryView.Registry64);
                    DeleteRegistryKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", RegistryView.Registry32);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                SetCheckboxState("DisabilitaIsolamentoCore", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Applicazione Defender Guard"))
            {
                SetCheckboxState("DisabilitaApplicazioneDefernderGuard", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = "Disable-WindowsOptionalFeature -online -FeatureName \"Windows-Defender-ApplicationGuard\" -NoRestart -WarningAction SilentlyContinue",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Verb = "runas"
                    };

                    using (var process = System.Diagnostics.Process.Start(startInfo))
                    {
                        process.WaitForExit();

                        var output = process.StandardOutput.ReadToEnd();
                        var error = process.StandardError.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaApplicazioneDefernderGuard", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Protezione Account Warning"))
            {
                SetCheckboxState("DisabilitaProtezioneAccountWarning", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1, RegistryView.Registry64);
                    SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaProtezioneAccountWarning", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Blocco Download Files"))
            {
                SetCheckboxState("DisabilitaBloccoDownloadFiles", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry64);
                    SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaBloccoDownloadFiles", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Windows Script Host"))
            {
                SetCheckboxState("DisabilitaWindowsScriptHost", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsScriptHost", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita .NET Strong Cryptography"))
            {
                SetCheckboxState("DisabilitaNETStrongCryptography", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaNETStrongCryptography", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Livello Minimo UAC"))
            {
                SetCheckboxState("LivelloMinimoUAC", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("LivelloMinimoUAC", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Implicit Administrative Sheres"))
            {
                SetCheckboxState("DisabilitaImplicitAdministrativeSheres", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaImplicitAdministrativeSheres", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Windows Firewall"))
            {
                SetCheckboxState("DisabilitaWindowsFirewall", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\WindowsFirewall\StandardProfile", "EnableFirewall", 0, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\WindowsFirewall\StandardProfile", "EnableFirewall", 0, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsFirewall", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Windows Defender CLoud"))
            {
                SetCheckboxState("DisabilitaWindowsDefenderCLoud", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsDefenderCLoud", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Windows Defender SysTray"))
            {
                SetCheckboxState("DisabilitaWindowsDefenderSysTray", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    string systrayKeyPath = @"SOFTWARE\Policies\Microsoft\Windows Defender Security Center\Systray";
                    using (var key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(systrayKeyPath, writable: true))
                    {
                        key64?.SetValue("HideSystray", 1, RegistryValueKind.DWord);
                    }
                    using (var key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).CreateSubKey(systrayKeyPath, writable: true))
                    {
                        key32?.SetValue("HideSystray", 1, RegistryValueKind.DWord);
                    }
                    var osVersion = Environment.OSVersion.Version;
                    if (osVersion.Build == 14393)
                    {
                        RemoveRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "WindowsDefender");
                    }
                    else if (osVersion.Build >= 15063)
                    {
                        RemoveRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "SecurityHealth");
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsDefenderSysTray", false);
            }
            if (DisabilitaDefender.CheckedItems.Contains("Disabilita Windows Defender Services"))
            {
                SetCheckboxState("DisabilitaWindowsDefenderServices", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    string[] registryPaths = new[]
                    {
            @"SYSTEM\CurrentControlSet\Services\WinDefend",
            @"SYSTEM\CurrentControlSet\Services\WdNisSvc",
            @"SYSTEM\CurrentControlSet\Services\Sense"
        };

                    foreach (var path in registryPaths)
                    {
                        TakeOwnership(path, RegistryView.Registry64);
                        TakeOwnership(path, RegistryView.Registry32);
                    }
                }
                catch (Exception)
                {

                }
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Controllo Accesso Cartella"))
            {
                SetCheckboxState("AbilitaControlloAccessoCartella", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 1, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 1, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaControlloAccessoCartella", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Isolamento Core"))
            {
                SetCheckboxState("AbilitaIsolamentoCore", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 1, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 1, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaIsolamentoCore", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Applicazione Defender Guard"))
            {
                SetCheckboxState("AbilitaApplicazioneDefenderGuard", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = "Enable-WindowsOptionalFeature -online -FeatureName \"Windows-Defender-ApplicationGuard\" -NoRestart -WarningAction SilentlyContinue",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Verb = "runas"
                    };

                    using (var process = System.Diagnostics.Process.Start(startInfo))
                    {
                        process.WaitForExit();

                        var output = process.StandardOutput.ReadToEnd();
                        var error = process.StandardError.ReadToEnd();
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaApplicazioneDefenderGuard", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Protezione Account Warning"))
            {
                SetCheckboxState("AbilitaProtezioneAccountWarning", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey(@"Software\Microsoft\Windows Security Health\State", RegistryView.Registry64);
                    DeleteRegistryKey(@"Software\Microsoft\Windows Security Health\State", RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaProtezioneAccountWarning", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Blocco Download Files"))
            {
                SetCheckboxState("AbilitaBloccoDownloadFiles", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey3arg(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaBloccoDownloadFiles", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Windows Script Host"))
            {
                SetCheckboxState("AbilitaWindowsScriptHost", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsScriptHost", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita .NET Strong Cryptography"))
            {
                SetCheckboxState("AbilitaNETStrongCryptography", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", 1, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", 1, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", 1, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", 1, RegistryView.Registry64);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaNETStrongCryptography", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Livello Massimo UAC"))
            {
                SetCheckboxState("LivelloMassimoUAC", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5, RegistryView.Registry32);

                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("LivelloMassimoUAC", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Implicit Administrative Sheres"))
            {
                SetCheckboxState("AbilitaImplicitAdministrativeSheres", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey3arg(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaImplicitAdministrativeSheres", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Windows Firewall"))
            {
                SetCheckboxState("AbilitaWindowsFirewall", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\WindowsFirewall\StandardProfile", "EnableFirewall", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\WindowsFirewall\StandardProfile", "EnableFirewall", RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsFirewall", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Windows Defender CLoud"))
            {
                SetCheckboxState("AbilitaWindowsDefenderCLoud", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", RegistryView.Registry32);

                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsDefenderCLoud", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Windows Defender SysTray"))
            {
                SetCheckboxState("AbilitaWindowsDefenderSysTray", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\Windows Defender Security Center\Systray", "HideSystray", RegistryView.Registry64);
                    DeleteRegistryKey3arg(@"SOFTWARE\Policies\Microsoft\Windows Defender Security Center\Systray", "HideSystray", RegistryView.Registry32);
                    var buildVersion = Environment.OSVersion.Version.Build;

                    if (buildVersion == 14393)
                    {
                        SetStringRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "WindowsDefender", @"%ProgramFiles%\Windows Defender\MSASCuiL.exe", RegistryView.Registry64);
                        SetStringRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "WindowsDefender", @"%ProgramFiles%\Windows Defender\MSASCuiL.exe", RegistryView.Registry32);
                    }
                    else if (buildVersion >= 15063 && buildVersion <= 17134)
                    {
                        SetStringRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "SecurityHealth", @"%ProgramFiles%\Windows Defender\MSASCuiL.exe", RegistryView.Registry64);
                        SetStringRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "SecurityHealth", @"%ProgramFiles%\Windows Defender\MSASCuiL.exe", RegistryView.Registry32);
                    }
                    else if (buildVersion >= 17763)
                    {
                        SetStringRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "SecurityHealth", @"%windir%\system32\SecurityHealthSystray.exe", RegistryView.Registry64);
                        SetStringRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "SecurityHealth", @"%windir%\system32\SecurityHealthSystray.exe", RegistryView.Registry32);
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsDefenderSysTray", false);
            }
            if (AbilitaDefender.CheckedItems.Contains("Abilita Windows Defender Services"))
            {
                SetCheckboxState("AbilitaWindowsDefenderServices", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    TakeOwnRegistry(@"SYSTEM\CurrentControlSet\Services\WinDefend");
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WinDefend", "Start", 3, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WinDefend", "AutorunsDisabled", 4, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WdNisSvc", "Start", 3, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WdNisSvc", "AutorunsDisabled", 4, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\Sense", "Start", 3, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\Sense", "AutorunsDisabled", 4, RegistryView.Registry64);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WinDefend", "Start", 3, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WinDefend", "AutorunsDisabled", 4, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WdNisSvc", "Start", 3, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\WdNisSvc", "AutorunsDisabled", 4, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\Sense", "Start", 3, RegistryView.Registry32);
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\Sense", "AutorunsDisabled", 4, RegistryView.Registry32);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsDefenderServices", false);
            }
        }

        private void btnProtezioneMinima_Click(object sender, EventArgs e)
        {
            try
            {
                SetMpPreference("EnableControlledFolderAccess", false);
                DeleteRegistryKey3arg(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", RegistryView.Registry64);
                DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry64);
                DeleteRegistryKey3arg(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry32);
                DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\QualityCompat", "cadca5fe-87d3-4b96-b7fb-a231484277cc", RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1, RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry64);
                DeleteRegistryKey3arg(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", RegistryView.Registry32);
                DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry32);
                DeleteRegistryKey3arg(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry64);
                DeleteRegistryKey3arg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\QualityCompat", "cadca5fe-87d3-4b96-b7fb-a231484277cc", RegistryView.Registry32);
                SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1, RegistryView.Registry32);
                SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry32);
            }
            catch (Exception)
            {

            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = Math.Min(e.ProgressPercentage, progressBar1.Maximum);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");
            _ = MessageBox.Show(
                messaggio,
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}

