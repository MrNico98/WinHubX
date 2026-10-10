using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormUpdate : Form, IImportedSettingsForm
    {
        private static bool IsExpectedUpdateOperationFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or InvalidOperationException
                or OperationCanceledException
                or TimeoutException
                or ArgumentException;

        private sealed record UpdateSelection(HashSet<string> Disable, HashSet<string> Enable);

        private readonly Form1 form1;
        private FormSettaggi formSettaggi;
        private int totalSteps = 0;
        private int tIndex = -1;
        private ElevatedRegistryMutationBatch? pendingRegistryMutations;
        public FormUpdate(FormSettaggi formSettaggi, Form1 form1)
        {
            LanguageManager.LoadTranslations();
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
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

        private void checkedListBox1_MouseMove(object? sender, MouseEventArgs e)
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

        private void checkedListBox2_MouseMove(object? sender, MouseEventArgs e)
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
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey("Software\\WinHubX"))
            {
                if (key != null)
                {
                    object? value = key.GetValue(itemName);
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
            var selection = new UpdateSelection(
                DisabilitaUpdate.CheckedItems.Cast<string>().ToHashSet(StringComparer.Ordinal),
                AbilitaUpdate.CheckedItems.Cast<string>().ToHashSet(StringComparer.Ordinal));
            totalSteps = selection.Disable.Count + selection.Enable.Count;
            if (totalSteps == 0 || backgroundWorker1.IsBusy)
            {
                return;
            }

            progressBar2.MaxValue = totalSteps;
            progressBar2.Value = 0;
            backgroundWorker1.RunWorkerAsync(selection);
        }

        Task IImportedSettingsForm.ApplyImportedSettingsAsync() =>
            ImportedSettingsWorker.RunAsync(
                backgroundWorker1,
                () => btnAvviaSelezionatiUpda_Click(btnAvviaSelezionatiVerdi, EventArgs.Empty));

        private void btnUpdateEssential_Click(object sender, EventArgs e)
        {
            try
            {
                pendingRegistryMutations = new ElevatedRegistryMutationBatch();
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
                ApplyPendingRegistryMutations();

                string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

                _ = MessageBox.Show(
                    messaggio,
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
            {
                ShowOperationError(ex);
            }
        }

        private void UpdateRegistry(string path, string name, int value, bool is64Bit)
        {
            RegistryView view = is64Bit ? RegistryView.Registry64 : RegistryView.Registry32;
            GetPendingRegistryMutations().SetValue(RegistryHive.LocalMachine,
                GetLocalMachineSubKeyPath(path), name, value, RegistryValueKind.DWord, view);
        }

        private ElevatedRegistryMutationBatch GetPendingRegistryMutations() =>
            pendingRegistryMutations ??= new ElevatedRegistryMutationBatch();

        private static string GetLocalMachineSubKeyPath(string path)
        {
            const string prefix = "HKLM\\";
            if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Il percorso deve appartenere a HKEY_LOCAL_MACHINE.", nameof(path));
            return path[prefix.Length..];
        }

        private void ApplyPendingRegistryMutations(string? additionalScript = null)
        {
            if (pendingRegistryMutations is null || pendingRegistryMutations.Count == 0)
                return;

            string script = pendingRegistryMutations.BuildCommand();
            if (!string.IsNullOrWhiteSpace(additionalScript))
                script += Environment.NewLine + additionalScript;
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-EncodedCommand");
            startInfo.ArgumentList.Add(Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script)));
            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare il processo elevato per Windows Update.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Le modifiche di Windows Update sono terminate con codice {process.ExitCode}.");
            pendingRegistryMutations = null;
        }

        private static void RunElevatedPowerShellCommand(string command, string operationDescription)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-Command");
            startInfo.ArgumentList.Add(command);

            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare il processo PowerShell.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"{operationDescription} terminata con codice {process.ExitCode}.");
        }

        private void btnResetUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                pendingRegistryMutations = new ElevatedRegistryMutationBatch();
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
                var registryRemovals = new (string Path, string Name)[]
                {
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\Device Metadata", "PreventDeviceMetadataFromNetwork"),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontPromptForWindowsUpdate"),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontSearchWindowsUpdate"),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DriverUpdateWizardWuSearchEnabled"),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate"),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers"),
            (@"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUPowerManagement"),
            (@"HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "BranchReadinessLevel"),
            (@"HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "DeferFeatureUpdatesPeriodInDays"),
            (@"HKLM\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "DeferQualityUpdatesPeriodInDays")
                };

                foreach (var removal in registryRemovals)
                {
                    RemoveRegistryValue(removal.Path, removal.Name, true);
                    RemoveRegistryValue(removal.Path, removal.Name, false);
                }
                ApplyPendingRegistryMutations("Set-Service -Name BITS,wuauserv -StartupType Automatic -ErrorAction Stop");
                string messaggio = LanguageManager.GetTranslation("Global", "modifichesuccesso");

                _ = MessageBox.Show(
                    messaggio,
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
            {
                pendingRegistryMutations = null;
                ShowOperationError(ex);
            }
        }

        private void RemoveRegistryValue(string path, string name, bool is64Bit)
        {
            const string localMachinePrefix = "HKLM\\";
            if (!path.StartsWith(localMachinePrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Il percorso deve appartenere a HKEY_LOCAL_MACHINE.", nameof(path));
            }

            RegistryView view = is64Bit ? RegistryView.Registry64 : RegistryView.Registry32;
            GetPendingRegistryMutations().DeleteValue(RegistryHive.LocalMachine,
                path[localMachinePrefix.Length..], name, view);
        }

        private void ModificaChiaveRegistro(RegistryView view)
        {
            var batch = GetPendingRegistryMutations();
            batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Device Metadata", "PreventDeviceMetadataFromNetwork", 1, RegistryValueKind.DWord, view);
            batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "SearchOrderConfig", 0, RegistryValueKind.DWord, view);
            batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", 1, RegistryValueKind.DWord, view);
        }

        private void ShowOperationError(Exception exception)
        {
            _ = MessageBox.Show(
                $"L'operazione non è stata completata. {exception.GetBaseException().Message}",
                "WinHubX",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ModificaDownloadAutomatico(RegistryView view)
        {
            GetPendingRegistryMutations().SetValue(RegistryHive.LocalMachine,
                @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUOptions", 2, RegistryValueKind.DWord, view);
        }

        private void RimuoviDriverUpdate()
        {
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                var batch = GetPendingRegistryMutations();
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Device Metadata", "PreventDeviceMetadataFromNetwork", view);
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "SearchOrderConfig", view);
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontPromptForWindowsUpdate", view);
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DontSearchWindowsUpdate", view);
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DriverSearching", "DriverUpdateWizardWuSearchEnabled", view);
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", view);
            }
        }

        private void RimuoviRiavvioAutomatico()
        {
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                var batch = GetPendingRegistryMutations();
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", view);
                batch.DeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUPowerManagement", view);
            }
        }
        private void ModificaNotificheUpdate(bool enable)
        {
            string windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string musNotification = Path.Join(windowsDirectory, "System32", "MusNotification.exe");
            string musNotificationUx = Path.Join(windowsDirectory, "System32", "MusNotificationUx.exe");
            string escapedNotification = EscapePowerShellLiteral(musNotification);
            string escapedNotificationUx = EscapePowerShellLiteral(musNotificationUx);
            string accessControlCommand = enable
                ? "& $icacls $target /remove:d Everyone"
                : "& $icacls $target /deny 'Everyone:(X)'";
            string script = $"$ErrorActionPreference = 'Stop'; $takeown = Join-Path $env:SystemRoot 'System32\\takeown.exe'; $icacls = Join-Path $env:SystemRoot 'System32\\icacls.exe'; $targets = @('{escapedNotification}', '{escapedNotificationUx}'); foreach ($target in $targets) {{ & $takeown /F $target /A; if ($LASTEXITCODE -ne 0) {{ exit $LASTEXITCODE }}; {accessControlCommand}; if ($LASTEXITCODE -ne 0) {{ exit $LASTEXITCODE }} }}";
            string encodedScript = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));

            var startInfo = new ProcessStartInfo()
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-EncodedCommand");
            startInfo.ArgumentList.Add(encodedScript);

            using (var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare il processo di sistema."))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"La modifica dei permessi delle notifiche Windows Update è terminata con codice {process.ExitCode}.");
            }
        }

        private static string EscapePowerShellLiteral(string value) => value.Replace("'", "''", StringComparison.Ordinal);

        private void RimuoviAUOptions()
        {
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                GetPendingRegistryMutations().DeleteValue(RegistryHive.LocalMachine,
                    @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUOptions", view);
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument is not UpdateSelection selection)
            {
                throw new InvalidOperationException("Selezione delle impostazioni Windows Update non valida.");
            }

            int currentStep = 0;
            var failures = new List<string>();
            pendingRegistryMutations = new ElevatedRegistryMutationBatch();
            int failuresBeforeCurrentOperation = 0;
            if (selection.Disable.Contains("Disabilita Download Automatico Windows Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaDownloadAutomatico(RegistryView.Registry32);
                    ModificaDownloadAutomatico(RegistryView.Registry64);
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Download automatico: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("DisabilitaDownloadAutomaticoWindowsUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaDownloadAutomaticoWindowsUpdate", false);
            }
            if (selection.Disable.Contains("Disabilita Update Prodotti Microsoft"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RunElevatedPowerShellCommand(
                        "$ErrorActionPreference='Stop'; $manager=New-Object -ComObject Microsoft.Update.ServiceManager; if ($manager.Services | Where-Object { $_.ServiceID -eq '7971f918-a847-4430-9279-4a52d1efe18d' }) { $manager.RemoveService('7971f918-a847-4430-9279-4a52d1efe18d') }",
                        "Rimozione del servizio Microsoft Update");
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Microsoft Update: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("DisabilitaUpdateProdottiMicrosoft", true);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaUpdateProdottiMicrosoft", false);
            }
            if (selection.Disable.Contains("Disabilita Download Driver Windows Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaChiaveRegistro(RegistryView.Registry32);
                    ModificaChiaveRegistro(RegistryView.Registry64);
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Driver Windows Update: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("DisabilitaDownloadDriverWindowsUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaDownloadDriverWindowsUpdate", false);
            }
            if (selection.Disable.Contains("Disabilita Riavvio Automatico Windows Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var batch = GetPendingRegistryMutations();
                    foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
                    {
                        batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", 1, RegistryValueKind.DWord, view);
                        batch.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUPowerManagement", 0, RegistryValueKind.DWord, view);
                    }
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Riavvio automatico: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("DisabilitaRiavvioAutomaticoWindowsUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaRiavvioAutomaticoWindowsUpdate", false);
            }
            if (selection.Disable.Contains("Disabilita Notifiche Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaNotificheUpdate(false);
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Notifiche Windows Update: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("DisabilitaNotificheUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaNotificheUpdate", false);
            }
            if (selection.Enable.Contains("Abilita Download Automatico Windows Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RimuoviAUOptions();
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Ripristino download automatico: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("AbilitaDownloadAutomaticoWindowsUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("AbilitaDownloadAutomaticoWindowsUpdate", false);
            }
            if (selection.Enable.Contains("Abilita Update Prodotti Microsoft"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RunElevatedPowerShellCommand(
                        "(New-Object -ComObject Microsoft.Update.ServiceManager).AddService2('7971f918-a847-4430-9279-4a52d1efe18d', 7, '')",
                        "Attivazione del servizio Microsoft Update");
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Servizio Microsoft Update: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("AbilitaUpdateProdottiMicrosoft", true);
                }
            }
            else
            {
                SetCheckboxState("AbilitaUpdateProdottiMicrosoft", false);
            }
            if (selection.Enable.Contains("Abilita Download Driver Windows Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RimuoviDriverUpdate();
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Ripristino driver Windows Update: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("AbilitaDownloadDriverWindowsUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("AbilitaDownloadDriverWindowsUpdate", false);
            }
            if (selection.Enable.Contains("Abilita Riavvio Automatico Windows Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    RimuoviRiavvioAutomatico();
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Ripristino riavvio automatico: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("AbilitaRiavvioAutomaticoWindowsUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("AbilitaRiavvioAutomaticoWindowsUpdate", false);
            }
            if (selection.Enable.Contains("Abilita Notifiche Update"))
            {
                failuresBeforeCurrentOperation = failures.Count;
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    ModificaNotificheUpdate(true);
                }
                catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
                {
                    failures.Add($"Ripristino notifiche Windows Update: {ex.GetBaseException().Message}");
                }
                if (failures.Count == failuresBeforeCurrentOperation)
                {
                    SetCheckboxState("AbilitaNotificheUpdate", true);
                }
            }
            else
            {
                SetCheckboxState("AbilitaNotificheUpdate", false);
            }

            try
            {
                ApplyPendingRegistryMutations();
            }
            catch (Exception ex) when (IsExpectedUpdateOperationFailure(ex))
            {
                failures.Add($"Modifiche Registro Windows Update (UAC): {ex.GetBaseException().Message}");
            }

            e.Result = failures;
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar2.Value = Math.Min(e.ProgressPercentage, progressBar2.MaxValue);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error is not null)
            {
                ShowOperationError(e.Error);
                return;
            }

            if (e.Cancelled)
            {
                _ = MessageBox.Show(
                    "Operazione annullata.",
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (e.Result is List<string> failures && failures.Count > 0)
            {
                string details = string.Join(Environment.NewLine, failures.Take(5));
                string remaining = failures.Count > 5
                    ? $"{Environment.NewLine}Altri errori: {failures.Count - 5}."
                    : string.Empty;
                _ = MessageBox.Show(
                    $"Alcune impostazioni non sono state applicate:{Environment.NewLine}{details}{remaining}",
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

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
            foreach (int index in daDisabilitare.Select(nome => DisabilitaUpdate.Items.IndexOf(nome)).Where(static index => index >= 0))
            {
                DisabilitaUpdate.SetItemChecked(index, true);
            }
        }

        private void DisabilitaUpdate_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string itemName = DisabilitaUpdate.Items[e.Index]?.ToString() ?? string.Empty;
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

        private void AbilitaUpdate_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string itemName = AbilitaUpdate.Items[e.Index]?.ToString() ?? string.Empty;
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
