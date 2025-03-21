﻿using Microsoft.Win32;
using System.Diagnostics;
using WinHubX.Forms.Base;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormUpdate : Form
    {
        private Form1 form1;
        private FormSettaggi formSettaggi;
        public FormUpdate(FormSettaggi formSettaggi, Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
            LoadCheckboxStates();
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
            int index = DisabilitaUpdate.Items.IndexOf("Disabilita Download Automatico Windows Update");
            if (index != -1)
            {
                DisabilitaUpdate.SetItemChecked(index, GetCheckboxState("DisabilitaDownloadAutomaticoWindowsUpdate"));
            }
            index = DisabilitaUpdate.Items.IndexOf("Disabilita Update Prodotti Microsoft");
            if (index != -1)
            {
                DisabilitaUpdate.SetItemChecked(index, GetCheckboxState("DisabilitaUpdateProdottiMicrosoft"));
            }
            index = DisabilitaUpdate.Items.IndexOf("Disabilita Download Driver Windows Update");
            if (index != -1)
            {
                DisabilitaUpdate.SetItemChecked(index, GetCheckboxState("DisabilitaDownloadDriverWindowsUpdate"));
            }
            index = DisabilitaUpdate.Items.IndexOf("Disabilita Riavvio Automatico Windows Update");
            if (index != -1)
            {
                DisabilitaUpdate.SetItemChecked(index, GetCheckboxState("DisabilitaRiavvioAutomaticoWindowsUpdate"));
            }
            index = DisabilitaUpdate.Items.IndexOf("Disabilita Notifiche Update");
            if (index != -1)
            {
                DisabilitaUpdate.SetItemChecked(index, GetCheckboxState("DisabilitaNotificheUpdate"));
            }
            index = AbilitaUpdate.Items.IndexOf("Abilita Download Automatico Windows Update");
            if (index != -1)
            {
                AbilitaUpdate.SetItemChecked(index, GetCheckboxState("AbilitaDownloadAutomaticoWindowsUpdate"));
            }
            index = AbilitaUpdate.Items.IndexOf("Abilita Update Prodotti Microsoft");
            if (index != -1)
            {
                AbilitaUpdate.SetItemChecked(index, GetCheckboxState("AbilitaUpdateProdottiMicrosoft"));
            }
            index = AbilitaUpdate.Items.IndexOf("Abilita Download Driver Windows Update");
            if (index != -1)
            {
                AbilitaUpdate.SetItemChecked(index, GetCheckboxState("AbilitaDownloadDriverWindowsUpdate"));
            }
            index = AbilitaUpdate.Items.IndexOf("Abilita Riavvio Automatico Windows Update");
            if (index != -1)
            {
                AbilitaUpdate.SetItemChecked(index, GetCheckboxState("AbilitaRiavvioAutomaticoWindowsUpdate"));
            }
            index = AbilitaUpdate.Items.IndexOf("Abilita Notifiche Update");
            if (index != -1)
            {
                AbilitaUpdate.SetItemChecked(index, GetCheckboxState("AbilitaNotificheUpdate"));
            }
        }

        private void btnAvviaSelezionatiUpda_Click(object sender, EventArgs e)
        {
            if (DisabilitaUpdate.CheckedItems.Contains("Disabilita Download Automatico Windows Update"))
            {
                SetCheckboxState("DisabilitaDownloadAutomaticoWindowsUpdate", true);
                try
                {
                    // Modifica per entrambe le architetture
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
                try
                {
                    // Modifiche per entrambe le architetture
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

            // Modifica per disabilitare il riavvio automatico di Windows Update
            if (DisabilitaUpdate.CheckedItems.Contains("Disabilita Riavvio Automatico Windows Update"))
            {
                SetCheckboxState("DisabilitaRiavvioAutomaticoWindowsUpdate", true);
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

            // Abilita Download Automatico Windows Update
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Download Automatico Windows Update"))
            {
                SetCheckboxState("AbilitaDownloadAutomaticoWindowsUpdate", true);
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

            // Abilita Riavvio Automatico Windows Update
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Riavvio Automatico Windows Update"))
            {
                SetCheckboxState("AbilitaRiavvioAutomaticoWindowsUpdate", true);
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

            // Abilita Notifiche Update
            if (AbilitaUpdate.CheckedItems.Contains("Abilita Notifiche Update"))
            {
                SetCheckboxState("AbilitaNotificheUpdate", true);
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
            MessageBox.Show("Modifiche apportate con successo", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdateEssential_Click(object sender, EventArgs e)
        {
            try
            {
                // Elenco delle chiavi da modificare
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

                // Aggiorna il registro a 64 bit
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, true);
                }

                // Aggiorna il registro a 32 bit
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, false);
                }

                MessageBox.Show("Modifiche apportate con successo", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    throw new Exception($"Failed to update registry: {error}");
                }
            }
        }

        private void btnResetUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Chiavi e valori da ripristinare
                var registryChanges = new (string Path, string Name, int Value)[]
                {
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", 0),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUOptions", 3),
            (@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\DeliveryOptimization\Config", "DODownloadMode", 1)
                };

                // Aggiorna il registro a 64 bit
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, true);
                }

                // Aggiorna il registro a 32 bit
                foreach (var change in registryChanges)
                {
                    UpdateRegistry(change.Path, change.Name, change.Value, false);
                }

                // Avvia i servizi richiesti
                StartService("BITS");
                StartService("wuauserv");

                // Rimuovi le proprietà specificate dal registro
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

                // Rimuovi le proprietà dal registro a 64 bit e a 32 bit
                foreach (var removal in registryRemovals)
                {
                    RemoveRegistryValue(removal, true);
                    RemoveRegistryValue(removal, false);
                }

                MessageBox.Show("Modifiche apportate con successo", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    throw new Exception($"Failed to start service {serviceName}: {error}");
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
                    key?.SetValue("AUOptions", 2, RegistryValueKind.DWord); // Imposta il valore per disabilitare il download automatico
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

        // Funzione per rimuovere il riavvio automatico
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

        // Funzione per rimuovere AUOptions
        private void RimuoviAUOptions()
        {
            using (var key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var key32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                key64.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", false);
                key32.DeleteValue(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", false);
            }
        }
    }
}
