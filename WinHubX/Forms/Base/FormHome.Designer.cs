namespace WinHubX
{
    partial class FormHome
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHome));
            imgHomeLogo = new PictureBox();
            btnKofi = new Button();
            tgWinHubX = new Button();
            label3 = new Label();
            lblInfoOffice2024 = new Label();
            label1 = new Label();
            lblInfoWinAIO64 = new Label();
            versioneapp = new Label();
            ((System.ComponentModel.ISupportInitialize)imgHomeLogo).BeginInit();
            SuspendLayout();
            // 
            // imgHomeLogo
            // 
            resources.ApplyResources(imgHomeLogo, "imgHomeLogo");
            imgHomeLogo.Image = Properties.Resources.homeLogo;
            imgHomeLogo.Name = "imgHomeLogo";
            imgHomeLogo.TabStop = false;
            // 
            // btnKofi
            // 
            resources.ApplyResources(btnKofi, "btnKofi");
            btnKofi.Cursor = Cursors.Hand;
            btnKofi.FlatAppearance.BorderSize = 0;
            btnKofi.ForeColor = Color.Coral;
            btnKofi.Image = Properties.Resources.pngCoffee;
            btnKofi.Name = "btnKofi";
            btnKofi.UseVisualStyleBackColor = true;
            btnKofi.Click += btnKofi_Click;
            // 
            // tgWinHubX
            // 
            resources.ApplyResources(tgWinHubX, "tgWinHubX");
            tgWinHubX.Cursor = Cursors.Hand;
            tgWinHubX.FlatAppearance.BorderSize = 0;
            tgWinHubX.ForeColor = Color.FromArgb(37, 38, 39);
            tgWinHubX.Image = Properties.Resources.pngTelegram;
            tgWinHubX.Name = "tgWinHubX";
            tgWinHubX.UseVisualStyleBackColor = true;
            tgWinHubX.Click += tgWinHubX_Click;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.Coral;
            label3.Name = "label3";
            // 
            // lblInfoOffice2024
            // 
            resources.ApplyResources(lblInfoOffice2024, "lblInfoOffice2024");
            lblInfoOffice2024.ForeColor = Color.Coral;
            lblInfoOffice2024.Name = "lblInfoOffice2024";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.Coral;
            label1.Name = "label1";
            // 
            // lblInfoWinAIO64
            // 
            resources.ApplyResources(lblInfoWinAIO64, "lblInfoWinAIO64");
            lblInfoWinAIO64.ForeColor = Color.White;
            lblInfoWinAIO64.Name = "lblInfoWinAIO64";
            // 
            // versioneapp
            // 
            resources.ApplyResources(versioneapp, "versioneapp");
            versioneapp.ForeColor = Color.Coral;
            versioneapp.Name = "versioneapp";
            // 
            // FormHome
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(versioneapp);
            Controls.Add(label1);
            Controls.Add(lblInfoOffice2024);
            Controls.Add(tgWinHubX);
            Controls.Add(label3);
            Controls.Add(btnKofi);
            Controls.Add(lblInfoWinAIO64);
            Controls.Add(imgHomeLogo);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormHome";
            ((System.ComponentModel.ISupportInitialize)imgHomeLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox imgHomeLogo;
        private Button btnKofi;
        private Button tgWinHubX;
        private Label label3;
        private Label lblInfoOffice2024;
        private Label label1;
        private Label lblInfoWinAIO64;
        public Label versioneapp;
    }
}