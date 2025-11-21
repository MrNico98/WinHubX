namespace WinHubX.Forms.Personalizzazione_office
{
    public partial class AppItem : UserControl
    {
        public AppItem()
        {
            InitializeComponent();
            foreach (Control c in Controls)
                c.Click += (s, e) => OnClick(e); // Propaga il click
        }

        public void SetApp(Image icon, string name)
        {
            pictureBoxApp.Image = icon;
            lblName.Text = name;
        }
    }
}
