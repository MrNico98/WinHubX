namespace WinHubX.Forms.Settaggi
{
    partial class FormExplorer
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
            grpFileSettings = new GroupBox();
            chkHideFileExtensions = new CheckBox();
            chkShowHiddenFiles = new CheckBox();
            chkShowSuperHidden = new CheckBox();
            grpViewSettings = new GroupBox();
            chkShowStatusBar = new CheckBox();
            chkShowPreviewPane = new CheckBox();
            chkShowDetailsPane = new CheckBox();
            chkShowFullPath = new CheckBox();
            chkShowEncryptedCompressed = new CheckBox();
            grpStartupSettings = new GroupBox();
            chkStartWithThisPC = new CheckBox();
            chkShowRibbon = new CheckBox();
            btnApply = new Button();
            btnReset = new Button();
            btnClose = new Button();
            lblStatus = new Label();
            grpFileSettings.SuspendLayout();
            grpViewSettings.SuspendLayout();
            grpStartupSettings.SuspendLayout();
            SuspendLayout();
            // 
            // grpFileSettings
            // 
            grpFileSettings.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            grpFileSettings.BackColor = Color.FromArgb(37, 38, 39);
            grpFileSettings.Controls.Add(chkHideFileExtensions);
            grpFileSettings.Controls.Add(chkShowHiddenFiles);
            grpFileSettings.Controls.Add(chkShowSuperHidden);
            grpFileSettings.ForeColor = Color.White;
            grpFileSettings.Location = new Point(20, 20);
            grpFileSettings.Name = "grpFileSettings";
            grpFileSettings.Size = new Size(632, 120);
            grpFileSettings.TabIndex = 0;
            grpFileSettings.TabStop = false;
            grpFileSettings.Text = "Impostazioni Visualizzazione File";
            // 
            // chkHideFileExtensions
            // 
            chkHideFileExtensions.Location = new Point(20, 25);
            chkHideFileExtensions.Name = "chkHideFileExtensions";
            chkHideFileExtensions.Size = new Size(250, 20);
            chkHideFileExtensions.TabIndex = 0;
            chkHideFileExtensions.Text = "Nascondi le estensioni dei file";
            // 
            // chkShowHiddenFiles
            // 
            chkShowHiddenFiles.Location = new Point(20, 50);
            chkShowHiddenFiles.Name = "chkShowHiddenFiles";
            chkShowHiddenFiles.Size = new Size(250, 20);
            chkShowHiddenFiles.TabIndex = 1;
            chkShowHiddenFiles.Text = "Mostra i file nascosti";
            // 
            // chkShowSuperHidden
            // 
            chkShowSuperHidden.Location = new Point(20, 75);
            chkShowSuperHidden.Name = "chkShowSuperHidden";
            chkShowSuperHidden.Size = new Size(300, 20);
            chkShowSuperHidden.TabIndex = 2;
            chkShowSuperHidden.Text = "Mostra i file di sistema protetti";
            // 
            // grpViewSettings
            // 
            grpViewSettings.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            grpViewSettings.BackColor = Color.FromArgb(37, 38, 39);
            grpViewSettings.Controls.Add(chkShowStatusBar);
            grpViewSettings.Controls.Add(chkShowPreviewPane);
            grpViewSettings.Controls.Add(chkShowDetailsPane);
            grpViewSettings.Controls.Add(chkShowFullPath);
            grpViewSettings.Controls.Add(chkShowEncryptedCompressed);
            grpViewSettings.ForeColor = Color.White;
            grpViewSettings.Location = new Point(20, 160);
            grpViewSettings.Name = "grpViewSettings";
            grpViewSettings.Size = new Size(632, 160);
            grpViewSettings.TabIndex = 1;
            grpViewSettings.TabStop = false;
            grpViewSettings.Text = "Impostazioni Visualizzazione";
            // 
            // chkShowStatusBar
            // 
            chkShowStatusBar.Location = new Point(20, 25);
            chkShowStatusBar.Name = "chkShowStatusBar";
            chkShowStatusBar.Size = new Size(250, 20);
            chkShowStatusBar.TabIndex = 0;
            chkShowStatusBar.Text = "Mostra la barra di stato";
            // 
            // chkShowPreviewPane
            // 
            chkShowPreviewPane.Location = new Point(20, 50);
            chkShowPreviewPane.Name = "chkShowPreviewPane";
            chkShowPreviewPane.Size = new Size(250, 20);
            chkShowPreviewPane.TabIndex = 1;
            chkShowPreviewPane.Text = "Mostra il riquadro di anteprima";
            // 
            // chkShowDetailsPane
            // 
            chkShowDetailsPane.Location = new Point(20, 75);
            chkShowDetailsPane.Name = "chkShowDetailsPane";
            chkShowDetailsPane.Size = new Size(250, 20);
            chkShowDetailsPane.TabIndex = 2;
            chkShowDetailsPane.Text = "Mostra il riquadro dei dettagli";
            // 
            // chkShowFullPath
            // 
            chkShowFullPath.Location = new Point(20, 100);
            chkShowFullPath.Name = "chkShowFullPath";
            chkShowFullPath.Size = new Size(250, 20);
            chkShowFullPath.TabIndex = 3;
            chkShowFullPath.Text = "Mostra il percorso completo nella barra del titolo";
            // 
            // chkShowEncryptedCompressed
            // 
            chkShowEncryptedCompressed.Location = new Point(20, 125);
            chkShowEncryptedCompressed.Name = "chkShowEncryptedCompressed";
            chkShowEncryptedCompressed.Size = new Size(300, 20);
            chkShowEncryptedCompressed.TabIndex = 4;
            chkShowEncryptedCompressed.Text = "Mostra i file criptati/compressi a colori";
            // 
            // grpStartupSettings
            // 
            grpStartupSettings.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            grpStartupSettings.BackColor = Color.FromArgb(37, 38, 39);
            grpStartupSettings.Controls.Add(chkStartWithThisPC);
            grpStartupSettings.Controls.Add(chkShowRibbon);
            grpStartupSettings.ForeColor = Color.White;
            grpStartupSettings.Location = new Point(20, 340);
            grpStartupSettings.Name = "grpStartupSettings";
            grpStartupSettings.Size = new Size(632, 62);
            grpStartupSettings.TabIndex = 2;
            grpStartupSettings.TabStop = false;
            grpStartupSettings.Text = "Impostazioni Avvio";
            // 
            // chkStartWithThisPC
            // 
            chkStartWithThisPC.Location = new Point(20, 25);
            chkStartWithThisPC.Name = "chkStartWithThisPC";
            chkStartWithThisPC.Size = new Size(250, 20);
            chkStartWithThisPC.TabIndex = 0;
            chkStartWithThisPC.Text = "Apri Esplora File su 'Questo PC'";
            // 
            // chkShowRibbon
            // 
            chkShowRibbon.Location = new Point(330, 25);
            chkShowRibbon.Name = "chkShowRibbon";
            chkShowRibbon.Size = new Size(280, 20);
            chkShowRibbon.TabIndex = 1;
            chkShowRibbon.Text = "Mostra la barra multifunzione per impostazione predefinita";
            // 
            // btnApply
            // 
            btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnApply.BackColor = Color.FromArgb(0, 120, 215);
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.ForeColor = Color.White;
            btnApply.Location = new Point(431, 458);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(100, 30);
            btnApply.TabIndex = 3;
            btnApply.Text = "Applica Impostazioni";
            btnApply.UseVisualStyleBackColor = false;
            btnApply.Click += BtnApply_Click;
            // 
            // btnReset
            // 
            btnReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnReset.BackColor = Color.FromArgb(200, 200, 200);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Location = new Point(541, 458);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(100, 30);
            btnReset.TabIndex = 4;
            btnReset.Text = "Ripristina Predefiniti";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += BtnReset_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(200, 200, 200);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(321, 458);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 30);
            btnClose.TabIndex = 5;
            btnClose.Text = "Chiudi";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += BtnClose_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.ForeColor = Color.Gainsboro;
            lblStatus.Location = new Point(20, 417);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(632, 20);
            lblStatus.TabIndex = 6;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FormExplorer
            // 
            BackColor = Color.FromArgb(37, 38, 39);
            ClientSize = new Size(664, 500);
            Controls.Add(grpFileSettings);
            Controls.Add(grpViewSettings);
            Controls.Add(grpStartupSettings);
            Controls.Add(btnApply);
            Controls.Add(btnReset);
            Controls.Add(btnClose);
            Controls.Add(lblStatus);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormExplorer";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Impostazioni Esplora File";
            grpFileSettings.ResumeLayout(false);
            grpViewSettings.ResumeLayout(false);
            grpStartupSettings.ResumeLayout(false);
            ResumeLayout(false);
        }

        private CheckBox chkHideFileExtensions;
        private CheckBox chkShowHiddenFiles;
        private CheckBox chkStartWithThisPC;
        private CheckBox chkShowSuperHidden;
        private CheckBox chkShowStatusBar;
        private CheckBox chkShowPreviewPane;
        private CheckBox chkShowDetailsPane;
        private CheckBox chkShowFullPath;
        private CheckBox chkShowEncryptedCompressed;
        private CheckBox chkShowRibbon;

        // Buttons
        private Button btnApply;
        private Button btnReset;
        private Button btnClose;

        // Group boxes
        private GroupBox grpFileSettings;
        private GroupBox grpViewSettings;
        private GroupBox grpStartupSettings;

        // Status label
        private Label lblStatus;

        #endregion
    }
}