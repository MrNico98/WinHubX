namespace WinHubX.Dialog
{
    public partial class InfoDialog : Form
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

        public InfoDialog()
        {
            InitializeComponent();
            ThemeManager.ApplyThemeToControl(this, ThemeManager.IsDarkTheme);
            LanguageManager.LoadTranslations();
        }

        public InfoDialog(string description)
        {
            this.Size = new(700, 500);
            this.ForeColor = Color.White;
            this.Padding = new Padding(2);
            // Crea il controllo RichTextBox per il paragrafo descrittivo
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Text = description;
            richTextBox.ReadOnly = true;
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox.BackColor = Color.FromArgb(37, 38, 39);
            richTextBox.ForeColor = Color.White;

            // Crea il pulsante per chiudere la finestra di dialogo
            Button closeButton = new Button();
            closeButton.Text = "Chiudi";
            closeButton.Dock = DockStyle.Bottom;
            closeButton.Height = 40; // Imposta l'altezza desiderata
            closeButton.FlatStyle = FlatStyle.Flat; // Rendi il pulsante flat
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.BackColor = Color.Coral;
            closeButton.ForeColor = Color.Black;
            closeButton.Font = new Font("Product Sans", 15f);
            closeButton.Cursor = Cursors.Hand;
            closeButton.Click += (sender, e) => this.Close();

            // Aggiungi i controlli alla finestra di dialogo
            this.Controls.Add(richTextBox);
            this.Controls.Add(closeButton);
        }

    }
}
