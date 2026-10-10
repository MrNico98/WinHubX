using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormDefender : Form, IImportedSettingsForm
    {
        private static bool IsExpectedDefenderOperationFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or InvalidOperationException
                or OperationCanceledException
                or TimeoutException
                or ArgumentException;

        private sealed record DefenderSelection(HashSet<string> Disable, HashSet<string> Enable);

        private readonly Form1 form1;
        private readonly FormSettaggi formSettaggi;
        private int totalSteps = 0;
        private int tIndex = -1;
        private ElevatedRegistryMutationBatch? pendingRegistryMutations;
        private ElevatedRegistryAclMutationBatch? pendingRegistryAclMutations;
        public FormDefender(FormSettaggi formSettaggi, Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
            LoadCheckboxStates();
            DisabilitaDefender.MouseMove += new MouseEventHandler(checkedListBox1_MouseMove);
            AbilitaDefender.MouseMove += new MouseEventHandler(checkedListBox2_MouseMove);
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
            btnRiprisitinoDefenderVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Ripristina",
                "en" => "  Restore",
                _ => btnRiprisitinoDefenderVerdi.Content
            };
            btnProtezioneMinimaVerdi.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Protezione minima",
                "en" => "  Minimal protection",
                _ => btnProtezioneMinimaVerdi.Content
            };
        }

        private void checkedListBox1_MouseMove(object? sender, MouseEventArgs e)
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

        private void checkedListBox2_MouseMove(object? sender, MouseEventArgs e)
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


        private void SetCheckboxState(string itemName, bool isChecked)
        {
            using (RegistryKey? key = Registry.CurrentUser.CreateSubKey("Software\\WinHubX"))
            {
                key?.SetValue(itemName, isChecked ? 1 : 0, RegistryValueKind.DWord);
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
        (DisabilitaDefender, "Disabilita Controllo Accesso Cartella", "DisabilitaControlloAccessoCartella"),
        (DisabilitaDefender, "Disabilita Isolamento Core", "DisabilitaIsolamentoCore"),
        (DisabilitaDefender, "Disabilita Applicazione Defender Guard", "DisabilitaApplicazioneDefenderGuard"),
        (DisabilitaDefender, "Disabilita Protezione Account Warning", "DisabilitaProtezioneAccountWarning"),
        (DisabilitaDefender, "Disabilita Blocco Download Files", "DisabilitaBloccoDownloadFiles"),
        (DisabilitaDefender, "Disabilita Windows Script Host", "DisabilitaWindowsScriptHost"),
        (DisabilitaDefender, "Disabilita .NET Strong Cryptography", "DisabilitaNETStrongCryptography"),
        (DisabilitaDefender, "Livello Minimo UAC", "LivelloMinimoUAC"),
        (DisabilitaDefender, "Disabilita Implicit Administrative Sheres", "DisabilitaImplicitAdministrativeSheres"),
        (DisabilitaDefender, "Disabilita Windows Firewall", "DisabilitaWindowsFirewall"),
        (DisabilitaDefender, "Disabilita Windows Defender CLoud", "DisabilitaWindowsDefenderCLoud"),
        (DisabilitaDefender, "Disabilita Windows Defender SysTray", "DisabilitaWindowsDefenderSysTray"),
        (DisabilitaDefender, "Disabilita Windows Defender Services", "DisabilitaWindowsDefenderServices"),
        (AbilitaDefender, "Abilita Controllo Accesso Cartella", "AbilitaControlloAccessoCartella"),
        (AbilitaDefender, "Abilita Isolamento Core", "AbilitaIsolamentoCore"),
        (AbilitaDefender, "Abilita Applicazione Defender Guard", "AbilitaApplicazioneDefenderGuard"),
        (AbilitaDefender, "Abilita Protezione Account Warning", "AbilitaProtezioneAccountWarning"),
        (AbilitaDefender, "Abilita Blocco Download Files", "AbilitaBloccoDownloadFiles"),
        (AbilitaDefender, "Abilita Windows Script Host", "AbilitaWindowsScriptHost"),
        (AbilitaDefender, "Abilita .NET Strong Cryptography", "AbilitaNETStrongCryptography"),
        (AbilitaDefender, "Livello Massimo UAC", "LivelloMassimoUAC"),
        (AbilitaDefender, "Abilita Implicit Administrative Sheres", "AbilitaImplicitAdministrativeSheres"),
        (AbilitaDefender, "Abilita Windows Firewall", "AbilitaWindowsFirewall"),
        (AbilitaDefender, "Abilita Windows Defender CLoud", "AbilitaWindowsDefenderCLoud"),
        (AbilitaDefender, "Abilita Windows Defender SysTray", "AbilitaWindowsDefenderSysTray"),
        (AbilitaDefender, "Abilita Windows Defender Services", "AbilitaWindowsDefenderServices"),
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


        private void btnAvviaSelezionatiDef_Click(object? sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                return;
            }

            var selection = new DefenderSelection(
                DisabilitaDefender.CheckedItems.Cast<string>().ToHashSet(StringComparer.Ordinal),
                AbilitaDefender.CheckedItems.Cast<string>().ToHashSet(StringComparer.Ordinal));
            totalSteps = selection.Disable.Count + selection.Enable.Count;
            if (totalSteps == 0)
            {
                return;
            }

            progressBar1.MaxValue = totalSteps;
            progressBar1.Value = 0;
            backgroundWorker1.RunWorkerAsync(selection);
        }

        Task IImportedSettingsForm.ApplyImportedSettingsAsync() =>
            ImportedSettingsWorker.RunAsync(
                backgroundWorker1,
                () => btnAvviaSelezionatiDef_Click(btnAvviaSelezionatiVerdi, EventArgs.Empty));

        private void btnRipristinaDefender_Click(object? sender, EventArgs e)
        {
            try
            {
                pendingRegistryMutations = new ElevatedRegistryMutationBatch();
                pendingRegistryAclMutations = new ElevatedRegistryAclMutationBatch();
                SetMpPreference("EnableControlledFolderAccess", true);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", 1);
                SetDwordRegistryValue(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", 1, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\QualityCompat", "cadca5fe-87d3-4b96-b7fb-a231484277cc", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 1);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1);
                SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 1);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2);
                ApplyPendingRegistryMutations();
            }
            catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
            {
                pendingRegistryMutations = null;
                MessageBox.Show("Si è verificato un errore durante il ripristino. Controlla i permessi o il registro eventi.",
                    "WinHubX - Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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


        private void DeleteRegistryValue(string keyPath, string valueName, RegistryView registryView)
        {
            GetPendingRegistryMutations().DeleteValue(RegistryHive.LocalMachine, keyPath, valueName, registryView);
        }

        private void DeleteRegistryValue(string keyPath, string valueName)
        {
            DeleteRegistryValue(keyPath, valueName, RegistryView.Registry64);
            DeleteRegistryValue(keyPath, valueName, RegistryView.Registry32);
        }

        private void SetDwordRegistryValue(string keyPath, string valueName, int value, RegistryView registryView)
        {
            GetPendingRegistryMutations().SetValue(RegistryHive.LocalMachine, keyPath, valueName, value,
                RegistryValueKind.DWord, registryView);
        }

        private void SetDwordRegistryValue(string keyPath, string valueName, int value)
        {
            SetDwordRegistryValue(keyPath, valueName, value, RegistryView.Registry64);
            SetDwordRegistryValue(keyPath, valueName, value, RegistryView.Registry32);
        }

        void GrantRegistryTakeOwnershipRight(string keyPath, RegistryView registryView)
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            string userSid = identity.User?.Value
                ?? throw new InvalidOperationException("Impossibile determinare l'identità Windows corrente.");
            GetPendingRegistryAclMutations().AddLocalMachineTakeOwnership(keyPath, registryView, userSid);
        }
        private void SetStringRegistryValue(string keyPath, string name, string value, RegistryView view)
        {
            GetPendingRegistryMutations().SetValue(RegistryHive.LocalMachine, keyPath, name, value,
                RegistryValueKind.ExpandString, view);
        }

        private ElevatedRegistryMutationBatch GetPendingRegistryMutations() =>
            pendingRegistryMutations ??= new ElevatedRegistryMutationBatch();

        private ElevatedRegistryAclMutationBatch GetPendingRegistryAclMutations() =>
            pendingRegistryAclMutations ??= new ElevatedRegistryAclMutationBatch();

        private void ApplyPendingRegistryMutations()
        {
            if ((pendingRegistryMutations is null || pendingRegistryMutations.Count == 0)
                && (pendingRegistryAclMutations is null || pendingRegistryAclMutations.Count == 0))
                return;

            string script = pendingRegistryMutations is { Count: > 0 }
                ? pendingRegistryMutations.BuildCommand()
                : "$ErrorActionPreference = 'Stop'";
            if (pendingRegistryAclMutations is { Count: > 0 })
                script += Environment.NewLine + pendingRegistryAclMutations.BuildCommand();

            string encodedScript = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-EncodedCommand");
            startInfo.ArgumentList.Add(encodedScript);
            using System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare le modifiche Defender con privilegi elevati.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Le modifiche del Registro Defender sono terminate con codice {process.ExitCode}.");
            pendingRegistryMutations = null;
            pendingRegistryAclMutations = null;
        }

        private void SetMpPreference(string preference, bool enabled)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                Verb = "runas",
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            psi.ArgumentList.Add("-NoProfile");
            psi.ArgumentList.Add("-NonInteractive");
            psi.ArgumentList.Add("-Command");
            psi.ArgumentList.Add($"$ErrorActionPreference = 'Stop'; Set-MpPreference -{preference} {(enabled ? "Enabled" : "Disabled")}");
            using var process = System.Diagnostics.Process.Start(psi)
                ?? throw new InvalidOperationException("Impossibile avviare Set-MpPreference.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Set-MpPreference -{preference} è terminato con codice {process.ExitCode}.");
        }

        private void backgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument is not DefenderSelection selection)
            {
                throw new InvalidOperationException("Selezione delle impostazioni Defender non valida.");
            }

            int currentStep = 0;
            var failures = new List<string>();
            pendingRegistryMutations = new ElevatedRegistryMutationBatch();
            pendingRegistryAclMutations = new ElevatedRegistryAclMutationBatch();
            if (selection.Disable.Contains("Disabilita Controllo Accesso Cartella"))
            {
                SetCheckboxState("DisabilitaControlloAccessoCartella", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetMpPreference("EnableControlledFolderAccess", true);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaControlloAccessoCartella", false);
            }
            if (selection.Disable.Contains("Disabilita Isolamento Core"))
            {
                SetCheckboxState("DisabilitaIsolamentoCore", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    _ = MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                SetCheckboxState("DisabilitaIsolamentoCore", false);
            }
            if (selection.Disable.Contains("Disabilita Applicazione Defender Guard"))
            {
                SetCheckboxState("DisabilitaApplicazioneDefernderGuard", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                        UseShellExecute = true,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        Verb = "runas"
                    };
                    startInfo.ArgumentList.Add("-NoProfile");
                    startInfo.ArgumentList.Add("-NonInteractive");
                    startInfo.ArgumentList.Add("-Command");
                    startInfo.ArgumentList.Add("Disable-WindowsOptionalFeature -Online -FeatureName 'Windows-Defender-ApplicationGuard' -NoRestart -WarningAction SilentlyContinue");

                    using (var process = System.Diagnostics.Process.Start(startInfo)
                        ?? throw new InvalidOperationException("Impossibile avviare la modifica di Windows Defender."))
                    {
                        process.WaitForExit();

                    }
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaApplicazioneDefernderGuard", false);
            }
            if (selection.Disable.Contains("Disabilita Protezione Account Warning"))
            {
                SetCheckboxState("DisabilitaProtezioneAccountWarning", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaProtezioneAccountWarning", false);
            }
            if (selection.Disable.Contains("Disabilita Blocco Download Files"))
            {
                SetCheckboxState("DisabilitaBloccoDownloadFiles", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaBloccoDownloadFiles", false);
            }
            if (selection.Disable.Contains("Disabilita Windows Script Host"))
            {
                SetCheckboxState("DisabilitaWindowsScriptHost", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsScriptHost", false);
            }
            if (selection.Disable.Contains("Disabilita .NET Strong Cryptography"))
            {
                SetCheckboxState("DisabilitaNETStrongCryptography", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaNETStrongCryptography", false);
            }
            if (selection.Disable.Contains("Livello Minimo UAC"))
            {
                SetCheckboxState("LivelloMinimoUAC", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("LivelloMinimoUAC", false);
            }
            if (selection.Disable.Contains("Disabilita Implicit Administrative Sheres"))
            {
                SetCheckboxState("DisabilitaImplicitAdministrativeSheres", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaImplicitAdministrativeSheres", false);
            }
            if (selection.Disable.Contains("Disabilita Windows Firewall"))
            {
                SetCheckboxState("DisabilitaWindowsFirewall", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\WindowsFirewall\StandardProfile", "EnableFirewall", 0);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsFirewall", false);
            }
            if (selection.Disable.Contains("Disabilita Windows Defender CLoud"))
            {
                SetCheckboxState("DisabilitaWindowsDefenderCLoud", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0);
                    SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsDefenderCLoud", false);
            }
            if (selection.Disable.Contains("Disabilita Windows Defender SysTray"))
            {
                SetCheckboxState("DisabilitaWindowsDefenderSysTray", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    string systrayKeyPath = @"SOFTWARE\Policies\Microsoft\Windows Defender Security Center\Systray";
                    SetDwordRegistryValue(systrayKeyPath, "HideSystray", 1);
                    var osVersion = Environment.OSVersion.Version;
                    if (osVersion.Build == 14393)
                    {
                        DeleteRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "WindowsDefender");
                    }
                    else if (osVersion.Build >= 15063)
                    {
                        DeleteRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "SecurityHealth");
                    }
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaWindowsDefenderSysTray", false);
            }
            if (selection.Disable.Contains("Disabilita Windows Defender Services"))
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
                        GrantRegistryTakeOwnershipRight(path, RegistryView.Registry64);
                        GrantRegistryTakeOwnershipRight(path, RegistryView.Registry32);
                    }
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            if (selection.Enable.Contains("Abilita Controllo Accesso Cartella"))
            {
                SetCheckboxState("AbilitaControlloAccessoCartella", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 1);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaControlloAccessoCartella", false);
            }
            if (selection.Enable.Contains("Abilita Isolamento Core"))
            {
                SetCheckboxState("AbilitaIsolamentoCore", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", 1);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaIsolamentoCore", false);
            }
            if (selection.Enable.Contains("Abilita Applicazione Defender Guard"))
            {
                SetCheckboxState("AbilitaApplicazioneDefenderGuard", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                        UseShellExecute = true,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        Verb = "runas"
                    };
                    startInfo.ArgumentList.Add("-NoProfile");
                    startInfo.ArgumentList.Add("-NonInteractive");
                    startInfo.ArgumentList.Add("-Command");
                    startInfo.ArgumentList.Add("Enable-WindowsOptionalFeature -Online -FeatureName 'Windows-Defender-ApplicationGuard' -NoRestart -WarningAction SilentlyContinue");

                    using (var process = System.Diagnostics.Process.Start(startInfo)
                        ?? throw new InvalidOperationException("Impossibile avviare la modifica di Windows Defender."))
                    {
                        process.WaitForExit();

                    }
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaApplicazioneDefenderGuard", false);
            }
            if (selection.Enable.Contains("Abilita Protezione Account Warning"))
            {
                SetCheckboxState("AbilitaProtezioneAccountWarning", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaProtezioneAccountWarning", false);
            }
            if (selection.Enable.Contains("Abilita Blocco Download Files"))
            {
                SetCheckboxState("AbilitaBloccoDownloadFiles", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaBloccoDownloadFiles", false);
            }
            if (selection.Enable.Contains("Abilita Windows Script Host"))
            {
                SetCheckboxState("AbilitaWindowsScriptHost", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsScriptHost", false);
            }
            if (selection.Enable.Contains("Abilita .NET Strong Cryptography"))
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
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaNETStrongCryptography", false);
            }
            if (selection.Enable.Contains("Livello Massimo UAC"))
            {
                SetCheckboxState("LivelloMassimoUAC", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5);

                    SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1);
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("LivelloMassimoUAC", false);
            }
            if (selection.Enable.Contains("Abilita Implicit Administrative Sheres"))
            {
                SetCheckboxState("AbilitaImplicitAdministrativeSheres", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaImplicitAdministrativeSheres", false);
            }
            if (selection.Enable.Contains("Abilita Windows Firewall"))
            {
                SetCheckboxState("AbilitaWindowsFirewall", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"SOFTWARE\Policies\Microsoft\WindowsFirewall\StandardProfile", "EnableFirewall");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsFirewall", false);
            }
            if (selection.Enable.Contains("Abilita Windows Defender CLoud"))
            {
                SetCheckboxState("AbilitaWindowsDefenderCLoud", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting");

                    DeleteRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent");
                }
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsDefenderCLoud", false);
            }
            if (selection.Enable.Contains("Abilita Windows Defender SysTray"))
            {
                SetCheckboxState("AbilitaWindowsDefenderSysTray", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender Security Center\Systray", "HideSystray");
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
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsDefenderSysTray", false);
            }
            if (selection.Enable.Contains("Abilita Windows Defender Services"))
            {
                SetCheckboxState("AbilitaWindowsDefenderServices", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
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
                catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
                {
                    failures.Add(ex.GetBaseException().Message);
                }
            }
            else
            {
                SetCheckboxState("AbilitaWindowsDefenderServices", false);
            }

            try
            {
                ApplyPendingRegistryMutations();
            }
            catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
            {
                failures.Add($"Modifiche Registro Defender (UAC): {ex.GetBaseException().Message}");
            }

            e.Result = failures;
        }

        private void btnProtezioneMinima_Click(object? sender, EventArgs e)
        {
            try
            {
                pendingRegistryMutations = new ElevatedRegistryMutationBatch();
                pendingRegistryAclMutations = new ElevatedRegistryAclMutationBatch();
                SetMpPreference("EnableControlledFolderAccess", false);
                DeleteRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", RegistryView.Registry64);
                DeleteRegistryValue(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry64);
                DeleteRegistryValue(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry32);
                DeleteRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\QualityCompat", "cadca5fe-87d3-4b96-b7fb-a231484277cc", RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1, RegistryView.Registry64);
                SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry64);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry64);
                DeleteRegistryValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled", RegistryView.Registry32);
                DeleteRegistryValue(@"SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry32);
                DeleteRegistryValue(@"SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto", RegistryView.Registry64);
                DeleteRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\QualityCompat", "cadca5fe-87d3-4b96-b7fb-a231484277cc", RegistryView.Registry32);
                SetDwordRegistryValue(@"Software\Microsoft\Windows Security Health\State", "AccountProtection_MicrosoftAccount_Disconnected", 1, RegistryView.Registry32);
                SetDwordRegistryValue(@"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SpynetReporting", 0, RegistryView.Registry32);
                SetDwordRegistryValue(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryView.Registry32);
                ApplyPendingRegistryMutations();
            }
            catch (Exception ex) when (IsExpectedDefenderOperationFailure(ex))
            {
                pendingRegistryMutations = null;
                _ = MessageBox.Show(
                    $"La protezione minima non è stata applicata completamente: {ex.GetBaseException().Message}",
                    "WinHubX - Errore",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void backgroundWorker1_ProgressChanged(object? sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = Math.Min(e.ProgressPercentage, progressBar1.MaxValue);
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            CompletionMessage completion = BackgroundWorkerCompletionMessageFactory.Create(
                e,
                "Alcune impostazioni Defender non sono state applicate:",
                LanguageManager.GetTranslation("Global", "modifichesuccesso"));
            MessageBoxIcon icon = completion.Severity switch
            {
                CompletionMessageSeverity.Error => MessageBoxIcon.Error,
                CompletionMessageSeverity.Warning => MessageBoxIcon.Warning,
                _ => MessageBoxIcon.Information
            };

            _ = MessageBox.Show(
                completion.Text,
                "WinHubX",
                MessageBoxButtons.OK,
                icon
            );
        }

        private void btnSuggeriti_Click(object? sender, EventArgs e)
        {
            var daDisabilitare = new List<string>
    {
        "Disabilita Controllo Accesso Cartella",
        "Disabilita Blocco Download Files",
        "Disabilita Implicit Administrative Sheres"
    };
            for (int i = 0; i < DisabilitaDefender.Items.Count; i++)
                DisabilitaDefender.SetItemChecked(i, false);

            foreach (int index in daDisabilitare.Select(nome => DisabilitaDefender.Items.IndexOf(nome)).Where(static index => index >= 0))
            {
                DisabilitaDefender.SetItemChecked(index, true);
            }
        }

        private void AbilitaDefender_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string itemName = AbilitaDefender.Items[e.Index]?.ToString() ?? string.Empty;
                string disabilitaName = itemName.Replace("Abilita", "Disabilita");

                int index = DisabilitaDefender.Items.IndexOf(disabilitaName);
                if (index >= 0)
                {
                    DisabilitaDefender.ItemCheck -= DisabilitaDefender_ItemCheck;
                    DisabilitaDefender.SetItemChecked(index, false);
                    DisabilitaDefender.ItemCheck += DisabilitaDefender_ItemCheck;
                }
            }
        }

        private void DisabilitaDefender_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            {
                if (e.NewValue == CheckState.Checked)
                {
                    string itemName = DisabilitaDefender.Items[e.Index]?.ToString() ?? string.Empty;
                    string abilitaName = itemName.Replace("Disabilita", "Abilita");

                    int index = AbilitaDefender.Items.IndexOf(abilitaName);
                    if (index >= 0)
                    {
                        AbilitaDefender.ItemCheck -= AbilitaDefender_ItemCheck;
                        AbilitaDefender.SetItemChecked(index, false);
                        AbilitaDefender.ItemCheck += AbilitaDefender_ItemCheck;
                    }
                }
            }
        }

        private void DisabilitaDefender_MouseDown(object? sender, MouseEventArgs e)
        {
            int index = DisabilitaDefender.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                DisabilitaDefender.SetItemChecked(index, !DisabilitaDefender.GetItemChecked(index));
            }
            DisabilitaDefender.ClearSelected();
        }

        private void AbilitaDefender_MouseDown(object? sender, MouseEventArgs e)
        {
            int index = AbilitaDefender.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                AbilitaDefender.SetItemChecked(index, !AbilitaDefender.GetItemChecked(index));
            }
            AbilitaDefender.ClearSelected();
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < AbilitaDefender.Items.Count; i++)
            {
                AbilitaDefender.SetItemChecked(i, false);
            }

            for (int i = 0; i < DisabilitaDefender.Items.Count; i++)
            {
                DisabilitaDefender.SetItemChecked(i, false);
            }
        }
    }
}

