using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using WinHubX.Forms.DebloatAvanzato;

namespace WinHubX.Forms.Base
{
    public partial class FormDebloat : Form
    {
        private Form1 form1;
        private List<string> appxNames = new List<string>();
        private Dictionary<string, string> appNameMappings = new Dictionary<string, string>();
        private Dictionary<string, string> imageUrls = new Dictionary<string, string>();
        private int totalSteps = 0;

        public FormDebloat(Form1 form1)
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
            this.form1 = form1;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.HorizontalScroll.Maximum = 0;
            flowLayoutPanel1.HorizontalScroll.Visible = false;
            flowLayoutPanel1.AutoScrollMinSize = new Size(0, 0);
            InizializzaDati();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private async void InizializzaDati()
        {
            await Task.Run(async () =>
            {
                try
                {
                    await Task.WhenAll(CaricaAppNameMappings(), CaricaImmaginiApp());
                }
                catch
                {

                }

                CaricaAppxPackages();
            });
        }


        private class ImmagineData
        {
            public required string Nome { get; set; }
            public required string ID { get; set; }
            public required string ImmagineUrl { get; set; }
        }
        private async Task CaricaImmaginiApp()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync("https://raw.githubusercontent.com/MrNico98/ImageDebloat/refs/heads/main/ImmaginiDebloat.json");
                    var immaginiList = JsonSerializer.Deserialize<List<ImmagineData>>(json);

                    if (immaginiList != null)
                    {
                        foreach (var item in immaginiList)
                        {
                            string chiave = item.ID ?? item.Nome;
                            if (!string.IsNullOrEmpty(chiave) && !string.IsNullOrEmpty(item.ImmagineUrl))
                            {
                                imageUrls[chiave] = item.ImmagineUrl;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task CaricaAppNameMappings()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync("https://raw.githubusercontent.com/MrNico98/ImageDebloat/refs/heads/main/AssociazioniDebloat.json");
                    appNameMappings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private void CaricaAppxPackages()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Get-AppxPackage | Where-Object { $_.SignatureKind -eq 'Store' } | Select-Object -ExpandProperty Name\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    _ = process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    appxNames = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                this.Invoke(new Action(() =>
                {
                    AggiornaUI(appxNames);
                }));
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AggiornaUI(List<string> filteredApps)
        {
            flowLayoutPanel1.Controls.Clear();
            foreach (string appName in filteredApps)
            {
                AggiungiElemento(appName);
            }
        }

        private void AggiungiElemento(string nomeTecnico)
        {
            string nomeLeggibile = OttieniNomeLeggibile(nomeTecnico);
            string? imgUrl = imageUrls.ContainsKey(nomeLeggibile) ? imageUrls[nomeLeggibile] : null;
            if (string.IsNullOrEmpty(imgUrl) && imageUrls.ContainsKey("Generale"))
            {
                imgUrl = imageUrls["Generale"];
            }

            int panelWidth = (flowLayoutPanel1.ClientSize.Width / 2) - 20;
            int panelHeight = 50;

            Panel panel = new Panel
            {
                Width = panelWidth,
                Height = panelHeight,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(3),
                BackColor = ThemeManager.GetBackColor(ThemeManager.IsDarkTheme)
            };

            PictureBox pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 30,
                Height = 30,
                Left = 5,
                Top = (panelHeight - 30) / 2
            };

            if (!string.IsNullOrEmpty(imgUrl))
            {
                try
                {
                    pictureBox.Load(imgUrl);
                }
                catch
                {
                    pictureBox.Image = null;
                }
            }

            Label lblNome = new Label
            {
                Text = nomeLeggibile,
                AutoSize = false,
                Width = panelWidth - 80,
                Height = 30,
                Left = 40,
                Top = (panelHeight - 30) / 2,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = ThemeManager.GetForeColor(ThemeManager.IsDarkTheme),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Padding = new Padding(0, 5, 0, 0)
            };

            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(lblNome, nomeTecnico);

            CheckBox checkBox = new CheckBox
            {
                AutoSize = true,
                Left = panelWidth - 25,
                Top = (panelHeight - 15) / 2
            };

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(lblNome);
            panel.Controls.Add(checkBox);
            flowLayoutPanel1.Controls.Add(panel);
        }

        private string OttieniNomeLeggibile(string nomeTecnico)
        {
            if (appNameMappings.ContainsKey(nomeTecnico))
            {
                return appNameMappings[nomeTecnico];
            }
            return nomeTecnico.Replace("Microsoft.", "").Replace("_", " ");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string filtro = textBox1.Text.Trim().ToLower();
                List<string> risultati = appxNames
                    .Where(app => OttieniNomeLeggibile(app).ToLower().Contains(filtro))
                    .ToList();

                AggiornaUI(risultati);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.ForeColor = Color.Gray;
                textBox1.SelectionStart = 0;
                AggiornaUI(appxNames);
            }
        }
        private void btnAvviaSelezionatiDebloat_Click(object sender, EventArgs e)
        {
            totalSteps = 0;
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is Panel panel)
                {
                    CheckBox checkBox = panel.Controls.OfType<CheckBox>().FirstOrDefault();
                    Label lblNome = panel.Controls.OfType<Label>().FirstOrDefault();
                    if (checkBox != null && checkBox.Checked && lblNome != null)
                    {
                        totalSteps++;
                    }
                }
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

        private void RimuoviApp(string nomeApp)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = $"-Command \"Get-AppxPackage -allusers {nomeApp} | Remove-AppxPackage\""
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    _ = process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RimuoviProvisioning(string nomeApp)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = $"-Command \"Get-AppxProvisionedPackage -Online | Where-Object {{ $_.DisplayName -like '*{nomeApp}*' }} | Remove-AppxProvisionedPackage -Online\""
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    _ = process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error: {nomeApp}: {ex.Message}", "WinHubX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDebloatAuto_Click(object sender, EventArgs e)
        {
            string messaggionbox = LanguageManager.GetTranslation("FormDebloat", "confermaEsecuzione");
            string titolo = LanguageManager.GetTranslation("FormDebloat", "titoloConferma");

            DialogResult result = MessageBox.Show(messaggionbox,
                                                  titolo,
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string powerShellCommand = GetPowerShellCommand();
                if (powerShellCommand != null)
                {
                    ExecutePowerShellCommand(powerShellCommand);
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

        private string? GetPowerShellCommand()
        {
            int version = Environment.OSVersion.Version.Major;
            if (version < 10)
            {
                return null;
            }

            string[] commonAppsToRemove = {
        "Microsoft.VP9VideoExtensions", "Microsoft.WebMediaExtensions",
        "Microsoft.WebpImageExtension", "Microsoft.Windows.ShellExperienceHost",
        "Microsoft.VCLibs*",
        "Microsoft.WindowsStore", "Microsoft.XboxIdentityProvider", "Microsoft.HEIFImageExtension",
        "Microsoft.UI.Xaml*"
    };

            string notepad = version == 11 ? "| Notepad" : "";

            string command = $"$ErrorActionPreference = 'SilentlyContinue'; Get-AppxPackage -AllUsers | Where-Object {{$_.name -notmatch '{string.Join("|", commonAppsToRemove)}'{notepad}}} | Remove-AppxPackage";

            return command;
        }

        private void ExecutePowerShellCommand(string command)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-command \"{command}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas"
            };

            using (var process = Process.Start(processStartInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();
                    _ = process.StandardOutput.ReadToEnd();
                    _ = process.StandardError.ReadToEnd();
                }
            }
        }
        private void btnDebloatAvanzato_Click(object sender, EventArgs e)
        {
            FormDebloatAvanzato formDebloat = new FormDebloatAvanzato();
            formDebloat.Show();
        }

        private void btnServizi_Click(object sender, EventArgs e)
        {
            FormServizi formServizi = new FormServizi();
            formServizi.Show();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int currentStep = 0;
            List<string> appsToRemove = new List<string>();

            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is Panel panel)
                {
                    CheckBox checkBox = panel.Controls.OfType<CheckBox>().FirstOrDefault();
                    Label lblNome = panel.Controls.OfType<Label>().FirstOrDefault();

                    if (checkBox != null && checkBox.Checked && lblNome != null)
                    {
                        string nomeTecnico = appxNames.FirstOrDefault(app => OttieniNomeLeggibile(app) == lblNome.Text);
                        if (!string.IsNullOrEmpty(nomeTecnico))
                        {
                            appsToRemove.Add(nomeTecnico);

                        }
                    }
                }
            }

            foreach (string app in appsToRemove)
            {
                RimuoviApp(app);
                RimuoviProvisioning(app);
                currentStep++;
                backgroundWorker1.ReportProgress(currentStep);
            }
            CaricaAppxPackages();
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
