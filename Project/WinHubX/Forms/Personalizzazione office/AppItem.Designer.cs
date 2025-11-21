namespace WinHubX.Forms.Personalizzazione_office
{
    partial class AppItem
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private PictureBox pictureBoxApp;
        private Label lblName;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
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
            pictureBoxApp = new PictureBox();
            lblName = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxApp).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxApp
            // 
            pictureBoxApp.Location = new Point(22, 5);
            pictureBoxApp.Name = "pictureBoxApp";
            pictureBoxApp.Size = new Size(56, 40);
            pictureBoxApp.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxApp.TabIndex = 0;
            pictureBoxApp.TabStop = false;
            // 
            // lblName
            // 
            lblName.Dock = DockStyle.Bottom;
            lblName.Font = new Font("Segoe UI", 9F);
            lblName.Location = new Point(0, 50);
            lblName.Name = "lblName";
            lblName.Size = new Size(102, 22);
            lblName.TabIndex = 1;
            lblName.Text = "Nome";
            lblName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AppItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(pictureBoxApp);
            Controls.Add(lblName);
            Margin = new Padding(10);
            Name = "AppItem";
            Size = new Size(102, 72);
            ((System.ComponentModel.ISupportInitialize)pictureBoxApp).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
