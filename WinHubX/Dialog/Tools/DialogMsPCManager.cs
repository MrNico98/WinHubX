
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;

namespace WinHubX.Dialog.Tools
{
    public partial class DialogMsPCManager : Form
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

        public DialogMsPCManager()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            LanguageManager.LoadTranslations();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string configUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";
            string tempFilePath = Path.Combine(Path.GetTempPath(), "microsoft-pc-manager.msixbundle");

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Scarica il JSON di configurazione
                    string json = await client.GetStringAsync(configUrl);

                    // Analizza il JSON per ottenere l'URL di Microsoft PC Manager
                    JObject configData = JObject.Parse(json);
                    string pcManagerUrl = configData["Dialog"]?["managersetupmicrosoft"]?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(pcManagerUrl))
                        throw new Exception("URL non trovato nel JSON!");

                    // Scarica il file msixbundle
                    byte[] fileBytes = await client.GetByteArrayAsync(pcManagerUrl);
                    await File.WriteAllBytesAsync(tempFilePath, fileBytes);

                    // Esegui l'installazione del pacchetto msixbundle
                    using (Process installProcess = new Process())
                    {
                        installProcess.StartInfo.FileName = "powershell";
                        installProcess.StartInfo.Arguments = $"-Command \"Add-AppxPackage -Path '{tempFilePath}'\"";
                        installProcess.StartInfo.UseShellExecute = false;
                        installProcess.StartInfo.CreateNoWindow = true;
                        _ = installProcess.Start();
                        await installProcess.WaitForExitAsync();
                    }

                    _ = MessageBox.Show("Installazione completata!");
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Errore: {ex.Message}");
                }
            }
        }

    }
}
