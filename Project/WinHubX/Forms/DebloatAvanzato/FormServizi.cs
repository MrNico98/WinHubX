using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.DebloatAvanzato;

public partial class FormServizi : Form
{
    private static bool IsExpectedServiceOperationFailure(Exception exception) =>
        exception is IOException
            or UnauthorizedAccessException
            or System.Security.SecurityException
            or System.ComponentModel.Win32Exception
            or System.Runtime.InteropServices.COMException
            or HttpRequestException
            or System.Text.Json.JsonException
            or Newtonsoft.Json.JsonException
            or InvalidOperationException
            or OperationCanceledException
            or TimeoutException
            or ArgumentException;

    private static readonly HttpClient HttpClient = CreateHttpClient();
    private const string ServicesUrl =
        "https://raw.githubusercontent.com/LightYagami28/WinHubX-Resource/refs/heads/main/servizi.json";

    private ServiziRoot? _serviceCatalog;

    public FormServizi()
    {
        LanguageManager.LoadTranslations();
        InitializeComponent();
        ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            AutomaticDecompression = System.Net.DecompressionMethods.All
        };

        return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
    }

    private async void FormServizi_Load(object? sender, EventArgs e)
    {
        try
        {
            string json = await TrustedHttpsClient.GetStringAsync(HttpClient, ServicesUrl);
            ServiziRoot catalog = JsonConvert.DeserializeObject<ServiziRoot>(json)
                ?? throw new InvalidDataException("Configurazione servizi non valida.");
            ServiceConfigurationValidator.ValidateCatalog(catalog);

            DisabilitaServizi.BeginUpdate();
            try
            {
                int firstAddedIndex = DisabilitaServizi.Items.Count;
                DisabilitaServizi.Items.AddRange(catalog.service
                    .Select(static service => (object)service.Name)
                    .ToArray());
                for (int index = firstAddedIndex; index < DisabilitaServizi.Items.Count; index++)
                    DisabilitaServizi.SetItemChecked(index, true);
            }
            finally
            {
                DisabilitaServizi.EndUpdate();
            }

            _serviceCatalog = catalog;
        }
        catch (Exception ex) when (IsExpectedServiceOperationFailure(ex))
        {
            _ = MessageBox.Show(
                this,
                ex.GetBaseException().Message,
                "WinHubX — servizi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async void ModificaServiziButton_Click(object? sender, EventArgs e)
    {
        if (!button1.Enabled || _serviceCatalog is null)
        {
            return;
        }

        string[] selectedNames = DisabilitaServizi.CheckedItems.Cast<string>().ToArray();
        if (selectedNames.Length == 0)
        {
            return;
        }

        string confirmation = Translate(
            "confirmMessage",
            "Stai per interrompere e modificare l'avvio di {0} servizi Windows con privilegi amministrativi. Potresti interrompere funzioni di Windows o applicazioni. Continuare?",
            "You are about to stop and change the startup configuration of {0} Windows services with administrator privileges. This may interrupt Windows or application features. Continue?");
        if (MessageBox.Show(
                this,
                LanguageManager.Format(confirmation, selectedNames.Length),
                Translate("confirmTitle", "Conferma modifica servizi", "Confirm service changes"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        HashSet<string> selectedNameSet = new(selectedNames, StringComparer.OrdinalIgnoreCase);
        List<Servizio> selectedChanges = _serviceCatalog.service
            .Where(service => selectedNameSet.Contains(service.Name))
            .ToList();
        if (selectedChanges.Count != selectedNames.Length)
        {
            _ = MessageBox.Show(
                this,
                "La selezione non corrisponde al catalogo validato. Ricarica la finestra e riprova.",
                "WinHubX — servizi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        progressBar1.Maximum = selectedChanges.Count;
        progressBar1.Value = 0;
        progressBar1.Style = ProgressBarStyle.Marquee;
        progressBar1.MarqueeAnimationSpeed = 25;
        richTextBox1.Clear();
        button1.Enabled = false;
        DisabilitaServizi.Enabled = false;

        try
        {
            await ApplyServiceChangesAsync(selectedChanges);
        }
        finally
        {
            progressBar1.MarqueeAnimationSpeed = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = 0;
            button1.Enabled = true;
            DisabilitaServizi.Enabled = true;
        }
    }

    private async Task ApplyServiceChangesAsync(IReadOnlyCollection<Servizio> changes)
    {
        string reportPath = Path.Join(Path.GetTempPath(), $"WinHubX-ServiceChanges-{Guid.NewGuid():N}.json");

        try
        {
            using (new FileStream(reportPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
            }

            string encodedCommand = ServiceConfigurationScriptBuilder.BuildEncodedCommand(changes, reportPath);
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Environment.SystemDirectory, "WindowsPowerShell", "v1.0", "powershell.exe"),
                Verb = "runas",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            startInfo.ArgumentList.Add("-NoLogo");
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-EncodedCommand");
            startInfo.ArgumentList.Add(encodedCommand);

            using Process process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Impossibile avviare PowerShell con privilegi amministrativi.");
            await process.WaitForExitAsync();
            progressBar1.Style = ProgressBarStyle.Blocks;

            string reportJson = await File.ReadAllTextAsync(reportPath);
            List<ServiceConfigurationResult> results = System.Text.Json.JsonSerializer.Deserialize<List<ServiceConfigurationResult>>(
                reportJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("Il report delle modifiche ai servizi non è valido.");

            if (results.Count != changes.Count)
            {
                throw new InvalidDataException("Il report non contiene l'esito di tutti i servizi selezionati.");
            }

            foreach (string line in results.Select(result => result.Success
                ? LanguageManager.Format(
                    Translate("serviceSuccess", "Servizio \"{0}\": modificato correttamente", "Service \"{0}\": changed successfully"),
                    result.ServiceName)
                : LanguageManager.Format(
                    Translate("serviceFailure", "Servizio \"{0}\": {1}", "Service \"{0}\": {1}"),
                    result.ServiceName,
                    result.Error ?? "Errore non specificato.")))
            {
                richTextBox1.AppendText(line + Environment.NewLine);
                progressBar1.Value = Math.Min(progressBar1.Value + 1, progressBar1.Maximum);
            }

            if (process.ExitCode != 0)
            {
                richTextBox1.AppendText(LanguageManager.Format(
                    Translate("serviceSummaryFailure", "Alcune modifiche non sono riuscite (codice {0}).", "Some changes failed (exit code {0})."),
                    process.ExitCode) + Environment.NewLine);
            }
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
        {
            richTextBox1.AppendText(Translate(
                "uacCancelled",
                "Operazione annullata nella richiesta UAC.",
                "Operation cancelled at the UAC prompt.") + Environment.NewLine);
        }
        catch (Exception ex) when (IsExpectedServiceOperationFailure(ex))
        {
            Debug.WriteLine(ex);
            richTextBox1.AppendText($"❌ {ex.GetBaseException().Message}{Environment.NewLine}");
        }
        finally
        {
            try
            {
                File.Delete(reportPath);
            }
            catch (Exception ex) when (IsExpectedServiceOperationFailure(ex))
            {
                Debug.WriteLine($"Impossibile eliminare il report temporaneo dei servizi: {ex}");
            }
        }
    }

    private static string Translate(string key, string italianFallback, string englishFallback)
    {
        string translation = LanguageManager.GetTranslation("FormServizi", key);
        if (!string.Equals(translation, key, StringComparison.Ordinal))
        {
            return translation;
        }

        return LanguageManager.CurrentLanguage.StartsWith("it", StringComparison.OrdinalIgnoreCase)
            ? italianFallback
            : englishFallback;
    }
}
