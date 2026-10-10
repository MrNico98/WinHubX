using System.ComponentModel;
using System.Diagnostics;
using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.DebloatAvanzato
{
    public partial class AppItemControl : UserControl
    {
        private readonly ToolTip _toolTip = new();
        private readonly IReadOnlyDictionary<string, string> _appNameMappings = new Dictionary<string, string>(StringComparer.Ordinal);

        public string NomeTecnico { get; private set; } = string.Empty;
        public string? ImgUrl { get; private set; }

        // ✅ Aggiungi questa proprietà pubblica
        public bool IsSelected => checkBox.Checked;

        public AppItemControl()
        {
            InitializeComponent();
        }

        // ✅ Nuovo costruttore con parametri
        public AppItemControl(
            string nomeTecnico,
            string? imgUrl = null,
            IReadOnlyDictionary<string, string>? appNameMappings = null) : this()
        {
            NomeTecnico = nomeTecnico;
            ImgUrl = imgUrl;
            if (appNameMappings is not null)
                _appNameMappings = appNameMappings;
            InizializzaControllo();
        }

        private void InizializzaControllo()
        {
            // Label
            lblNome.Text = OttieniNomeLeggibile(NomeTecnico);

            // Carica immagine se presente
            if (Uri.TryCreate(ImgUrl, UriKind.Absolute, out Uri? imageUri)
                && imageUri.Scheme == Uri.UriSchemeHttps)
            {
                try
                {
                    pictureBox.LoadCompleted += PictureBox_LoadCompleted;
                    pictureBox.LoadAsync(imageUri.AbsoluteUri);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    Debug.WriteLine($"Caricamento immagine debloat non avviato: {ex.Message}");
                }
            }

            // Tooltip
            _toolTip.SetToolTip(lblNome, NomeTecnico);

            // Tema
            BackColor = ThemeManager.GetBackColor(ThemeManager.IsDarkTheme);
            lblNome.ForeColor = ThemeManager.GetForeColor(ThemeManager.IsDarkTheme);
        }

        private void PictureBox_LoadCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            if (e.Error is not null)
                Debug.WriteLine($"Caricamento immagine debloat fallito: {e.Error.Message}");
        }

        private string OttieniNomeLeggibile(string nomeTecnico)
        {
            if (_appNameMappings.TryGetValue(nomeTecnico, out string? nomeLeggibile))
                return nomeLeggibile;

            return nomeTecnico.Replace("Microsoft.", "").Replace("_", " ");
        }
    }
}
