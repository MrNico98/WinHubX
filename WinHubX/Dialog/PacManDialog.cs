using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;

namespace WinHubX.Dialog
{
    public partial class PacManDialog : Form
    {

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int borderWidth = 2;

            Color borderColor = Color.Coral;

            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                Rectangle borderRectangle = new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);

                e.Graphics.DrawRectangle(pen, borderRectangle);
            }
        }


        private NotifyIcon notifyIcon;


        public PacManDialog()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            LanguageManager.LoadTranslations();
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };

            Button closeButton = new Button();
            closeButton.Text = "Chiudi";
            closeButton.Dock = DockStyle.Bottom;
            closeButton.Height = 40;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.BackColor = Color.Coral;
            closeButton.ForeColor = Color.Black;
            closeButton.Font = new Font("Product Sans", 15f);
            closeButton.Cursor = Cursors.Hand;
            closeButton.Click += (sender, e) => this.Close();

            this.Controls.Add(closeButton);
        }

        public void openDialog(Label lblPacMan, string link32, string link64)
        {
            Label infoLabel = new Label();
            infoLabel.Image = lblPacMan.Image;
            infoLabel.Text = lblPacMan.Text;
            infoLabel.Font = lblPacMan.Font;
            infoLabel.Size = new Size(211, 110);
            infoLabel.Location = new Point(50, 70);
            infoLabel.ForeColor = lblPacMan.ForeColor;
            infoLabel.BackColor = lblPacMan.BackColor;
            infoLabel.TextAlign = ContentAlignment.MiddleRight;
            infoLabel.ImageAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(infoLabel);
            infoLabel.BringToFront();
        }

        private async void btnInstallaPacMan_Click(object sender, EventArgs e)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string zipFilePath = Path.Combine(desktopPath, "pacman-main.zip");
                string extractPath = desktopPath;
                string sourceDirectory = Path.Combine(desktopPath, "pacman-main", "WSA-pacman-v1.5.0-portable");
                string destinationDirectory = Path.Combine(desktopPath, "PacManWSA");

                if (!Directory.Exists(destinationDirectory))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        // Scarica il JSON
                        string json = await client.GetStringAsync("https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json");

                        JObject jsonObject = JObject.Parse(json);
                        string downloadLink = jsonObject["PacMan"]?["DownloadFile"]?.ToString() ?? throw new Exception("Download link non trovato nel JSON.");

                        // Scarica il file ZIP
                        using (var response = await client.GetAsync(downloadLink))
                        {
                            _ = response.EnsureSuccessStatusCode();
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                    }

                    // Estrai il contenuto ZIP
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath, true);

                    // Sposta nella destinazione finale
                    Directory.Move(sourceDirectory, destinationDirectory);

                    // Pulisci i file temporanei
                    Directory.Delete(Path.Combine(desktopPath, "pacman-main"), true);
                    File.Delete(zipFilePath);

                    // Avvia PacMan
                    _ = Process.Start(Path.Combine(destinationDirectory, "WSA-pacman.exe"));
                }
                else
                {
                    _ = MessageBox.Show("PacMan è presente sul Desktop.");
                    _ = Process.Start(Path.Combine(destinationDirectory, "WSA-pacman.exe"));
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Errore: " + ex.Message);
            }
        }

    }
}
