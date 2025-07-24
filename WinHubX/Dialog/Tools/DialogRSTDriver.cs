namespace WinHubX.Dialog.Tools
{
    public partial class DialogRSTDriver : Form
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

        private SaveFileDialog saveFileDialog;

        public DialogRSTDriver()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            LanguageManager.LoadTranslations();
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Zip Files (*.zip)|*.zip";
            saveFileDialog.Title = "Salva Driver RST";
            saveFileDialog.FileName = "DriverRST.zip";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            string jsonUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
            string? driverRstUrl = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Scarica il JSON
                    string response = await client.GetStringAsync(jsonUrl);

                    // Parse JSON
                    var json = Newtonsoft.Json.Linq.JObject.Parse(response);
                    driverRstUrl = json["Dialog"]?["DriverRST"]?.ToString();

                    if (string.IsNullOrWhiteSpace(driverRstUrl))
                    {
                        _ = MessageBox.Show("Link non trovato nella sezione 'DriverRST'.");
                        return;
                    }
                }

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = "DriverRST.zip";
                    saveFileDialog.Filter = "ZIP Files (*.zip)|*.zip";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string destPath = saveFileDialog.FileName;

                        using (HttpClient client = new HttpClient())
                        using (var response = await client.GetAsync(driverRstUrl))
                        {
                            _ = response.EnsureSuccessStatusCode();

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = File.OpenWrite(destPath))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }

                        _ = MessageBox.Show("Download completato.");
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Errore: {ex.Message}");
            }
        }

    }
}
