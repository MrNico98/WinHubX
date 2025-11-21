namespace WinHubX.Forms.InstallaComponenti
{
    partial class FormInstallaComponenti
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInstallaComponenti));
            label3 = new Label();
            btnInstallaVerdi = new CuoreUI.Controls.cuiButton();
            pictureBoxPowerPoint = new PictureBox();
            pictureBoxExcel = new PictureBox();
            pictureBoxWord = new PictureBox();
            checkBox_winget = new CheckBox();
            checkBox_microsoftdefender = new CheckBox();
            checkBox_MicrosoftStore = new CheckBox();
            panel27 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxExcel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxWord).BeginInit();
            panel27.SuspendLayout();
            SuspendLayout();
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.FromArgb(0, 126, 249);
            label3.Name = "label3";
            // 
            // btnInstallaVerdi
            // 
            resources.ApplyResources(btnInstallaVerdi, "btnInstallaVerdi");
            btnInstallaVerdi.CheckButton = false;
            btnInstallaVerdi.Checked = false;
            btnInstallaVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btnInstallaVerdi.Content = "  Installa";
            btnInstallaVerdi.DialogResult = DialogResult.None;
            btnInstallaVerdi.ForeColor = Color.White;
            btnInstallaVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.HoverForeColor = Color.White;
            btnInstallaVerdi.HoverImageTint = Color.White;
            btnInstallaVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.Image = Properties.Resources.pngCheckCreaISO;
            btnInstallaVerdi.ImageAutoCenter = true;
            btnInstallaVerdi.ImageExpand = new Point(0, 0);
            btnInstallaVerdi.ImageOffset = new Point(0, 0);
            btnInstallaVerdi.Name = "btnInstallaVerdi";
            btnInstallaVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnInstallaVerdi.NormalForeColor = Color.White;
            btnInstallaVerdi.NormalImageTint = Color.White;
            btnInstallaVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.OutlineThickness = 1F;
            btnInstallaVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.PressedForeColor = Color.Black;
            btnInstallaVerdi.PressedImageTint = Color.Black;
            btnInstallaVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btnInstallaVerdi.Rounding = new Padding(8);
            btnInstallaVerdi.TextAlignment = StringAlignment.Center;
            btnInstallaVerdi.TextOffset = new Point(0, 0);
            btnInstallaVerdi.Click += btnInstalla_Click;
            // 
            // pictureBoxPowerPoint
            // 
            pictureBoxPowerPoint.Cursor = Cursors.Hand;
            pictureBoxPowerPoint.Image = Properties.Resources.Windows_Package_Manager_logo;
            resources.ApplyResources(pictureBoxPowerPoint, "pictureBoxPowerPoint");
            pictureBoxPowerPoint.Name = "pictureBoxPowerPoint";
            pictureBoxPowerPoint.TabStop = false;
            // 
            // pictureBoxExcel
            // 
            pictureBoxExcel.Cursor = Cursors.Hand;
            pictureBoxExcel.Image = Properties.Resources.pngDefenderWin;
            resources.ApplyResources(pictureBoxExcel, "pictureBoxExcel");
            pictureBoxExcel.Name = "pictureBoxExcel";
            pictureBoxExcel.TabStop = false;
            // 
            // pictureBoxWord
            // 
            pictureBoxWord.Cursor = Cursors.Hand;
            pictureBoxWord.Image = Properties.Resources.Microsoft_Store_logo_dark_svg;
            resources.ApplyResources(pictureBoxWord, "pictureBoxWord");
            pictureBoxWord.Name = "pictureBoxWord";
            pictureBoxWord.TabStop = false;
            // 
            // checkBox_winget
            // 
            resources.ApplyResources(checkBox_winget, "checkBox_winget");
            checkBox_winget.Cursor = Cursors.Hand;
            checkBox_winget.ForeColor = Color.White;
            checkBox_winget.Name = "checkBox_winget";
            checkBox_winget.UseVisualStyleBackColor = true;
            // 
            // checkBox_microsoftdefender
            // 
            resources.ApplyResources(checkBox_microsoftdefender, "checkBox_microsoftdefender");
            checkBox_microsoftdefender.Cursor = Cursors.Hand;
            checkBox_microsoftdefender.ForeColor = Color.White;
            checkBox_microsoftdefender.Name = "checkBox_microsoftdefender";
            checkBox_microsoftdefender.UseVisualStyleBackColor = true;
            // 
            // checkBox_MicrosoftStore
            // 
            resources.ApplyResources(checkBox_MicrosoftStore, "checkBox_MicrosoftStore");
            checkBox_MicrosoftStore.Cursor = Cursors.Hand;
            checkBox_MicrosoftStore.ForeColor = Color.White;
            checkBox_MicrosoftStore.Name = "checkBox_MicrosoftStore";
            checkBox_MicrosoftStore.UseVisualStyleBackColor = true;
            // 
            // panel27
            // 
            panel27.Controls.Add(pictureBoxWord);
            panel27.Controls.Add(pictureBoxPowerPoint);
            panel27.Controls.Add(checkBox_MicrosoftStore);
            panel27.Controls.Add(pictureBoxExcel);
            panel27.Controls.Add(checkBox_microsoftdefender);
            panel27.Controls.Add(checkBox_winget);
            resources.ApplyResources(panel27, "panel27");
            panel27.Name = "panel27";
            // 
            // FormInstallaComponenti
            // 
            BackColor = Color.FromArgb(37, 38, 39);
            resources.ApplyResources(this, "$this");
            Controls.Add(panel27);
            Controls.Add(btnInstallaVerdi);
            Controls.Add(label3);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormInstallaComponenti";
            ((System.ComponentModel.ISupportInitialize)pictureBoxPowerPoint).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxExcel).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxWord).EndInit();
            panel27.ResumeLayout(false);
            panel27.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label3;
        private CuoreUI.Controls.cuiButton btnInstallaVerdi;
        private PictureBox pictureBoxPowerPoint;
        private PictureBox pictureBoxExcel;
        private PictureBox pictureBoxWord;
        private CheckBox checkBox_powerpoint;
        private CheckBox checkBox_winget;
        private CheckBox checkBox_microsoftdefender;
        private CheckBox checkBox_MicrosoftStore;
        private Panel panel27;
    }
}