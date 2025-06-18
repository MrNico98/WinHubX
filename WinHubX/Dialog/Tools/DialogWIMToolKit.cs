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

            using (WebClient client = new WebClient())
            {
                try
                {
                    // Scarica il JSON di configurazione
                    string json = client.DownloadString(configUrl);

                    // Analizza il JSON per ottenere l'URL di WimToolkit
                    JObject configData = JObject.Parse(json);
                    string wimToolkitUrl = configData["Dialog"]["WimToolkit"].ToString();

                    // Scarica lo script dal URL di WimToolkit
                    string script = client.DownloadString(wimToolkitUrl);

                    // Salva lo script in un file temporaneo
                    string tempScriptPath = Path.GetTempPath() + "WIMtoolkitDowload.ps1";
                    File.WriteAllText(tempScriptPath, script);

                    // Esegui lo script PowerShell in modo asincrono
                    await Task.Run(() => ExecutePowerShellScript(tempScriptPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore durante il download o l'esecuzione dello script: " + ex.Message);
                }
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
                MessageBox.Show("Errore durante l'esecuzione dello script PowerShell: " + ex.Message);
            }
        }
    }

}
