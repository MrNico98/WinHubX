using System.Diagnostics;

namespace WinHubX.Dialog
{
    public partial class OfficeDialog : Form
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


        private string dlLink32;
        private string dlLink64;
        private NotifyIcon notifyIcon;
        public OfficeDialog()
        {
            InitializeComponent();
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = false
            };

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


            this.Controls.Add(closeButton);
        }


        public void openDialog(Label lblOffice, string link32, string link64)
        {
            // Imposta il link di download per la versione a 32 bit
            this.dlLink32 = link32;
            this.dlLink64 = link64;

            // Crea e configura le etichette e i pulsanti come prima
            Label infoLabel = new Label();
            infoLabel.Image = lblOffice.Image;
            infoLabel.Text = lblOffice.Text;
            infoLabel.Font = lblOffice.Font;
            infoLabel.Size = new Size(211, 110);
            infoLabel.Location = new Point(50, 70);
            infoLabel.ForeColor = lblOffice.ForeColor;
            infoLabel.BackColor = lblOffice.BackColor;
            infoLabel.TextAlign = ContentAlignment.MiddleRight;
            infoLabel.ImageAlign = ContentAlignment.MiddleLeft;

            this.Controls.Add(infoLabel);
            infoLabel.BringToFront();
        }

        private void btnOfficeOnline32_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                notifyIcon.BalloonTipTitle = "Nessun codice hash da copiare!";
                notifyIcon.BalloonTipText = "Il codice hash è disponibile solo per le versioni offline.";
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = dlLink32,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore nell'aprire l'URL: {ex.Message}");
                }
            }
        }

        private void btnOfficeOnline64_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                notifyIcon.BalloonTipTitle = "Nessun codice hash da copiare!";
                notifyIcon.BalloonTipText = "Il codice hash è disponibile solo per le versioni offline.";
                notifyIcon.ShowBalloonTip(1000);
            }
            else if (e.Button == MouseButtons.Left)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = dlLink64,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore nell'aprire l'URL: {ex.Message}");
                }
            }
        }
    }
}