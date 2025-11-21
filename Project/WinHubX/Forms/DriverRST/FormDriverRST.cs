using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;
using System.Security.Policy;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.DriverRST
{
    public partial class FormDriverRST : Form
    {
        public FormDriverRST()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
        }

        private async void btnDriverRSTPrinci_Click(object sender, EventArgs e)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Seleziona dove vuoi salvare il driver RST";
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string destinationFolder = folderDialog.SelectedPath;
                        string downloadUrl = await GetDriverRSTDownloadUrl();

                        if (!string.IsNullOrEmpty(downloadUrl))
                        {
                            await DownloadAndExtractDriverRST(downloadUrl, destinationFolder);
                            MessageBox.Show("Driver RST scaricato ed estratto con successo!", "Successo",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Impossibile trovare l'URL di download del driver RST", "Errore",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il download: {ex.Message}", "Errore",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<string> GetDriverRSTDownloadUrl()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string jsonContent = await httpClient.GetStringAsync(Dipendenze.GitHubConfigUrl);
                    var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);

                    return jsonObject["Dialog"]?["DriverRST"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel recupero dell'URL: {ex.Message}", "Errore",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private async Task DownloadAndExtractDriverRST(string downloadUrl, string destinationFolder)
        {
            string tempZipPath = Path.Combine(Path.GetTempPath(), "DriverRST.zip");

            try
            {
                using (var httpClient = new HttpClient())
                {
                    byte[] fileData = await httpClient.GetByteArrayAsync(downloadUrl);
                    await File.WriteAllBytesAsync(tempZipPath, fileData);
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(tempZipPath, destinationFolder, true);
            }
            finally
            {
                if (File.Exists(tempZipPath))
                {
                    File.Delete(tempZipPath);
                }
            }
        }
    }
}