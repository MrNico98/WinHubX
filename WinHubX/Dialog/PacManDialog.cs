using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;

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

        private string dlLink32;
        private string dlLink64;
        private NotifyIcon notifyIcon;


        public PacManDialog()
        {
            InitializeComponent();
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

            this.dlLink32 = link32;
            this.dlLink64 = link64;

            this.Controls.Add(infoLabel);
            infoLabel.BringToFront();
        }

        private void btnInstallaPacMan_Click(object sender, EventArgs e)
        {
            try
            {
                // Percorsi per il download e l'estrazione
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string zipFilePath = Path.Combine(desktopPath, "pacman-main.zip");
                string extractPath = desktopPath;
                string sourceDirectory = Path.Combine(desktopPath, "pacman-main", "WSA-pacman-v1.5.0-portable");
                string destinationDirectory = Path.Combine(desktopPath, "PacManWSA");

                if (!Directory.Exists(destinationDirectory))
                {
                    // Scarica il JSON
                    using (WebClient client = new WebClient())
                    {
                        string json = client.DownloadString("https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json");

                        // Deserializza il JSON per ottenere il link di download
                        JObject jsonObject = JObject.Parse(json);
                        string downloadLink = jsonObject["PacMan"]["DownloadFile"].ToString();

                        // Scarica il file zip
                        client.DownloadFile(downloadLink, zipFilePath);
                    }

                    // Estrai e sposta il contenuto
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath, true);
                    Directory.Move(sourceDirectory, destinationDirectory);

                    // Pulisci i file temporanei
                    Directory.Delete(Path.Combine(desktopPath, "pacman-main"), true);
                    File.Delete(zipFilePath);

                    // Avvia l'applicazione
                    Process.Start(Path.Combine(destinationDirectory, "WSA-pacman.exe"));
                }
                else
                {
                    MessageBox.Show("PacMan è presente sul Desktop.");
                    Process.Start(Path.Combine(destinationDirectory, "WSA-pacman.exe"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
