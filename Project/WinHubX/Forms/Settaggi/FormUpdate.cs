using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormUpdate : Form
    {
        private Form1 form1;
        private FormSettaggi formSettaggi;
        private int totalSteps = 0;
        private int tIndex = -1;
        public FormUpdate(FormSettaggi formSettaggi, Form1 form1)
        {
            LanguageManager.LoadTranslations();
            InitializeComponent();
            form1 = form1;
            formSettaggi = formSettaggi;
            LoadCheckboxStates();
            DisabilitaUpdate.MouseMove += new MouseEventHandler(checkedListBox1_MouseMove);
            AbilitaUpdate.MouseMove += new MouseEventHandler(checkedListBox2_MouseMove);
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btnAvviaSelezionatiVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Avvia",
                "en" => "  Start",
                _ => btnAvviaSelezionatiVerdi.Content
            };
            btnSuggeritiVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Applica",
                "en" => "  Apply",
                _ => btnSuggeritiVerdi.Content
            };
            btnRipristinaWinUpdateVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Ripristina",
                "en" => "  Restore",
                _ => btnRipristinaWinUpdateVerdi.Content
            };
            btnUpdateEssenzialeVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Update essenziale",
                "en" => "  Essenzial update",
                _ => btnUpdateEssenzialeVerdi.Content
            };
        }

        private void checkedListBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int index = DisabilitaUpdate.IndexFromPoint(e.Location);
            if (tIndex != index)
            {
                tIndex = index;
                if (tIndex > -1)
                {
                    string tooltipText = GetTooltipTextDisa(tIndex);
                    toolTip1.SetToolTip(DisabilitaUpdate, tooltipText);
                }
            }
        }

        private void checkedListBox2_MouseMove(object sender, MouseEventArgs e)
        {
            int index = AbilitaUpdate.IndexFromPoint(e.Location);
            if (tIndex != index)
            {
                tIndex = index;
                if (tIndex > -1)
                {
                    string tooltipText = GetTooltipTextAbil(tIndex);
                    toolTip1.SetToolTip(AbilitaUpdate, tooltipText);
                }
            }
        }

        private string GetTooltipTextDisa(int index)
        {
            return LanguageManager.GetTranslation("FormUpdate", $"tooltipDisabilita_{index}");
        }
        private string GetTooltipTextAbil(int index)
        {
            return LanguageManager.GetTranslation("FormUpdate", $"tooltipAbilita_{index}");
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
            var checkboxMappings = new (CheckedListBox box, string displayName, string regKey)[]
            {
        (DisabilitaUpdate, "Disabilita Download Automatico Windows Update", "DisabilitaDownloadAutomaticoWindowsUpdate"),
        (DisabilitaUpdate, "Disabilita Update Prodotti Microsoft", "DisabilitaUpdateProdottiMicrosoft"),
        (DisabilitaUpdate, "Disabilita Download Driver Windows Update", "DisabilitaDownloadDriverWindowsUpdate"),
        (DisabilitaUpdate, "Disabilita Riavvio Automatico Windows Update", "DisabilitaRiavvioAutomaticoWindowsUpdate"),
        (DisabilitaUpdate, "Disabilita Notifiche Update", "DisabilitaNotificheUpdate"),
        (AbilitaUpdate, "Abilita Download Automatico Windows Update", "AbilitaDownloadAutomaticoWindowsUpdate"),
        (AbilitaUpdate, "Abilita Update Prodotti Microsoft", "AbilitaUpdateProdottiMicrosoft"),
        (AbilitaUpdate, "Abilita Download Driver Windows Update", "AbilitaDownloadDriverWindowsUpdate"),
        (AbilitaUpdate, "Abilita Riavvio Automatico Windows Update", "AbilitaRiavvioAutomaticoWindowsUpdate"),
        (AbilitaUpdate, "Abilita Notifiche Update", "AbilitaNotificheUpdate"),
            };

            foreach (var (box, displayName, regKey) in checkboxMappings)
            {
                int index = box.Items.IndexOf(displayName);
                if (index != -1)
                {
                    box.SetItemChecked(index, GetCheckboxState(regKey));
                }
            }
        }


        private void btnAvviaSelezionatiUpda_Click(object sender, EventArgs e)
        {
            totalSteps = 0;

            totalSteps = DisabilitaUpdate.CheckedItems.Count + AbilitaUpdate.CheckedItems.Count;
            if (totalSteps == 0) totalSteps = 1;
            progressBar2.MaxValue = totalSteps;
            progressBar2.Value = 0;

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void btnUpdateEssential_Click(object sender, EventArgs e)
        {
            try
            {
                var registryChanges = new (string Path, string Name, int Value)[]
                {
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\Device Metadata", "PreventDeviceMetadataFromNetwork", 1),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontPromptForWindowsUpdate", 1),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontSearchWindowsUpdate", 1),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DriverUpdateWizardWuSearchEnabled", 0),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", 1),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", 1),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUPowerManagement", 0)
                };
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, true);
                }
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, false);
                }

                string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

                _ = MessageBox.Show(
                    messaggio,
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception)
            {

            }
        }

        private void UpdateRegistry(string path, string name, int value, bool is64Bit)
        {
            var regPath = is64Bit ? path : path.Replace("SOFTWARE", "SOFTWARE\\WOW6432Node");

            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "reg.exe",
                Arguments = $"add \"{regPath}\" /v \"{name}\" /t REG_DWORD /d {value} /f",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                process.WaitForExit();

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Error: {error}");
                }
            }
        }

        private void btnResetUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var registryChanges = new (string Path, string Name, int Value)[]
                {
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", 0),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUOptions", 3),
            (@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\DeliveryOptimization\Config", "DODownloadMode", 1)
                };
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, true);
                }
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, false);
                }
                StartService("BITS");
                StartService("wuauserv");
                var registryRemovals = new[]
                {
            @"HKLM\SOFTWARE\Policies\Microsoft\Windows\Device Metadata", "PreventDeviceMetadataFromNetwork",
            @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontPromptForWindowsUpdate",
            @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontSearchWindowsUpdate",
            @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DriverUpdateWizardWuSearchEnabled",
            @"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate",
            @"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers",
            @"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUPowerManagement",
            @"HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "BranchReadinessLevel",
            @"HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "DeferFeatureUpdatesPeriodInDays",
            @"HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "DeferQualityUpdatesPeriodInDays"
                };

                foreach (var removal in registryRemovals)
                {
                    RemoveRegistryValue(removal, true);
                    RemoveRegistryValue(removal, false);
                }
                string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

                _ = MessageBox.Show(
                    messaggio,
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception)
            {

            }
        }

        private void RemoveRegistryValue(string path, bool is64Bit)
        {
            var regPath = is64Bit ? path : path.Replace("SOFTWARE", "SOFTWARE\\WOW6432Node");

            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "reg.exe",
                Arguments = $"delete \"{regPath}\" /f",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                process.WaitForExit();

                var error = process.StandardError.ReadToEnd();
                if (process.ExitCode != 0 && !error.Contains("ERROR_FILE_NOT_FOUND"))
                {
                    throw new Exception($"Failed to remove registry value: {error}");
                }
            }
        }

        private void StartService(string serviceName)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "sc.exe",
                Arguments = $"config \"{serviceName}\" start= auto",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                process.WaitForExit();

                var error = process.StandardError.ReadToEnd();
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Error {serviceName}: {error}");
                }
            }
        }

        private void ModificaChiaveRegistro(RegistryView view)
        {
            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view).CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Device Metadata", true))
                {
                    key?.SetValue("PreventDeviceMetadataFromNetwork", 1, RegistryValueKind.DWord);
                }

                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view).CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", true))
                {
                    key?.SetValue("SearchOrderConfig", 0, RegistryValueKind.DWord);
                }

                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view).CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", true))
                {
                    key?.SetValue("ExcludeWUDriversInQualityUpdate", 1, RegistryValueKind.DWord);
                }
            }
            catch (Exception)
            {

            }
        }
        private void ModificaDownloadAutomatico(RegistryView view)
        {
            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view).CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", true))
                {
                    key?.SetValue("AUOptions", 2, RegistryValueKind.DWord);
                }
            }
            catch (Exception)
            {

            }
        }

        private void RimuoviDriverUpdate()
        {
            using (var key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                key64.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\Device Metadata", false);
                key32.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\Device Metadata", false);
                key64.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", false);
                key32.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", false);
                key64.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", false);
                key32.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", false);
            }
        }

        private void RimuoviRiavvioAutomatico()
        {
            using (var key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                key64.DeleteValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\MusNotification.exe", false);
                key32.DeleteValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\MusNotification.exe", false);
            }
        }
        private void ModificaNotificheUpdate(bool enable)
        {
            string cmdArgument = enable
                ? "/c takeown /F \"%WinDIR%\\System32\\MusNotification.exe\" && icacls \"%WinDIR%\\System32\\MusNotification.exe\" /allow \"%EveryOne%:(X)\" && takeown /F \"%WinDIR%\\System32\\MusNotificationUx.exe\" && icacls \"%WinDIR%\\System32\\MusNotificationUx.exe\" /allow \"%EveryOne%:(X)\""
                : "/c takeown /F \"%WinDIR%\\System32\\MusNotification.exe\" && icacls \"%WinDIR%\\System32\\MusNotification.exe\" /deny \"%EveryOne%:(X)\" && takeown /F \"%WinDIR%\\System32\\MusNotificationUx.exe\" && icacls \"%WinDIR%\\System32\\MusNotificationUx.exe\" /deny \"%EveryOne%:(X)\"";

            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = cmdArgument,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas"
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
            }
        }

        private void RimuoviAUOptions()
        {
            using (var key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                key64.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", false);
                key32.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", false);
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int currentStep = 0;
            if (DisabilitaUpdate.CheckedItems.Contains("Disabilita Download Automatico Windows Update"))
            {
                SetCheckboxState("DisabilitaDownloadAutomaticoWindowsUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaDownloadAutomatico(RegistryView.Registry32);
                    ModificaDownloadAutomatico(RegistryView.Registry64);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaDownloadAutomaticoWindowsUpdate", false);
            }
            if (DisabilitaUpdate.CheckedItems.Contains("Disabilita Update Prodotti Microsoft"))
            {
                SetCheckboxState("DisabilitaUpdateProdottiMicrosoft", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = @"
                        If ((New-Object -ComObject Microsoft.Update.ServiceManager).Services | Where-Object { $_.ServiceID -eq ""7971f918-a847-4430-9279-4a52d1efe18d""}) {
                        (New-Object -ComObject Microsoft.Update.ServiceManager).RemoveService(""7971f918-a847-4430-9279-4a52d1efe18d"")
                          }
                           ",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Verb = "runus"
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
                SetCheckboxState("DisabilitaUpdateProdottiMicrosoft", false);
            }
            if (DisabilitaUpdate.CheckedItems.Contains("Disabilita Download Driver Windows Update"))
            {
                SetCheckboxState("DisabilitaDownloadDriverWindowsUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaChiaveRegistro(RegistryView.Registry32);
                    ModificaChiaveRegistro(RegistryView.Registry64);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaDownloadDriverWindowsUpdate", false);
            }
            if (DisabilitaUpdate.CheckedItems.Contains("Disabilita Riavvio Automatico Windows Update"))
            {
                SetCheckboxState("DisabilitaRiavvioAutomaticoWindowsUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("NoAutoRebootWithLoggedOnUsers", 1, RegistryValueKind.DWord);
                            key.SetValue("AUPowerManagement", 0, RegistryValueKind.DWord);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaRiavvioAutomaticoWindowsUpdate", false);
            }
            if (DisabilitaUpdate.CheckedItems.Contains("Disabilita Notifiche Update"))
            {
                SetCheckboxState("DisabilitaNotificheUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaNotificheUpdate(false);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("DisabilitaNotificheUpdate", false);
            }
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Download Automatico Windows Update"))
            {
                SetCheckboxState("AbilitaDownloadAutomaticoWindowsUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RimuoviAUOptions();
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaDownloadAutomaticoWindowsUpdate", false);
            }
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Update Prodotti Microsoft"))
            {
                SetCheckboxState("AbilitaUpdateProdottiMicrosoft", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = "(New-Object -ComObject Microsoft.Update.ServiceManager).AddService2(\"7971f918-a847-4430-9279-4a52d1efe18d\", 7, \"\")",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        Verb = "runus"
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
                SetCheckboxState("AbilitaUpdateProdottiMicrosoft", false);
            }
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Download Driver Windows Update"))
            {
                SetCheckboxState("AbilitaDownloadDriverWindowsUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RimuoviDriverUpdate();
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaDownloadDriverWindowsUpdate", false);
            }
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Riavvio Automatico Windows Update"))
            {
                SetCheckboxState("AbilitaRiavvioAutomaticoWindowsUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RimuoviRiavvioAutomatico();
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaRiavvioAutomaticoWindowsUpdate", false);
            }
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Notifiche Update"))
            {
                SetCheckboxState("AbilitaNotificheUpdate", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaNotificheUpdate(true);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                SetCheckboxState("AbilitaNotificheUpdate", false);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar2.Value = Math.Min(e.ProgressPercentage, progressBar2.MaxValue);
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

        private void btnSuggeriti_Click(object sender, EventArgs e)
        {
            var daDisabilitare = new List<string>
    {
        "Disabilita Riavvio Automatico Windows Update"
    };
            for (int i = 0; i < DisabilitaUpdate.Items.Count; i++)
                DisabilitaUpdate.SetItemChecked(i, false);
            foreach (string nome in daDisabilitare)
            {
                int index = DisabilitaUpdate.Items.IndexOf(nome);
                if (index != -1)
                    DisabilitaUpdate.SetItemChecked(index, true);
            }
        }

        private void DisabilitaUpdate_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string itemName = DisabilitaUpdate.Items[e.Index].ToString();
                string abilitaName = itemName.Replace("Disabilita", "Abilita");

                int index = AbilitaUpdate.Items.IndexOf(abilitaName);
                if (index >= 0)
                {
                    AbilitaUpdate.ItemCheck -= AbilitaUpdate_ItemCheck;
                    AbilitaUpdate.SetItemChecked(index, false);
                    AbilitaUpdate.ItemCheck += AbilitaUpdate_ItemCheck;
                }
            }
        }

        private void AbilitaUpdate_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string itemName = AbilitaUpdate.Items[e.Index].ToString();
                string disabilitaName = itemName.Replace("Abilita", "Disabilita");

                int index = DisabilitaUpdate.Items.IndexOf(disabilitaName);
                if (index >= 0)
                {
                    DisabilitaUpdate.ItemCheck -= DisabilitaUpdate_ItemCheck;
                    DisabilitaUpdate.SetItemChecked(index, false);
                    DisabilitaUpdate.ItemCheck += DisabilitaUpdate_ItemCheck;
                }
            }
        }

        private void DisabilitaUpdate_MouseDown(object sender, MouseEventArgs e)
        {
            int index = DisabilitaUpdate.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                DisabilitaUpdate.SetItemChecked(index, !DisabilitaUpdate.GetItemChecked(index));
            }
            DisabilitaUpdate.ClearSelected();
        }

        private void AbilitaUpdate_MouseDown(object sender, MouseEventArgs e)
        {
            int index = AbilitaUpdate.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                AbilitaUpdate.SetItemChecked(index, !AbilitaUpdate.GetItemChecked(index));
            }
            AbilitaUpdate.ClearSelected();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < AbilitaUpdate.Items.Count; i++)
            {
                AbilitaUpdate.SetItemChecked(i, false);
            }
            for (int i = 0; i < DisabilitaUpdate.Items.Count; i++)
            {
                DisabilitaUpdate.SetItemChecked(i, false);
            }
        }
    }
}
