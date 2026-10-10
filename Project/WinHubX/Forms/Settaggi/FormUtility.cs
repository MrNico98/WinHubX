using Microsoft.Win32;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Text.Json;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Settaggi
{
    public partial class FormUtility : Form, IImportedSettingsForm
    {
        private readonly Form1 form1;
        private readonly FormSettaggi formSettaggi;
        private readonly ConcurrentQueue<string> operationFailures = new();
        private ElevatedRegistryMutationBatch? elevatedRegistryMutations;
        private int tIndex = -1;
        private int totalSteps = 0;
        private sealed record UtilitySelection(HashSet<string> Disable, HashSet<string> Enable);

        private static HashSet<string> SnapshotCheckedItems(CheckedListBox list)
        {
            var selectedItems = new HashSet<string>(StringComparer.Ordinal);
            foreach (object item in list.CheckedItems)
            {
                if (item is string name)
                    selectedItems.Add(name);
            }

            return selectedItems;
        }

        private void RecordOperationFailure(Exception exception)
        {
            operationFailures.Enqueue(exception.GetBaseException().Message);
            Debug.WriteLine(exception);
        }

        private static bool IsExpectedSystemOperationFailure(Exception exception) =>
            exception is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Runtime.InteropServices.COMException
                or System.Management.ManagementException
                or InvalidOperationException
                or OperationCanceledException
                or TimeoutException
                or ArgumentException
                or FormatException
                or InvalidCastException
                or OverflowException;

        private string? GetOperationFailureSummary()
        {
            string[] failures = operationFailures.ToArray();
            if (failures.Length == 0)
                return null;

            int displayedFailureCount = Math.Min(failures.Length, 5);
            string details = string.Join(Environment.NewLine, failures, 0, displayedFailureCount);
            if (failures.Length > displayedFailureCount)
                details += Environment.NewLine + $"… e altri {failures.Length - displayedFailureCount} errori.";

            return $"Errori durante {failures.Length} operazioni:{Environment.NewLine}{details}";
        }
        public FormUtility(FormSettaggi formSettaggi, Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            this.formSettaggi = formSettaggi;
            LoadCheckboxStates();
            DisabilitaUtility.MouseMove += new MouseEventHandler(checkedListBox1_MouseMove);
            AbilitaUtility.MouseMove += new MouseEventHandler(checkedListBox2_MouseMove);
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
        }

        private void checkedListBox1_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = DisabilitaUtility.IndexFromPoint(e.Location);
            if (tIndex != index)
            {
                tIndex = index;
                if (tIndex > -1)
                {
                    string tooltipText = GetTooltipTextDisa(tIndex);
                    toolTip1.SetToolTip(DisabilitaUtility, tooltipText);
                }
            }
        }

        private void checkedListBox2_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = AbilitaUtility.IndexFromPoint(e.Location);
            if (tIndex != index)
            {
                tIndex = index;
                if (tIndex > -1)
                {
                    string tooltipText = GetTooltipTextAbil(tIndex);
                    toolTip1.SetToolTip(AbilitaUtility, tooltipText);
                }
            }
        }

        private string GetTooltipTextDisa(int index)
        {
            return LanguageManager.GetTranslation("FormUtility", $"tooltipDisa_{index}");
        }
        private string GetTooltipTextAbil(int index)
        {
            return LanguageManager.GetTranslation("FormUtility", $"tooltipAbil_{index}");
        }

        private static void SetSystemVolumeIndexing(bool enabled)
        {
            string systemDrive = Path.GetPathRoot(Environment.SystemDirectory)
                ?? throw new InvalidOperationException("Impossibile individuare l'unità di Windows.");
            systemDrive = systemDrive.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string escapedDrive = systemDrive.Replace("'", "''", StringComparison.Ordinal);
            string enabledLiteral = enabled ? "$true" : "$false";
            string script = "$ErrorActionPreference='Stop'; " +
                $"$volume=Get-CimInstance -ClassName Win32_Volume -Filter \"DriveLetter='{escapedDrive}'\"; " +
                "if ($null -eq $volume) { throw 'Volume di Windows non trovato.' }; " +
                $"$volume | Set-CimInstance -Property @{{IndexingEnabled={enabledLiteral}}} | Out-Null";

            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-Command");
            startInfo.ArgumentList.Add(script);

            using var process = System.Diagnostics.Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare la modifica dell'indicizzazione.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Modifica dell'indicizzazione terminata con codice {process.ExitCode}.");
        }

        private static void RunSystemTool(string executableName, params string[] arguments)
        {
            string executablePath = Path.Join(Environment.SystemDirectory, executableName);
            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            foreach (string argument in arguments)
                startInfo.ArgumentList.Add(argument);

            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException($"Impossibile avviare {executableName}.");
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new InvalidOperationException($"{executableName} è terminato con codice {process.ExitCode}.");
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
        private void btnSuggeriti_Click(object? sender, EventArgs e)
        {
            string path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                       "WinHubX\\Computer\\osehardware.json");

            string tipoDisk = "";
            int ramGB = 8;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                HardwareInfo? info = JsonSerializer.Deserialize<HardwareInfo>(json);
                tipoDisk = info?.Hardware?.Disk ?? "";
                if (int.TryParse(info?.Hardware?.RAM?.Replace(" GB", ""), out int parsedRam))
                    ramGB = parsedRam;
            }

            bool isHDD = tipoDisk.Contains("HDD", StringComparison.OrdinalIgnoreCase);
            bool isSSD = tipoDisk.Contains("SSD", StringComparison.OrdinalIgnoreCase) || tipoDisk.Contains("NVMe", StringComparison.OrdinalIgnoreCase);
            var daDisabilitare = new List<string>
    {
        "Disabilita Background App",
        "Disabilita Feedback",
        "Disabilita Advertising ID",
        "Disabilita Wifi Sense",
        "Disabilita News e Interessi",
        "Disabilita Mappe",
        "Disabilita UWP apps",
        "Disabilita Esperienze Personalizzate Microsoft",
        "Disabilita Ibernazione",
        "Disabilita Desktop Remoto"
    };
            if (ramGB < 8)
            {
                daDisabilitare.Add("Disabilita Superfetch");
                daDisabilitare.Add("Disabilita Storage Check");
            }
            if (isHDD)
            {
                daDisabilitare.Add("Disabilita Ottimizzazione FullScreen");
                daDisabilitare.Add("Disabilita Index File");
            }
            if (isSSD)
            {
                daDisabilitare.Add("Disabilita Superfetch");
                daDisabilitare.Add("Disabilita Index File");
                daDisabilitare.Add("Disabilita Avvio Rapido");
            }
            for (int i = 0; i < DisabilitaUtility.Items.Count; i++)
                DisabilitaUtility.SetItemChecked(i, false);
            foreach (int index in daDisabilitare.Select(nome => DisabilitaUtility.Items.IndexOf(nome)).Where(static index => index >= 0))
            {
                DisabilitaUtility.SetItemChecked(index, true);
            }

            var daAbilitare = new List<string>
    {
        "Abilita Ottimizzazione FullScreen",
        "Abilita Risparmio Energetico Personalizzato",
        "Abilita attivazione del Numlock in avvio"
    };

            if (isHDD)
            {
                daAbilitare.Add("Abilita Superfetch");
                daAbilitare.Add("Abilita Avvio Rapido");
            }
            if (isSSD)
            {
                daAbilitare.Add("Migliora uso SSD");
            }
            for (int i = 0; i < AbilitaUtility.Items.Count; i++)
                AbilitaUtility.SetItemChecked(i, false);
            foreach (int index in daAbilitare.Select(nome => AbilitaUtility.Items.IndexOf(nome)).Where(static index => index >= 0))
            {
                AbilitaUtility.SetItemChecked(index, true);
            }
        }

        private void LoadCheckboxStates()
        {
            var checkboxMappings = new (CheckedListBox box, string displayName, string regKey)[]
            {
        (DisabilitaUtility, "Disabilita Background App", "DisabilitaBackgroundApp"),
        (DisabilitaUtility, "Disabilita Feedback", "DisabilitaFeedback"),
        (DisabilitaUtility, "Disabilita Advertising ID", "DisabilitaAdvertisingID"),
        (DisabilitaUtility, "Disabilita Filtro Smart Screen", "DisabilitaFiltroSmartScreen"),
        (DisabilitaUtility, "Disabilita Wifi Sense", "DisabilitaWifiSense"),
        (DisabilitaUtility, "Disabilita Desktop Remoto", "DisabilitaDesktopRemoto"),
        (DisabilitaUtility, "Disabilita attivazione del Numlock in avvio", "DisabilitaattivazionedelNumlockinavvio"),
        (DisabilitaUtility, "Disabilita News e Interessi", "DisabilitaNewseInteressi"),
        (DisabilitaUtility, "Disabilita Index File", "DisabilitaIndexFile"),
        (DisabilitaUtility, "Disabilita Edge PDF", "DisabilitaEdgePDF"),
        (DisabilitaUtility, "Disabilita Mappe", "DisabilitaMappe"),
        (DisabilitaUtility, "Disabilita UWP apps", "DisabilitaUWPapps"),
        (DisabilitaUtility, "Disabilita Esperienze Personalizzate Microsoft", "DisabilitaEsperienzePersonalizzateMicrosoft"),
        (DisabilitaUtility, "Disabilita Storage Check", "DisabilitaStorageCheck"),
        (DisabilitaUtility, "Disabilita Superfetch", "DisabilitaSuperfetch"),
        (DisabilitaUtility, "Disabilita Ibernazione", "DisabilitaIbernazione"),
        (DisabilitaUtility, "Disabilita Ottimizzazione FullScreen", "DisabilitaOttimizzazioneFullScreen"),
        (DisabilitaUtility, "Disabilita Avvio Rapido", "DisabilitaAvvioRapido"),
        (DisabilitaUtility, "Normal Bandwidth", "NormalBandwidth"),
        (DisabilitaUtility, "Disabilita Migliora uso SSD", "DisabilitaMigliorausoSSD"),

        (AbilitaUtility, "All Bandwidth", "AllBandwidth"),
        (AbilitaUtility, "Abilita Storage Check", "AbilitaStorageCheck"),
        (AbilitaUtility, "Abilita Superfetch", "AbilitaSuperfetch"),
        (AbilitaUtility, "Abilita Ibernazione", "AbilitaIbernazione"),
        (AbilitaUtility, "Abilita Ottimizzazione FullScreen", "AbilitaOttimizzazioneFullScreen"),
        (AbilitaUtility, "Abilita Avvio Rapido", "AbilitaAvvioRapido"),
        (AbilitaUtility, "Abilita Background App", "AbilitaBackgroundApp"),
        (AbilitaUtility, "Abilita Feedback", "AbilitaFeedback"),
        (AbilitaUtility, "Abilita Advertising ID", "AbilitaAdvertisingID"),
        (AbilitaUtility, "Abilita Filtro Smart Screen", "AbilitaFiltroSmartScreen"),
        (AbilitaUtility, "Abilita Wifi Sense", "AbilitaWifiSense"),
        (AbilitaUtility, "Abilita Desktop Remoto", "AbilitaDesktopRemoto"),
        (AbilitaUtility, "Abilita Index File", "AbilitaIndexFile"),
        (AbilitaUtility, "Abilita attivazione del Numlock in avvio", "AbilitaattivazionedelNumlockinavvio"),
        (AbilitaUtility, "Abilita News e Interessi", "AbilitaNewseInteressi"),
        (AbilitaUtility, "Abilita Risparmio Energetico Personalizzato", "AbilitaRisparmioEnergeticoPersonalizzato"),
        (AbilitaUtility, "Abilita Mappe", "AbilitaMappe"),
        (AbilitaUtility, "Abilita UWP apps", "AbilitaUWPapps"),
        (AbilitaUtility, "Abilita Esperienze Personalizzate Microsoft", "AbilitaEsperienzePersonalizzateMicrosoft"),
        (AbilitaUtility, "Abilita Migliora uso SSD", "MigliorausoSSD")
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

        private void btnAvviaSelezionatiUti_Click(object? sender, EventArgs e)
        {
            var selection = new UtilitySelection(
                SnapshotCheckedItems(DisabilitaUtility),
                SnapshotCheckedItems(AbilitaUtility)
            );
            totalSteps = selection.Disable.Count + selection.Enable.Count;
            if (totalSteps == 0)
                return;
            progressBar1.MaxValue = totalSteps;
            progressBar1.Value = 0;

            if (!backgroundWorker1.IsBusy)
            {
                while (operationFailures.TryDequeue(out _))
                {
                    // Scarta tutti i risultati del tentativo precedente prima di iniziare il nuovo.
                }

                backgroundWorker1.RunWorkerAsync(selection);
            }
        }

        Task IImportedSettingsForm.ApplyImportedSettingsAsync() =>
            ImportedSettingsWorker.RunAsync(
                backgroundWorker1,
                () => btnAvviaSelezionatiUti_Click(btnAvviaSelezionatiVerdi, EventArgs.Empty));

        private static void RestartExplorer()
        {
            string explorerPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            _ = System.Diagnostics.Process.Start(explorerPath);
        }

        private void DisableScheduledTask(string taskPath)
        {
            try
            {
                using var taskService = new Microsoft.Win32.TaskScheduler.TaskService();
                var task = taskService.GetTask(taskPath);
                if (task != null)
                {
                    task.Enabled = false;
                }
            }
            catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
            {
                RecordOperationFailure(new InvalidOperationException(
                    $"Impossibile disabilitare l’attività pianificata '{taskPath}'.",
                    ex
                ));
            }
        }

        private void ConfigureNewsAndInterests(bool enabled)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "taskkill.exe"),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("/IM");
            startInfo.ArgumentList.Add("explorer.exe");
            startInfo.ArgumentList.Add("/F");
            using (Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile riavviare Explorer."))
            {
                process.WaitForExit();
            }

            int viewMode = enabled ? 1 : 2;
            int available = enabled ? 1 : 0;
            foreach (RegistryView view in new[] { RegistryView.Registry32, RegistryView.Registry64 })
            {
                using RegistryKey? key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view)
                    .CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Feeds");
                key?.SetValue("ShellFeedsTaskbarViewMode", viewMode, RegistryValueKind.DWord);
                key?.SetValue("IsFeedsAvailable", available, RegistryValueKind.DWord);
            }

            foreach (RegistryView view in new[] { RegistryView.Registry32, RegistryView.Registry64 })
            {
                QueueRegistryValue(RegistryHive.LocalMachine,
                    @"SOFTWARE\Policies\Microsoft\Windows\Windows Feeds",
                    "EnableFeeds", available, RegistryValueKind.DWord, view);
            }
        }

        private void ApplySelectedUtilityOption(
            HashSet<string> selectedOptions,
            string optionName,
            string checkboxName,
            Action apply,
            ref int currentStep,
            string? failureMessage = null)
        {
            if (!selectedOptions.Contains(optionName))
            {
                SetCheckboxState(checkboxName, false);
                return;
            }

            SetCheckboxState(checkboxName, true);
            currentStep++;
            backgroundWorker1.ReportProgress(currentStep);
            try
            {
                apply();
            }
            catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
            {
                RecordOperationFailure(failureMessage is null
                    ? ex
                    : new InvalidOperationException(failureMessage, ex));
            }
        }

        private void SetRegistryValue(string path, string name, object? value, RegistryView view = RegistryView.Default)
        {
            (RegistryHive hive, string subKeyPath) = ParseSystemRegistryPath(path);
            if (value is null)
            {
                QueueRegistryDeletion(hive, subKeyPath, name, view);
                return;
            }

            RegistryValueKind kind = value is string ? RegistryValueKind.String : RegistryValueKind.DWord;
            QueueRegistryValue(hive, subKeyPath, name, value, kind, view);
        }

        private void SetRegistryValueInBothViews(string path, string name, object? value)
        {
            SetRegistryValue(path, name, value, RegistryView.Registry32);
            SetRegistryValue(path, name, value, RegistryView.Registry64);
        }

        private void DeleteRegistryKey(string path, string name, RegistryView view = RegistryView.Default)
        {
            (RegistryHive hive, string subKeyPath) = ParseSystemRegistryPath(path);
            QueueRegistryDeletion(hive, subKeyPath, name, view);
        }

        private void DeleteRegistryKeyInBothViews(string path, string name)
        {
            DeleteRegistryKey(path, name, RegistryView.Registry32);
            DeleteRegistryKey(path, name, RegistryView.Registry64);
        }

        private void QueueRegistryValue(
            RegistryHive hive,
            string path,
            string name,
            object value,
            RegistryValueKind kind,
            RegistryView view)
        {
            (elevatedRegistryMutations
                ?? throw new InvalidOperationException("Batch Registro elevato non inizializzato."))
                .SetValue(hive, path, name, value, kind, view);
        }

        private void QueueRegistryDeletion(RegistryHive hive, string path, string name, RegistryView view)
        {
            (elevatedRegistryMutations
                ?? throw new InvalidOperationException("Batch Registro elevato non inizializzato."))
                .DeleteValue(hive, path, name, view);
        }

        private static (RegistryHive Hive, string Path) ParseSystemRegistryPath(string path)
        {
            const string usersPrefix = "HKEY_USERS\\";
            if (path.StartsWith(usersPrefix, StringComparison.OrdinalIgnoreCase))
                return (RegistryHive.Users, path[usersPrefix.Length..]);

            return (RegistryHive.LocalMachine, path);
        }

        private void ApplyElevatedRegistryMutations()
        {
            ElevatedRegistryMutationBatch? batch = elevatedRegistryMutations;
            elevatedRegistryMutations = null;
            if (batch is null || batch.Count == 0)
                return;

            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                Verb = "runas"
            };
            startInfo.ArgumentList.Add("-NoLogo");
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-Command");
            startInfo.ArgumentList.Add(batch.BuildCommand());

            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare il batch Registro elevato.");
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException($"Il batch Registro elevato è terminato con codice {process.ExitCode}.");
        }

        private void RunPowerShellCommands(string[] commands)
        {
            var commandString = string.Join("; ", commands);
            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                Verb = "runas"
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-Command");
            startInfo.ArgumentList.Add(commandString);

            using (var process = System.Diagnostics.Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare il comando elevato."))
            {
                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"PowerShell terminato con codice {process.ExitCode}.");
            }
        }
        private void EnableScheduledTask(string taskName)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = Path.Join(Environment.SystemDirectory, "schtasks.exe"),
                    UseShellExecute = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    Verb = "runas"
                };
                startInfo.ArgumentList.Add("/Change");
                startInfo.ArgumentList.Add("/TN");
                startInfo.ArgumentList.Add(taskName);
                startInfo.ArgumentList.Add("/ENABLE");

                using (var process = System.Diagnostics.Process.Start(startInfo)
                    ?? throw new InvalidOperationException("Impossibile avviare schtasks."))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        throw new InvalidOperationException($"schtasks.exe è terminato con codice {process.ExitCode} per l’attività '{taskName}'.");
                }
            }
            catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
            {
                RecordOperationFailure(new InvalidOperationException(
                    $"Impossibile riattivare l’attività pianificata '{taskName}'.",
                    ex
                ));
            }
        }
        private void RemoveDisabledProperties(RegistryKey backgroundAppsKey)
        {
            foreach (string subKeyName in backgroundAppsKey.GetSubKeyNames())
            {
                if (subKeyName.StartsWith("Microsoft.Windows.Cortana"))
                {
                    continue;
                }

                using (RegistryKey? subKey = backgroundAppsKey.OpenSubKey(subKeyName, true))
                {
                    if (subKey != null)
                    {
                        if (subKey.GetValue("Disabled") != null)
                        {
                            subKey.DeleteValue("Disabled", false);
                        }

                        if (subKey.GetValue("DisabledByUser") != null)
                        {
                            subKey.DeleteValue("DisabledByUser", false);
                        }
                    }
                }
            }
        }

        private void backgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument is not UtilitySelection selection)
            {
                e.Cancel = true;
                return;
            }

            HashSet<string> selectedToDisable = selection.Disable;
            HashSet<string> selectedToEnable = selection.Enable;
            elevatedRegistryMutations = new ElevatedRegistryMutationBatch();
            int currentStep = 0;
            if (selectedToDisable.Contains("Disabilita Background App"))
            {
                SetCheckboxState("DisabilitaBackgroundApp", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    using (RegistryKey? key64 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", true))
                    {
                        key64?.SetValue("GlobalUserDisabled", 1, RegistryValueKind.DWord);
                    }
                    using (RegistryKey? key32 = Registry.CurrentUser.OpenSubKey(@"Software\WOW6432Node\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", true))
                    {
                        key32?.SetValue("GlobalUserDisabled", 1, RegistryValueKind.DWord);
                    }
                    using (RegistryKey? baseKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", true))
                    {
                        if (baseKey != null)
                        {
                            foreach (RegistryKey? subKey in baseKey.GetSubKeyNames()
                                .Where(static subKeyName => !subKeyName.StartsWith("Microsoft.Windows.Cortana", StringComparison.Ordinal))
                                .Select(subKeyName => baseKey.OpenSubKey(subKeyName, true)))
                            {
                                using (subKey)
                                {
                                subKey?.SetValue("Disabled", 1, RegistryValueKind.DWord);
                                subKey?.SetValue("DisabledByUser", 1, RegistryValueKind.DWord);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(new InvalidOperationException("Modifica delle app in background non riuscita.", ex));
                }
            }
            else
            {
                SetCheckboxState("DisabilitaBackgroundApp", false);
            }
            if (selectedToDisable.Contains("Disabilita Feedback"))
            {
                SetCheckboxState("DisabilitaFeedback", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    using (RegistryKey? key32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Siuf\Rules", true))
                    {
                        key32?.SetValue("NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                    }
                    using (RegistryKey? key64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Siuf\Rules", true))
                    {
                        key64?.SetValue("NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                    }
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                        "DoNotShowFeedbackNotifications", 1, RegistryValueKind.DWord, RegistryView.Registry32);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                        "DoNotShowFeedbackNotifications", 1, RegistryValueKind.DWord, RegistryView.Registry64);
                    DisableScheduledTask(@"Microsoft\Windows\Feedback\Siuf\DmClient");
                    DisableScheduledTask(@"Microsoft\Windows\Feedback\Siuf\DmClientOnScenarioDownload");
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaFeedback", false);
            }
            if (selectedToDisable.Contains("Disabilita Advertising ID"))
            {
                SetCheckboxState("DisabilitaAdvertisingID", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo",
                        "DisabledByGroupPolicy", 1, RegistryValueKind.DWord, RegistryView.Registry32);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo",
                        "DisabledByGroupPolicy", 1, RegistryValueKind.DWord, RegistryView.Registry64);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(new InvalidOperationException("Modifica Advertising ID non riuscita.", ex));
                }
            }
            else
            {
                SetCheckboxState("DisabilitaAdvertisingID", false);
            }
            if (selectedToDisable.Contains("Disabilita Filtro Smart Screen"))
            {
                SetCheckboxState("DisabilitaFiltroSmartScreen", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System",
                        "EnableSmartScreen", 0, RegistryValueKind.DWord, RegistryView.Registry32);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System",
                        "EnableSmartScreen", 0, RegistryValueKind.DWord, RegistryView.Registry64);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\MicrosoftEdge\PhishingFilter",
                        "EnabledV9", 0, RegistryValueKind.DWord, RegistryView.Registry32);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\MicrosoftEdge\PhishingFilter",
                        "EnabledV9", 0, RegistryValueKind.DWord, RegistryView.Registry64);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaFiltroSmartScreen", false);
            }
            if (selectedToDisable.Contains("Disabilita Desktop Remoto"))
            {
                SetCheckboxState("DisabilitaDesktopRemoto", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Terminal Server",
                        "fDenyTSConnections", 1, RegistryValueKind.DWord, RegistryView.Registry32);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Terminal Server",
                        "fDenyTSConnections", 1, RegistryValueKind.DWord, RegistryView.Registry64);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp",
                        "UserAuthentication", 1, RegistryValueKind.DWord, RegistryView.Registry32);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp",
                        "UserAuthentication", 1, RegistryValueKind.DWord, RegistryView.Registry64);

                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaDesktopRemoto", false);
            }
            if (selectedToDisable.Contains("Disabilita attivazione del Numlock in avvio"))
            {
                SetCheckboxState("DisabilitaattivazionedelNumlockinavvio", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    QueueRegistryValue(RegistryHive.Users, @".DEFAULT\Control Panel\Keyboard", "InitialKeyboardIndicators",
                        2147483648U, RegistryValueKind.DWord, RegistryView.Registry32);
                    QueueRegistryValue(RegistryHive.Users, @".DEFAULT\Control Panel\Keyboard", "InitialKeyboardIndicators",
                        2147483648U, RegistryValueKind.DWord, RegistryView.Registry64);
                    if (Control.IsKeyLocked(Keys.NumLock))
                    {
                        SendKeys.SendWait("{NUMLOCK}");
                    }


                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaattivazionedelNumlockinavvio", false);
            }
            ApplySelectedUtilityOption(selectedToDisable, "Disabilita News e Interessi",
                "DisabilitaNewseInteressi", () => ConfigureNewsAndInterests(enabled: false), ref currentStep);
            ApplySelectedUtilityOption(selectedToDisable, "Disabilita Index File", "DisabilitaIndexFile",
                () => SetSystemVolumeIndexing(enabled: false), ref currentStep,
                "Disattivazione indicizzazione non riuscita.");
            if (selectedToDisable.Contains("Disabilita Edge PDF"))
            {
                SetCheckboxState("DisabilitaEdgePDF", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    string pdfKeyPath = @"Software\Classes\.pdf";
                    string openWithProgidsPath = @"Software\Classes\.pdf\OpenWithProgids";
                    string openWithListPath = @"Software\Classes\.pdf\OpenWithList";
                    using (RegistryKey pdfKey32 = Registry.CurrentUser.CreateSubKey(pdfKeyPath))
                    {
                        if (pdfKey32 != null)
                        {
                            pdfKey32.SetValue("NoOpenWith", "", RegistryValueKind.String);
                            pdfKey32.SetValue("NoStaticDefaultVerb", "", RegistryValueKind.String);
                        }
                    }

                    using (RegistryKey openWithProgidsKey32 = Registry.CurrentUser.CreateSubKey(openWithProgidsPath))
                    {
                        if (openWithProgidsKey32 != null)
                        {
                            openWithProgidsKey32.SetValue("NoOpenWith", "", RegistryValueKind.String);
                            openWithProgidsKey32.SetValue("NoStaticDefaultVerb", "", RegistryValueKind.String);
                        }
                    }

                    using (RegistryKey openWithListKey32 = Registry.CurrentUser.CreateSubKey(openWithListPath))
                    {
                        if (openWithListKey32 != null)
                        {
                            openWithListKey32.SetValue("NoOpenWith", "", RegistryValueKind.String);
                            openWithListKey32.SetValue("NoStaticDefaultVerb", "", RegistryValueKind.String);
                        }
                    }
                    string edgeKeyPath = @"Software\Classes\AppXd4nrz8ff68srnhf9t5a8sbjyar1cr723_";

                    using (RegistryKey edgeKey64 = Registry.CurrentUser.CreateSubKey(edgeKeyPath))
                    {
                        edgeKey64?.SetValue("AppXd4nrz8ff68srnhf9t5a8sbjyar1cr723_", "", RegistryValueKind.String);
                    }


                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaEdgePDF", false);
            }
            if (selectedToDisable.Contains("Disabilita Mappe"))
            {
                SetCheckboxState("DisabilitaMappe", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Maps",
                        "AutoUpdateEnabled", 0, RegistryValueKind.DWord, RegistryView.Registry64);
                    QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Maps",
                        "AutoUpdateEnabled", 0, RegistryValueKind.DWord, RegistryView.Registry64);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaMappe", false);
            }
            if (selectedToDisable.Contains("Disabilita UWP apps"))
            {
                SetCheckboxState("DisabilitaUWPapps", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    Version osVersion = Environment.OSVersion.Version;
                    if (osVersion.Build >= 17763)
                    {
                        string appPrivacyPath = @"SOFTWARE\Policies\Microsoft\Windows\AppPrivacy";
                        string[] restrictedCapabilities =
                        [
                            "LetAppsRunInBackground", "LetAppsActivateWithVoice", "LetAppsActivateWithVoiceAboveLock",
                            "LetAppsAccessNotifications", "LetAppsAccessAccountInfo", "LetAppsAccessContacts",
                            "LetAppsAccessCalendar", "LetAppsAccessPhone", "LetAppsAccessCallHistory",
                            "LetAppsAccessEmail", "LetAppsAccessTasks", "LetAppsAccessMessaging",
                            "LetAppsAccessRadios", "LetAppsSyncWithDevices", "LetAppsGetDiagnosticInfo"
                        ];
                        foreach (string property in restrictedCapabilities)
                            QueueRegistryValue(RegistryHive.LocalMachine, appPrivacyPath, property, 2,
                                RegistryValueKind.DWord, RegistryView.Registry64);

                        string consentStorePath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore";
                        foreach (string capability in new[] { "documentsLibrary", "picturesLibrary", "videosLibrary", "broadFileSystemAccess" })
                            QueueRegistryValue(RegistryHive.LocalMachine, $"{consentStorePath}\\{capability}", "Value", "Deny",
                                RegistryValueKind.String, RegistryView.Registry64);

                        QueueRegistryValue(RegistryHive.LocalMachine,
                            @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                            "SwapfileControl", 0, RegistryValueKind.DWord, RegistryView.Registry64);
                    }
                    else
                    {
                        using (RegistryKey? backgroundAccessKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", true))
                        {
                            if (backgroundAccessKey != null)
                            {
                                foreach (string subKey in backgroundAccessKey.GetSubKeyNames()
                                    .Where(static subKey =>
                                        !subKey.StartsWith("Microsoft.Windows.Cortana", StringComparison.Ordinal) &&
                                        !subKey.StartsWith("Microsoft.Windows.ShellExperienceHost", StringComparison.Ordinal)))
                                {
                                    using RegistryKey? appKey = backgroundAccessKey.OpenSubKey(subKey, true);
                                    appKey?.SetValue("Disabled", 1, RegistryValueKind.DWord);
                                    appKey?.SetValue("DisabledByUser", 1, RegistryValueKind.DWord);
                                }
                            }
                        }
                    }


                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaUWPapps", false);
            }
            if (selectedToDisable.Contains("Disabilita Esperienze Personalizzate Microsoft"))
            {
                SetCheckboxState("DisabilitaEsperienzePersonalizzateMicrosoft", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    foreach (string name in new[] { "EnableCdp", "EnableMmx" })
                    {
                        QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System",
                            name, 0, RegistryValueKind.DWord, RegistryView.Registry64);
                        QueueRegistryValue(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Policies\Microsoft\Windows\System",
                            name, 0, RegistryValueKind.DWord, RegistryView.Registry64);
                    }
                    using (var cloudContentKey64 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CloudContent"))
                    {
                        cloudContentKey64?.SetValue("DisableTailoredExperiencesWithDiagnosticData", 1, RegistryValueKind.DWord);
                    }

                    using (var cloudContentKey32 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WOW6432Node\Policies\Microsoft\Windows\CloudContent"))
                    {
                        cloudContentKey32?.SetValue("DisableTailoredExperiencesWithDiagnosticData", 1, RegistryValueKind.DWord);
                    }
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaEsperienzePersonalizzateMicrosoft", false);
            }
            if (selectedToDisable.Contains("Disabilita Storage Check"))
            {
                SetCheckboxState("DisabilitaStorageCheck", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    using (RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                    {
                        key64.DeleteSubKeyTree(@"SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy", false);
                    }
                    using (RegistryKey key32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                    {
                        key32.DeleteSubKeyTree(@"SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy", false);
                    }
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaStorageCheck", false);
            }
            ApplySelectedUtilityOption(selectedToDisable, "Disabilita Superfetch", "DisabilitaSuperfetch",
                () => SetRegistryValue(@"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 4), ref currentStep);
            if (selectedToDisable.Contains("Disabilita Storage Check"))
            {
                SetCheckboxState("DisabilitaStorageCheck", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKeyInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters", "StoragePolicy");
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("DisabilitaStorageCheck", false);
            }
            ApplySelectedUtilityOption(selectedToDisable, "Disabilita Ibernazione", "DisabilitaIbernazione", () =>
            {
                SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HibernateEnabled", 0);
                SetRegistryValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings", "ShowHibernateOption", 0);
                RunSystemTool("powercfg.exe", "/hibernate", "off");
            }, ref currentStep);
            ApplySelectedUtilityOption(selectedToDisable, "Disabilita Ottimizzazione FullScreen",
                "DisabilitaOttimizzazioneFullScreen", () =>
                {
                    SetRegistryValueInBothViews(@"System\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", 1);
                    SetRegistryValueInBothViews(@"System\GameConfigStore", "GameDVR_FSEBehavior", 2);
                    SetRegistryValueInBothViews(@"System\GameConfigStore", "GameDVR_FSEBehaviorMode", 2);
                    SetRegistryValueInBothViews(@"System\GameConfigStore", "GameDVR_HonorUserFSEBehaviorMode", 1);
                }, ref currentStep);
            ApplySelectedUtilityOption(selectedToDisable, "Disabilita Avvio Rapido", "DisabilitaAvvioRapido",
                () => SetRegistryValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", 0), ref currentStep);
            if (selectedToDisable.Contains("Normal Bandwidth"))
            {
                SetCheckboxState("NormalBandwidth", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    DeleteRegistryKeyInBothViews(@"SOFTWARE\Policies\Microsoft\Psched", "NonBestEffortLimit");
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("NormalBandwidth", false);
            }
            ApplySelectedUtilityOption(selectedToDisable, "Disabilita Migliora uso SSD",
                "DisabilitaMigliorausoSSD", () =>
                {
                    SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\FileSystem", "DisableLastAccess", 0);
                    SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\FileSystem", "EncryptPagingFile", 1);
                }, ref currentStep);
            if (selectedToEnable.Contains("Abilita Storage Check"))
            {
                SetCheckboxState("AbilitaStorageCheck", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    string storagePolicyKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy";
                    using (RegistryKey baseKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                    using (RegistryKey key32 = baseKey32.CreateSubKey(storagePolicyKey)
                        ?? throw new UnauthorizedAccessException($"Impossibile creare HKCU\\{storagePolicyKey} nella vista 32 bit."))
                    {
                        key32.SetValue("01", 1, RegistryValueKind.DWord);
                        key32.SetValue("04", 1, RegistryValueKind.DWord);
                        key32.SetValue("08", 1, RegistryValueKind.DWord);
                        key32.SetValue("32", 0, RegistryValueKind.DWord);
                        key32.SetValue("StoragePoliciesNotified", 1, RegistryValueKind.DWord);
                    }
                    using (RegistryKey baseKey64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                    using (RegistryKey key64 = baseKey64.CreateSubKey(storagePolicyKey)
                        ?? throw new UnauthorizedAccessException($"Impossibile creare HKCU\\{storagePolicyKey} nella vista 64 bit."))
                    {
                        key64.SetValue("01", 1, RegistryValueKind.DWord);
                        key64.SetValue("04", 1, RegistryValueKind.DWord);
                        key64.SetValue("08", 1, RegistryValueKind.DWord);
                        key64.SetValue("32", 0, RegistryValueKind.DWord);
                        key64.SetValue("StoragePoliciesNotified", 1, RegistryValueKind.DWord);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    RecordOperationFailure(ex);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaStorageCheck", false);
            }
            ApplySelectedUtilityOption(selectedToEnable, "Abilita Superfetch", "AbilitaSuperfetch", () =>
            {
                SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 2);
                RunSystemTool("sc.exe", "start", "SysMain");
            }, ref currentStep);
            ApplySelectedUtilityOption(selectedToEnable, "Abilita Ibernazione", "AbilitaIbernazione", () =>
            {
                SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HibernateEnabled", 1);
                SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FlyoutMenuSettings", "ShowHibernateOption", 1);
                RunSystemTool("powercfg.exe", "/hibernate", "on");
            }, ref currentStep);
            ApplySelectedUtilityOption(selectedToEnable, "Abilita Ottimizzazione FullScreen",
                "AbilitaOttimizzazioneFullScreen", () =>
                {
                    SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", 0);
                    DeleteRegistryKeyInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\GameConfigStore", "GameDVR_FSEBehavior");
                    SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\GameConfigStore", "GameDVR_FSEBehaviorMode", 0);
                    SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\GameConfigStore", "GameDVR_HonorUserFSEBehaviorMode", 0);
                }, ref currentStep);
            ApplySelectedUtilityOption(selectedToEnable, "Abilita Avvio Rapido", "AbilitaAvvioRapido",
                () => SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", 1), ref currentStep);
            if (selectedToEnable.Contains("All Bandwidth"))
            {
                SetCheckboxState("AllBandwidth", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetRegistryValueInBothViews(@"SOFTWARE\Policies\Microsoft\Psched", "NonBestEffortLimit", 0);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AllBandwidth", false);
            }
            if (selectedToEnable.Contains("Abilita Background App"))
            {
                SetCheckboxState("AbilitaBackgroundApp", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);

                try
                {
                    RegistryView view32 = RegistryView.Registry32;
                    RegistryView view64 = RegistryView.Registry64;
                    using (RegistryKey? backgroundAppsKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view32).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", true))
                    using (RegistryKey? backgroundAppsKey64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view64).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", true))
                    {
                        if (backgroundAppsKey32 != null)
                        {
                            backgroundAppsKey32.SetValue("GlobalUserDisabled", 0, RegistryValueKind.DWord);
                            RemoveDisabledProperties(backgroundAppsKey32);
                        }

                        if (backgroundAppsKey64 != null)
                        {
                            backgroundAppsKey64.SetValue("GlobalUserDisabled", 0, RegistryValueKind.DWord);
                            RemoveDisabledProperties(backgroundAppsKey64);
                        }
                    }
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaBackgroundApp", false);
            }

            if (selectedToEnable.Contains("Abilita Feedback"))
            {
                SetCheckboxState("AbilitaFeedback", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    using (RegistryKey? rulesKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32)
                        .OpenSubKey(@"SOFTWARE\Microsoft\Siuf\Rules", true))
                    {
                        if (rulesKey32 != null && rulesKey32.GetValue("NumberOfSIUFInPeriod") != null)
                        {
                            rulesKey32.DeleteValue("NumberOfSIUFInPeriod", false);
                        }
                    }
                    QueueRegistryDeletion(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                        "DoNotShowFeedbackNotifications", RegistryView.Registry64);
                    EnableScheduledTask("Microsoft\\Windows\\Feedback\\Siuf\\DmClient");
                    EnableScheduledTask("Microsoft\\Windows\\Feedback\\Siuf\\DmClientOnScenarioDownload");
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaFeedback", false);
            }
            if (selectedToEnable.Contains("Abilita Advertising ID"))
            {
                SetCheckboxState("AbilitaAdvertisingID", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    QueueRegistryDeletion(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo",
                        "DisabledByGroupPolicy", RegistryView.Registry32);
                    QueueRegistryDeletion(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo",
                        "DisabledByGroupPolicy", RegistryView.Registry64);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaAdvertisingID", false);
            }
            if (selectedToEnable.Contains("Abilita Filtro Smart Screen"))
            {
                SetCheckboxState("AbilitaFiltroSmartScreen", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    QueueRegistryDeletion(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System",
                        "EnableSmartScreen", RegistryView.Registry32);
                    QueueRegistryDeletion(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\MicrosoftEdge\PhishingFilter",
                        "EnabledV9", RegistryView.Registry32);
                    QueueRegistryDeletion(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System",
                        "EnableSmartScreen", RegistryView.Registry64);
                    QueueRegistryDeletion(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\MicrosoftEdge\PhishingFilter",
                        "EnabledV9", RegistryView.Registry64);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaFiltroSmartScreen", false);
            }
            if (selectedToEnable.Contains("Abilita Desktop Remoto"))
            {
                SetCheckboxState("AbilitaDesktopRemoto", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\Terminal Server", "fDenyTSConnections", 0);

                    SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "UserAuthentication", 0);
                    string[] commands = new[]
                    {
            "Enable-NetFirewallRule -Name \"RemoteDesktop*\""
        };
                    RunPowerShellCommands(commands);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaDesktopRemoto", false);
            }
            if (selectedToEnable.Contains("Abilita attivazione del Numlock in avvio"))
            {
                SetCheckboxState("AbilitaattivazionedelNumlockinavvio", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetRegistryValueInBothViews(@"HKEY_USERS\.DEFAULT\Control Panel\Keyboard", "InitialKeyboardIndicators", 2147483650);
                    string[] commands = new[]
                    {
            "Add-Type -AssemblyName System.Windows.Forms",
            "If (!([System.Windows.Forms.Control]::IsKeyLocked('NumLock'))) { $wsh = New-Object -ComObject WScript.Shell; $wsh.SendKeys('{NUMLOCK}') }"
        };
                    RunPowerShellCommands(commands);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaattivazionedelNumlockinavvio", false);
            }
            ApplySelectedUtilityOption(selectedToEnable, "Abilita News e Interessi",
                "AbilitaNewseInteressi", () => ConfigureNewsAndInterests(enabled: true), ref currentStep);
            ApplySelectedUtilityOption(selectedToEnable, "Abilita Index File", "AbilitaIndexFile",
                () => SetSystemVolumeIndexing(enabled: true), ref currentStep,
                "Attivazione indicizzazione non riuscita.");
            if (selectedToEnable.Contains("Abilita Risparmio Energetico Personalizzato"))
            {
                SetCheckboxState("AbilitaRisparmioEnergeticoPersonalizzato", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = Path.Join(Environment.SystemDirectory, "powercfg.exe"),
                        UseShellExecute = true,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        Verb = "runas"
                    };
                    startInfo.ArgumentList.Add("-duplicatescheme");
                    startInfo.ArgumentList.Add("e9a42b02-d5df-448d-aa00-03f14749eb61");

                    using (var process = System.Diagnostics.Process.Start(startInfo)
                        ?? throw new InvalidOperationException("Impossibile avviare il processo di risparmio energetico."))
                    {
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                            throw new InvalidOperationException($"powercfg terminato con codice {process.ExitCode}.");
                    }
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(new InvalidOperationException("Creazione del profilo energetico non riuscita.", ex));
                }
            }
            else
            {
                SetCheckboxState("AbilitaRisparmioEnergeticoPersonalizzato", false);
            }
            ApplySelectedUtilityOption(selectedToEnable, "Abilita Migliora uso SSD", "MigliorausoSSD", () =>
            {
                SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\FileSystem", "DisableLastAccess", 1);
                SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\FileSystem", "EncryptPagingFile", 0);
            }, ref currentStep);

            if (selectedToEnable.Contains("Abilita Mappe"))
            {
                SetCheckboxState("AbilitaMappe", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetRegistryValueInBothViews(@"SYSTEM\Maps", "AutoUpdateEnabled", null);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaMappe", false);
            }
            if (selectedToEnable.Contains("Abilita UWP apps"))
            {
                SetCheckboxState("AbilitaUWPapps", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetRegistryValueInBothViews(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "SwapfileControl", null);
                    SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\documentsLibrary", "Value", "Allow");
                    SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\picturesLibrary", "Value", "Allow");
                    SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\videosLibrary", "Value", "Allow");
                    SetRegistryValueInBothViews(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\broadFileSystemAccess", "Value", "Allow");
                    string appPrivacyPath = @"SOFTWARE\Policies\Microsoft\Windows\AppPrivacy";
                    string[] propertiesToRemove = {
                "LetAppsGetDiagnosticInfo",
                "LetAppsSyncWithDevices",
                "LetAppsAccessRadios",
                "LetAppsAccessMessaging",
                "LetAppsAccessTasks",
                "LetAppsAccessEmail",
                "LetAppsAccessCallHistory",
                "LetAppsAccessPhone",
                "LetAppsAccessCalendar",
                "LetAppsAccessContacts",
                "LetAppsAccessAccountInfo",
                "LetAppsAccessNotifications",
                "LetAppsActivateWithVoice",
                "LetAppsActivateWithVoiceAboveLock",
                "LetAppsRunInBackground"
            };

                    foreach (string property in propertiesToRemove)
                    {
                        SetRegistryValueInBothViews(appPrivacyPath, property, null);
                    }
                    using (RegistryKey? backgroundAccessKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", true))
                    {
                        if (backgroundAccessKey != null)
                        {
                            foreach (var subkeyName in backgroundAccessKey.GetSubKeyNames())
                            {
                                using (var subkey = backgroundAccessKey.OpenSubKey(subkeyName, true))
                                {
                                    if (subkey != null)
                                    {
                                        subkey.DeleteValue("Disabled", false);
                                        subkey.DeleteValue("DisabledByUser", false);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaUWPapps", false);
            }

            if (selectedToEnable.Contains("Abilita Esperienze Personalizzate Microsoft"))
            {
                SetCheckboxState("AbilitaEsperienzePersonalizzateMicrosoft", true);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
                try
                {
                    SetRegistryValueInBothViews(@"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableTailoredExperiencesWithDiagnosticData", null);
                }
                catch (Exception ex) when (IsExpectedSystemOperationFailure(ex))
                {
                    RecordOperationFailure(ex);
                }
            }
            else
            {
                SetCheckboxState("AbilitaEsperienzePersonalizzateMicrosoft", false);
            }

            ApplyElevatedRegistryMutations();
        }

        private void backgroundWorker1_ProgressChanged(object? sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = Math.Min(e.ProgressPercentage, progressBar1.MaxValue);
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error is not null)
            {
                string errorMessage = LanguageManager.GetTranslation("Global", "erroreoperazione");
                string details = e.Error.GetBaseException().Message;
                string? recordedFailures = GetOperationFailureSummary();
                if (recordedFailures is not null)
                    details += Environment.NewLine + recordedFailures;

                _ = MessageBox.Show(
                    $"{errorMessage}{Environment.NewLine}{details}",
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (e.Cancelled)
            {
                _ = MessageBox.Show(
                    LanguageManager.GetTranslation("Global", "operazioneannullatatoken"),
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            RestartExplorer();
            string? operationFailureSummary = GetOperationFailureSummary();
            if (operationFailureSummary is not null)
            {
                _ = MessageBox.Show(
                    $"{LanguageManager.GetTranslation("Global", "erroreoperazione")}{Environment.NewLine}{operationFailureSummary}",
                    "WinHubX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
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

        private void AbilitaUtility_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string itemName = AbilitaUtility.Items[e.Index]?.ToString() ?? string.Empty;
                string disabilitaName = itemName.Replace("Abilita", "Disabilita");

                int index = DisabilitaUtility.Items.IndexOf(disabilitaName);
                if (index >= 0)
                {
                    DisabilitaUtility.ItemCheck -= DisabilitaUtility_ItemCheck;
                    DisabilitaUtility.SetItemChecked(index, false);
                    DisabilitaUtility.ItemCheck += DisabilitaUtility_ItemCheck;
                }
            }
        }

        private void DisabilitaUtility_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                string itemName = DisabilitaUtility.Items[e.Index]?.ToString() ?? string.Empty;
                string abilitaName = itemName.Replace("Disabilita", "Abilita");

                int index = AbilitaUtility.Items.IndexOf(abilitaName);
                if (index >= 0)
                {
                    AbilitaUtility.ItemCheck -= AbilitaUtility_ItemCheck;
                    AbilitaUtility.SetItemChecked(index, false);
                    AbilitaUtility.ItemCheck += AbilitaUtility_ItemCheck;
                }
            }
        }

        private void AbilitaUtility_MouseDown(object? sender, MouseEventArgs e)
        {
            int index = AbilitaUtility.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                AbilitaUtility.SetItemChecked(index, !AbilitaUtility.GetItemChecked(index));
            }
            AbilitaUtility.ClearSelected();
        }

        private void DisabilitaUtility_MouseDown(object? sender, MouseEventArgs e)
        {
            int index = DisabilitaUtility.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                DisabilitaUtility.SetItemChecked(index, !DisabilitaUtility.GetItemChecked(index));
            }
            DisabilitaUtility.ClearSelected();
        }

        private void btnReset_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < AbilitaUtility.Items.Count; i++)
            {
                AbilitaUtility.SetItemChecked(i, false);
            }
            for (int i = 0; i < DisabilitaUtility.Items.Count; i++)
            {
                DisabilitaUtility.SetItemChecked(i, false);
            }
        }
    }
}
