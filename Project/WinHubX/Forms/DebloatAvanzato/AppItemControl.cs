using WinHubX.Forms.Base;
using WinHubX.Impostazioni;

namespace WinHubX.Forms.DebloatAvanzato
{
    public partial class AppItemControl : UserControl
    {
        public string NomeTecnico { get; private set; }
        public string? ImgUrl { get; private set; }

        // ✅ Aggiungi questa proprietà pubblica
        public bool IsSelected => checkBox.Checked;

        public AppItemControl()
        {
            InitializeComponent();
        }

        // ✅ Nuovo costruttore con parametri
        public AppItemControl(string nomeTecnico, string? imgUrl = null) : this()
        {
            NomeTecnico = nomeTecnico;
            ImgUrl = imgUrl;
            InizializzaControllo();
        }

        private void InizializzaControllo()
        {
            // Label
            lblNome.Text = OttieniNomeLeggibile(NomeTecnico);

            // Carica immagine se presente
            if (!string.IsNullOrEmpty(ImgUrl))
            {
                try
                {
                    pictureBox.Load(ImgUrl);
                }
                catch
                {
                    pictureBox.Image = null;
                }
            }

            // Tooltip
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(lblNome, NomeTecnico);

            // Tema
            BackColor = ThemeManager.GetBackColor(ThemeManager.IsDarkTheme);
            lblNome.ForeColor = ThemeManager.GetForeColor(ThemeManager.IsDarkTheme);
        }

        private string OttieniNomeLeggibile(string nomeTecnico)
        {
            if (FormDebloat.appNameMappings.ContainsKey(nomeTecnico))
                return FormDebloat.appNameMappings[nomeTecnico];

            return nomeTecnico.Replace("Microsoft.", "").Replace("_", " ");
        }
    }
}