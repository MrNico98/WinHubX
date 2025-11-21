using WinHubX.Impostazioni;

namespace WinHubX.Forms.DebloatAvanzato
{
    partial class AppItemControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        private void InitializeComponent()
        {
            pictureBox = new PictureBox();
            lblNome = new Label();
            checkBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.Anchor = AnchorStyles.Left;
            pictureBox.Location = new Point(34, 12);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(30, 30);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // lblNome
            // 
            lblNome.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblNome.Font = new Font("Arial", 9F, FontStyle.Bold);
            lblNome.ForeColor = Color.Black;
            lblNome.Location = new Point(69, 12);
            lblNome.Name = "lblNome";
            lblNome.Size = new Size(196, 30);
            lblNome.TabIndex = 1;
            lblNome.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // checkBox
            // 
            checkBox.Anchor = AnchorStyles.Left;
            checkBox.AutoSize = true;
            checkBox.Location = new Point(8, 20);
            checkBox.Name = "checkBox";
            checkBox.Size = new Size(15, 14);
            checkBox.TabIndex = 2;
            // 
            // AppItemControl
            // 
            BackColor = Color.White;
            Controls.Add(checkBox);
            Controls.Add(pictureBox);
            Controls.Add(lblNome);
            Name = "AppItemControl";
            Padding = new Padding(5);
            Size = new Size(286, 52);
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public PictureBox pictureBox;
        public Label lblNome;
        public CheckBox checkBox;
    }
}
