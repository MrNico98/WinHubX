using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;

namespace WinHubX.Forms.DebloatAvanzato
{
    public partial class FormServizi : Form
    {
        private int totalSteps = 0;
        public FormServizi()
        {
            string savedLanguage = Properties.Settings.Default.Language ?? "it";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(savedLanguage);
            InitializeComponent();
        }

        private async void FormServizi_Load(object sender, EventArgs e)
        {
            string url = "https://raw.githubusercontent.com/AMStore-na/WinHubX-Resource/refs/heads/main/Servizi.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    ServiziRoot serviziRoot = JsonConvert.DeserializeObject<ServiziRoot>(json);

                    for (int i = 0; i < serviziRoot.service.Count; i++)
                    {
                        var servizio = serviziRoot.service[i];
                        DisabilitaServizi.Items.Add(servizio.Name);
                        DisabilitaServizi.SetItemChecked(i, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void ModificaServiziButton_Click(object sender, EventArgs e)
        {
            int totalSteps = DisabilitaServizi.CheckedItems.Count;
            if (totalSteps == 0) totalSteps = 1;

            progressBar1.Maximum = totalSteps;
            progressBar1.Value = 0;
            richTextBox1.Clear();

            await EseguiModificaServiziAsync();
        }

        private async Task EseguiModificaServiziAsync()
        {
            string url = "https://raw.githubusercontent.com/AMStore-na/WinHubX-Resource/refs/heads/main/Servizi.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    ServiziRoot serviziRoot = JsonConvert.DeserializeObject<ServiziRoot>(json);

                    int currentStep = 0;

                    foreach (var checkedItem in DisabilitaServizi.CheckedItems)
                    {
                        string serviceName = checkedItem.ToString();
                        var servizio = serviziRoot.service.FirstOrDefault(s => s.Name == serviceName);

                        if (servizio != null)
                        {
                            string stopComando = $"Get-Service -Name \"{servizio.Name}\" -ErrorAction Stop | Stop-Service";

                            string comando = $"Set-Service -Name \"{servizio.Name}\" -StartupType {servizio.StartupType}";

                            string stato = await Task.Run(() =>
                            {
                                try
                                {
                                    ProcessStartInfo psi = new ProcessStartInfo
                                    {
                                        FileName = "powershell.exe",
                                        Arguments = $"-Command \"{stopComando}; {comando}\"",
                                        Verb = "runas",
                                        UseShellExecute = false,
                                        CreateNoWindow = true,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true
                                    };

                                    using (var proc = Process.Start(psi))
                                    {
                                        string output = proc.StandardOutput.ReadToEnd();
                                        string error = proc.StandardError.ReadToEnd();
                                        proc.WaitForExit();

                                        if (proc.ExitCode == 0)
                                        {
                                            return $"Servizio \"{servizio.Name}\" → ✅ Modificato con successo";
                                        }
                                        else
                                        {
                                            string errorMessage = error.Split(new[] { "\r\n" }, StringSplitOptions.None).FirstOrDefault() ?? error;
                                            return $"❌ Error: {errorMessage}";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return $"❌ Error: {ex.Message}";
                                }
                            });

                            richTextBox1.Invoke((MethodInvoker)(() =>
                            {
                                richTextBox1.AppendText($"{stato}\n");
                                richTextBox1.ScrollToCaret();
                                progressBar1.Value = Math.Min(++currentStep, progressBar1.Maximum);
                            }));
                        }
                    }
                }
            }
            catch (Exception)
            {
                richTextBox1.Invoke((MethodInvoker)(() =>
                {
                    richTextBox1.AppendText($"❌ Error");
                }));
            }
        }

    }

    public class Servizio
    {
        public string Name { get; set; }
        public string StartupType { get; set; }
        public string OriginalType { get; set; }
    }

    public class ServiziRoot
    {
        public List<Servizio> service { get; set; }
    }
}
