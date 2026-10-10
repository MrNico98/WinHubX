using HartUI.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using WinHubX.Forms.Settaggi;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormSettaggi : Form
    {
        private readonly Form1 form1;

        public FormSettaggi(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            btnWSLTweaksPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Attiva WSL",
                "en" => "  Activate WSL",
                _ => btnWSLTweaksPrinci.Content
            };
            btnRipristinoeTestTweaksPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Ripristino e test",
                "en" => "  Recovery and testing",
                _ => btnRipristinoeTestTweaksPrinci.Content
            };
            btnPersonalizzazioneTweaksPrinci.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Personalizzazione",
                "en" => "  Customization",
                _ => btnPersonalizzazioneTweaksPrinci.Content
            };
            btnWSATweaksDisattivo.Content = LanguageManager.CurrentLanguage switch
            {
                "it" => "  Attiva WSA",
                "en" => "  Activate WSA",
                _ => btnWSATweaksDisattivo.Content
            };
            cuiFileDropper1White.UploadContent = LanguageManager.CurrentLanguage switch
            {
                "it" => "Clicca o trascina qui il file",
                "en" => "Click or drag the file here",
                _ => cuiFileDropper1White.UploadContent
            };

            cuiFileDropper1White.NormalContent = LanguageManager.CurrentLanguage switch
            {
                "it" => "Importa",
                "en" => "Import",
                _ => cuiFileDropper1White.NormalContent
            };


            cuiFileDropper2White.UploadContent = LanguageManager.CurrentLanguage switch
            {
                "it" => "Clicca qui per estrarre il file",
                "en" => "Click here to extract the file",
                _ => cuiFileDropper2White.UploadContent
            };

            cuiFileDropper2White.NormalContent = LanguageManager.CurrentLanguage switch
            {
                "it" => "Esporta",
                "en" => "Export",
                _ => cuiFileDropper2White.NormalContent
            };
        }

        private void btnPrivacy_Click(object sender, EventArgs e)
        {
            string hardwarePath = Path.Join(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(hardwarePath))
            {
                var popup = new WinHubX.DialogBlock.Form_DialogBlock(form1);
                popup.StartPosition = FormStartPosition.CenterScreen;
                popup.ShowDialog();
                return;
            }
            MostraFormInPanel<FormPrivacy>("Privacy", btnPrivacyTweaksPrinci);
        }

        private void btnUtility_Click(object sender, EventArgs e)
        {
            string hardwarePath = Path.Join(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(hardwarePath))
            {
                var popup = new WinHubX.DialogBlock.Form_DialogBlock(form1);
                popup.StartPosition = FormStartPosition.CenterScreen;
                popup.ShowDialog();
                return;
            }
            MostraFormInPanel<FormUtility>("Utility", btnUtilityTweaksPrinci);
        }

        private void btnDefender_Click(object sender, EventArgs e)
        {
            MostraFormInPanel<FormDefender>("Defender", btnDefenderTweaksPrinci);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            MostraFormInPanel<FormUpdate>("Update", btnUpdateTweaksPrinci);
        }

        private void btnRipristinaSO_Click(object sender, EventArgs e)
        {
            MostraFormInPanel<FormRipristinoSO>(
                LanguageManager.GetTranslation("FormSettaggi", "restoreos"),
                btnRipristinoeTestTweaksPrinci
            );
        }

        private void btnPersonalizzazione_Click(object sender, EventArgs e)
        {
            string hardwarePath = Path.Join(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "WinHubX", "Computer", "osehardware.json");

            if (!File.Exists(hardwarePath))
            {
                var popup = new WinHubX.DialogBlock.Form_DialogBlock(form1);
                popup.StartPosition = FormStartPosition.CenterScreen;
                popup.ShowDialog();
                return;
            }
            MostraFormInPanel<FormPersonalizzazione>(LanguageManager.GetTranslation("FormSettaggi", "customization"), btnPersonalizzazioneTweaksPrinci);
        }

        private void MostraFormInPanel<T>(string titoloTraduzione, cuiButton button) where T : Form
        {
            panel70.Controls.Clear();

            Form1? mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;

            mainForm.pictureBox3.Visible = true;

            mainForm.pictureBox3.Click -= PictureBox3_Click_BackToTweaks;
            mainForm.pictureBox3.Click += PictureBox3_Click_BackToTweaks;

            mainForm.lblPanelTitle.Text = titoloTraduzione;
            mainForm.pictureBoxlblalto.Image = button.Image;
            Form form = Activator.CreateInstance(typeof(T), this, mainForm) as Form
                ?? throw new InvalidOperationException($"Impossibile creare {typeof(T).Name}.");
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panel70.Controls.Add(form);
            form.Show();
        }
        private void PictureBox3_Click_BackToTweaks(object? sender, EventArgs e)
        {
            Form1? mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;

            mainForm.pictureBox3.Visible = false;
            mainForm.LoadForm(new FormSettaggi(mainForm), mainForm.btnSettaggi, "Tweaks");
        }
        private async void btnAttivaWSL_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "WinHubX eseguirà lo script WSL incorporato con privilegi amministrativi. Continuare?",
                    "Conferma attivazione WSL",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            string? scriptPath = null;
            try
            {
                string assemblyName1 = Assembly.GetExecutingAssembly().GetName().Name
                    ?? throw new InvalidOperationException("Nome assembly non disponibile.");
                string resourcePath1 = $"{assemblyName1}.Resources.WinHubXWSL.ps1";
                byte[] exeBytes1 = LoadEmbeddedResource1(resourcePath1);
                scriptPath = Path.Join(Path.GetTempPath(), $"WinHubXWSL-{Guid.NewGuid():N}.ps1");
                using (FileStream scriptFile = new(scriptPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read, 64 * 1024, FileOptions.SequentialScan))
                {
                    scriptFile.Write(exeBytes1, 0, exeBytes1.Length);
                    scriptFile.Flush(flushToDisk: true);
                }

                await StartPowerShell1Async(scriptPath);
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or InvalidOperationException
                or OperationCanceledException)
            {
                Debug.WriteLine($"Attivazione WSL non riuscita: {ex}");
                MessageBox.Show($"Impossibile completare l'attivazione WSL.\n{ex.Message}", "WinHubX",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (scriptPath is not null)
                {
                    try { File.Delete(scriptPath); }
                    catch (IOException ex) { Debug.WriteLine($"Impossibile eliminare lo script WSL temporaneo: {ex}"); }
                    catch (UnauthorizedAccessException ex) { Debug.WriteLine($"Accesso negato durante la rimozione dello script WSL: {ex}"); }
                }
            }
        }

        private byte[] LoadEmbeddedResource1(string resourcePath)
        {
            using (Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Error: {resourcePath}");
                }
                byte[] buffer = new byte[stream.Length];
                _ = stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        private static async Task StartPowerShell1Async(string scriptFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-File");
            startInfo.ArgumentList.Add(scriptFilePath);

            using (Process process = new Process { StartInfo = startInfo })
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException("Impossibile avviare lo script WSL.");
                }

                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Lo script WSL è terminato con codice {process.ExitCode}.");
                }
            }
        }



        public async Task ImportaSettaggiDaPercorsoAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                _ = MessageBox.Show("Il file di configurazione selezionato non esiste.",
                    "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                RegistryPresetFileValidator.Validate(filePath);
            }
            catch (Exception ex) when (ex is InvalidDataException or IOException or UnauthorizedAccessException)
            {
                _ = MessageBox.Show($"Impossibile leggere il file di configurazione: {ex.GetBaseException().Message}",
                    "File non valido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult confirmation = MessageBox.Show(
                "L'importazione ripristina le selezioni e applica i tweak salvati; alcune modifiche interessano il sistema e potrebbero richiedere privilegi amministrativi. Continuare?",
                "Conferma importazione e applicazione",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirmation != DialogResult.Yes)
                return;

            try
            {
                using FileStream presetFile = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                RegistryPresetFileValidator.Validate(presetFile);

                string systemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string registryEditorPath = Path.Join(systemDirectory, "reg.exe");
                if (!File.Exists(registryEditorPath))
                    throw new FileNotFoundException("reg.exe non è disponibile nella cartella di sistema.", registryEditorPath);

                ProcessStartInfo startInfo = new(registryEditorPath)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                startInfo.ArgumentList.Add("import");
                startInfo.ArgumentList.Add(Path.GetFullPath(filePath));

                using Process process = new() { StartInfo = startInfo };
                if (!process.Start())
                    throw new InvalidOperationException("Impossibile avviare reg.exe per importare la configurazione.");

                Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();
                string processOutput = await standardOutputTask;
                string processError = await standardErrorTask;
                if (process.ExitCode != 0)
                {
                    string details = string.Join(Environment.NewLine,
                        new[] { processError, processOutput }.Where(static text => !string.IsNullOrWhiteSpace(text)).Select(static text => text.Trim()));
                    string message = $"Errore durante l'importazione. Codice uscita: {process.ExitCode}";
                    if (!string.IsNullOrWhiteSpace(details))
                        message += Environment.NewLine + details;
                    _ = MessageBox.Show(message,
                        "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await ApplicaFormSelezionatiAsync();
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Text.Json.JsonException
                or InvalidDataException
                or InvalidOperationException
                or OperationCanceledException
                or ArgumentException)
            {
                _ = MessageBox.Show($"Importazione o applicazione non completata:\n{ex.GetBaseException().Message}",
                    "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ApplicaFormSelezionatiAsync()
        {
            panel70.Controls.Clear();
            Func<IImportedSettingsForm>[] createForms =
            [
                () => new FormPrivacy(this, form1),
                () => new FormUtility(this, form1),
                () => new FormDefender(this, form1),
                () => new FormUpdate(this, form1),
                () => new FormPersonalizzazione(this, form1)
            ];

            try
            {
                foreach (Form form in createForms.Select(static createForm => (Form)createForm()))
                {
                    using (form)
                    {
                        form.TopLevel = false;
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.Dock = DockStyle.Fill;
                        panel70.Controls.Add(form);
                        form.Show();

                        try
                        {
                            await ((IImportedSettingsForm)form).ApplyImportedSettingsAsync();
                        }
                        finally
                        {
                            panel70.Controls.Remove(form);
                            form.Close();
                        }
                    }
                }
            }
            finally
            {
                if (!form1.IsDisposed)
                {
                    form1.pictureBox3.Visible = false;
                    form1.LoadForm(new FormSettaggi(form1), form1.btnSettaggi, "Tweaks");
                }
            }
        }

        private void cuiButton1_Click(object sender, EventArgs e)
        {

        }

        private async void cuiFileDropper1_FileDropped(object sender, HartUI.Controls.FileDroppedEventArgs e)
        {
            string? filePath = e.FileNames?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(filePath))
                await ImportaSettaggiDaPercorsoAsync(filePath);
        }

        private async void cuiFileDropper2_Click(object sender, EventArgs e)
        {
            using SaveFileDialog dialog = new()
            {
                Title = LanguageManager.GetTranslation("FormSettaggi", "exporttitle"),
                Filter = "Dat file (*.dat)|*.dat|Tutti i file (*.*)|*.*",
                FileName = "config.dat",
                InitialDirectory = Application.StartupPath
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string registryEditorPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.System), "reg.exe");
                if (!File.Exists(registryEditorPath))
                    throw new FileNotFoundException("reg.exe non è disponibile nella cartella di sistema.", registryEditorPath);

                ProcessStartInfo startInfo = new(registryEditorPath)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                startInfo.ArgumentList.Add("export");
                startInfo.ArgumentList.Add(@"HKEY_CURRENT_USER\Software\WinHubX");
                startInfo.ArgumentList.Add(Path.GetFullPath(dialog.FileName));
                startInfo.ArgumentList.Add("/y");

                using Process process = new() { StartInfo = startInfo };
                if (!process.Start())
                    throw new InvalidOperationException("Impossibile avviare reg.exe per esportare la configurazione.");

                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();
                string output = await outputTask;
                string error = await errorTask;

                if (process.ExitCode == 0)
                {
                    _ = MessageBox.Show(
                        LanguageManager.FormatTranslation("FormSettaggi", "exportsuccess", dialog.FileName),
                        LanguageManager.GetTranslation("FormSettaggi", "exportdone"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                string details = string.Join(Environment.NewLine,
                    new[] { error, output }.Where(static text => !string.IsNullOrWhiteSpace(text)).Select(static text => text.Trim()));
                string message = LanguageManager.FormatTranslation("FormSettaggi", "exporterrorcode", process.ExitCode);
                if (!string.IsNullOrWhiteSpace(details))
                    message += Environment.NewLine + details;
                _ = MessageBox.Show(message, LanguageManager.GetTranslation("FormSettaggi", "error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) when (ex is IOException
                or UnauthorizedAccessException
                or System.Security.SecurityException
                or System.ComponentModel.Win32Exception
                or System.Text.Json.JsonException
                or InvalidDataException
                or InvalidOperationException
                or ArgumentException)
            {
                _ = MessageBox.Show(
                    LanguageManager.FormatTranslation("FormSettaggi", "exportexception", ex.GetBaseException().Message),
                    LanguageManager.GetTranslation("FormSettaggi", "exception"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

    }
}

namespace WinHubX.Forms.Base
{
    internal interface IImportedSettingsForm
    {
        Task ApplyImportedSettingsAsync();
    }

    internal static class ImportedSettingsWorker
    {
        internal static Task RunAsync(BackgroundWorker worker, Action start)
        {
            if (worker.IsBusy)
                throw new InvalidOperationException("È già in corso un'operazione su questa schermata.");

            TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
            RunWorkerCompletedEventHandler handler = (_, _) => completion.TrySetResult();
            worker.RunWorkerCompleted += handler;

            try
            {
                start();
                if (!worker.IsBusy)
                    completion.TrySetResult();
            }
            catch
            {
                worker.RunWorkerCompleted -= handler;
                throw;
            }

            return AwaitAndDetachAsync(worker, handler, completion.Task);
        }

        private static async Task AwaitAndDetachAsync(
            BackgroundWorker worker,
            RunWorkerCompletedEventHandler handler,
            Task completion)
        {
            try
            {
                await completion;
            }
            finally
            {
                worker.RunWorkerCompleted -= handler;
            }
        }
    }
}
