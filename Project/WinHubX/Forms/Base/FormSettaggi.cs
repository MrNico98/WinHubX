using CuoreUI.Controls;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;
using WinHubX.Forms.Settaggi;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.Base
{
    public partial class FormSettaggi : Form
    {
        private Form1 form1;
        private string? wsa11x64;
        private string? wsa11arm64;
        private string? wsa10x64;
        private FormPersonalizzazione? formPersonalizzazione;

        public FormSettaggi(Form1 form1)
        {
            InitializeComponent();
            form1 = form1;
            LoadJsonLinks();
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
            string hardwarePath = Path.Combine(
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
            string hardwarePath = Path.Combine(
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
            string hardwarePath = Path.Combine(
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

            Form1 mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;

            mainForm.pictureBox3.Visible = true;

            mainForm.pictureBox3.Click -= PictureBox3_Click_BackToTweaks;
            mainForm.pictureBox3.Click += PictureBox3_Click_BackToTweaks;

            mainForm.lblPanelTitle.Text = titoloTraduzione;
            mainForm.pictureBoxlblalto.Image = button.Image;
            Form form = (Form)Activator.CreateInstance(typeof(T), this, mainForm);
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panel70.Controls.Add(form);
            form.Show();
        }
        private void PictureBox3_Click_BackToTweaks(object sender, EventArgs e)
        {
            Form1 mainForm = Application.OpenForms["Form1"] as Form1;
            if (mainForm == null) return;

            mainForm.pictureBox3.Visible = false;
            mainForm.LoadForm(new FormSettaggi(mainForm), mainForm.btnSettaggi, "Tweaks");
        }
        private async void LoadJsonLinks()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(Dipendenze.GitHubConfigUrl);
                    JObject data = JObject.Parse(json);

                    wsa11x64 = data["WSA"]?["win11x64"]?.ToString();
                    wsa11arm64 = data["WSA"]?["win11arm64"]?.ToString();
                    wsa10x64 = data["WSA"]?["win10x64"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAttivaWSL_Click(object sender, EventArgs e)
        {
            try
            {
                string assemblyName1 = Assembly.GetExecutingAssembly().GetName().Name;
                string resourcePath1 = $"{assemblyName1}.Resources.WinHubXWSL.ps1";
                byte[] exeBytes1 = LoadEmbeddedResource1(resourcePath1);
                string ps1FilePath1 = Path.Combine(Path.GetTempPath(), "WinHubXWSL.ps1");
                File.WriteAllBytes(ps1FilePath1, exeBytes1);

                StartPowerShell1(ps1FilePath1);
            }
            finally { }
        }

        private byte[] LoadEmbeddedResource1(string resourcePath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
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

        private void StartPowerShell1(string scriptFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{scriptFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                _ = process.Start();
                string output = process.StandardOutput.ReadToEnd();
            }
        }



        private void btnImportaSettaggi_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Seleziona file di registro da importare";
                dlg.Filter = "Dat file (*.dat)|*.dat|Tutti i file (*.*)|*.*";
                dlg.InitialDirectory = Application.StartupPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dlg.FileName;

                    var process = new Process();
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = $"import \"{filePath}\"";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;

                    try
                    {
                        _ = process.Start();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            _ = MessageBox.Show("Settaggi importati correttamente dal file .dat.",
                                "Importazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IstanziaEAvviaFormSelezionati();
                        }
                        else
                        {
                            _ = MessageBox.Show($"Errore durante l'importazione. Codice uscita: {process.ExitCode}",
                                "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show($"Si è verificato un errore:\n{ex.Message}",
                            "Eccezione", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void ImportaSettaggiDaPercorso(string filePath)
        {
            var process = new Process();
            process.StartInfo.FileName = "reg.exe";
            process.StartInfo.Arguments = $"import \"{filePath}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            try
            {
                _ = process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Settaggi importati correttamente dal file .dat.");
                    IstanziaEAvviaFormSelezionati();
                }
                else
                {
                    Console.WriteLine($"Errore durante l'importazione. Codice uscita: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Si è verificato un errore:\n{ex.Message}");
            }
        }

        private void IstanziaEAvviaFormSelezionati()
        {
            var formList = new List<Form>
    {
        new FormPrivacy(this, form1),
        new FormUtility(this, form1),
        new FormDefender(this, form1),
        new FormUpdate(this, form1),
        new FormPersonalizzazione(this, form1)
    };

            foreach (Form form in formList)
            {
                form.TopLevel = false;
                form.TopMost = true;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.CreateControl();
                form.Show();
                var metodo = form.GetType().GetMethod("btnAvviaSelezionati_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bottone = form.Controls.Find("btnAvviaSelezionati", true).FirstOrDefault();

                if (metodo != null && bottone != null)
                {
                    _ = metodo.Invoke(form, new object[] { bottone, EventArgs.Empty });
                }
                form.Close();
            }
        }

        private void cuiButton1_Click(object sender, EventArgs e)
        {

        }

        private void cuiFileDropper2_FileDropped(object sender, CuoreUI.Controls.FileDroppedEventArgs e)
        {

        }

        private void cuiFileDropper1_FileDropped(object sender, CuoreUI.Controls.FileDroppedEventArgs e)
        {
            DoImport();
        }

        private void DoImport()
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = LanguageManager.GetTranslation("FormSettaggi", "exporttitle");
                dlg.Filter = "Dat file (*.dat)|*.dat|Tutti i file (*.*)|*.*";
                dlg.FileName = "config.dat";
                dlg.InitialDirectory = Application.StartupPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string exportPath = dlg.FileName;
                    string keyToExport = @"HKEY_CURRENT_USER\Software\WinHubX";

                    var process = new Process();
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = $"export \"{keyToExport}\" \"{exportPath}\" /y";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;

                    try
                    {
                        _ = process.Start();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            _ = MessageBox.Show(
                                string.Format(LanguageManager.GetTranslation("FormSettaggi", "exportsuccess"), exportPath),
                                LanguageManager.GetTranslation("FormSettaggi", "exportdone"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            _ = MessageBox.Show(
                                string.Format(LanguageManager.GetTranslation("FormSettaggi", "exporterrorcode"), process.ExitCode),
                                LanguageManager.GetTranslation("FormSettaggi", "error"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show(
                            string.Format(LanguageManager.GetTranslation("FormSettaggi", "exportexception"), ex.Message),
                            LanguageManager.GetTranslation("FormSettaggi", "exception"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void FormSettaggi_Load(object sender, EventArgs e)
        {
            cuiFileDropper2White.Click += (s, e) => MessageBox.Show("Click di cuiFileDropper2");
            cuiFileDropper2White.MouseDown += (s, e) => MessageBox.Show("MouseDown");
            cuiFileDropper2White.MouseUp += (s, e) => MessageBox.Show("MouseUp");
            cuiFileDropper2White.DoubleClick += (s, e) => Console.WriteLine("DoubleClick");
        }
        private void DoExport()
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = LanguageManager.GetTranslation("FormSettaggi", "exporttitle");
                dlg.Filter = "Dat file (*.dat)|*.dat|Tutti i file (*.*)|*.*";
                dlg.FileName = "config.dat";
                dlg.InitialDirectory = Application.StartupPath;

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                string exportPath = dlg.FileName;
                string keyToExport = @"HKEY_CURRENT_USER\Software\WinHubX";

                using (var process = new Process())
                {
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = $"export \"{keyToExport}\" \"{exportPath}\" /y";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;

                    try
                    {
                        process.Start();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            MessageBox.Show(
                                string.Format(LanguageManager.GetTranslation("FormSettaggi", "exportsuccess"), exportPath),
                                LanguageManager.GetTranslation("FormSettaggi", "exportdone"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            MessageBox.Show(
                                string.Format(LanguageManager.GetTranslation("FormSettaggi", "exporterrorcode"), process.ExitCode),
                                LanguageManager.GetTranslation("FormSettaggi", "error"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format(LanguageManager.GetTranslation("FormSettaggi", "exportexception"), ex.Message),
                            LanguageManager.GetTranslation("FormSettaggi", "exception"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void cuiFileDropper2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DoExport();
            }
        }

        private void cuiFileDropper1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DoImport();
            }
        }
    }
}
