using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;

namespace WinHubX.Dialog.Tools
{
    public partial class DialogWIMToolKit : Form
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
        public DialogWIMToolKit()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            LanguageManager.LoadTranslations();
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            // Imposta il protocollo di sicurezza su TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // URL del JSON di configurazione online
            string configUrl = "https://aimodsitalia.store/ConfigWinHubX/configWinHubX.json";

            try
            {
                // Crea un'istanza di HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Scarica il JSON di configurazione
                    string json = await client.GetStringAsync(configUrl);

                    // Analizza il JSON per ottenere l'URL di WimToolkit
                    JObject configData = JObject.Parse(json);
                    string? wimToolkitUrl = configData["Dialog"]["WimToolkit"]?.ToString();

                    // Verifica se il link è presente
                    if (string.IsNullOrEmpty(wimToolkitUrl))
                    {
                        _ = MessageBox.Show("WimToolkit URL non trovato.");
                        return;
                    }

                    // Scarica lo script dal URL di WimToolkit
                    string script = await client.GetStringAsync(wimToolkitUrl);

                    // Salva lo script in un file temporaneo
                    string tempScriptPath = Path.Combine(Path.GetTempPath(), "WIMtoolkitDownload.ps1");
                    File.WriteAllText(tempScriptPath, script);

                    // Esegui lo script PowerShell in modo asincrono
                    await Task.Run(() => ExecutePowerShellScript(tempScriptPath));
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ExecutePowerShellScript(string scriptPath)
        {
            // Crea un nuovo processo per eseguire PowerShell
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"", // Usa Bypass per eseguire script non firmati
                UseShellExecute = true, // Rende visibile la finestra di PowerShell
                CreateNoWindow = false // Non crea una finestra nascosta
            };

            try
            {
                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit(); // Aspetta che il processo finisca
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error:" + ex.Message);
            }
        }
    }

}
