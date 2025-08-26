namespace WinHubX
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tableLayoutPanel1 = new TableLayoutPanel();
            panel3 = new Panel();
            pictureBox4 = new PictureBox();
            btnFullScreen = new Button();
            btnMnmz = new Button();
            btnClose = new Button();
            lblPanelTitle = new Label();
            pictureBox2 = new PictureBox();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            pnlNav = new Panel();
            btnReinstallaApp = new Button();
            btnmonitoraggio = new Button();
            btnTools = new Button();
            btnCreaISO = new Button();
            btnDebloat = new Button();
            btnSettaggi = new Button();
            btnOffice = new Button();
            btnWin = new Button();
            btnHome = new Button();
            PnlFormLoader = new Panel();
            tableLayoutPanel1.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.BackColor = Color.FromArgb(64, 60, 59);
            tableLayoutPanel1.Controls.Add(panel3, 1, 0);
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(panel2, 0, 1);
            tableLayoutPanel1.Controls.Add(PnlFormLoader, 1, 1);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel3
            // 
            resources.ApplyResources(panel3, "panel3");
            panel3.BackColor = Color.FromArgb(64, 60, 59);
            panel3.Controls.Add(pictureBox4);
            panel3.Controls.Add(btnFullScreen);
            panel3.Controls.Add(btnMnmz);
            panel3.Controls.Add(btnClose);
            panel3.Controls.Add(lblPanelTitle);
            panel3.Controls.Add(pictureBox2);
            panel3.Name = "panel3";
            // 
            // pictureBox4
            // 
            pictureBox4.Cursor = Cursors.Hand;
            pictureBox4.Image = Properties.Resources.pngConfiguration;
            resources.ApplyResources(pictureBox4, "pictureBox4");
            pictureBox4.Name = "pictureBox4";
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // btnFullScreen
            // 
            resources.ApplyResources(btnFullScreen, "btnFullScreen");
            btnFullScreen.Cursor = Cursors.Hand;
            btnFullScreen.FlatAppearance.BorderSize = 0;
            btnFullScreen.Image = Properties.Resources.pngmaxi;
            btnFullScreen.Name = "btnFullScreen";
            btnFullScreen.UseMnemonic = false;
            btnFullScreen.UseVisualStyleBackColor = true;
            btnFullScreen.Click += btnFullScreen_Click;
            // 
            // btnMnmz
            // 
            resources.ApplyResources(btnMnmz, "btnMnmz");
            btnMnmz.Cursor = Cursors.Hand;
            btnMnmz.FlatAppearance.BorderSize = 0;
            btnMnmz.Image = Properties.Resources.pngMinimize;
            btnMnmz.Name = "btnMnmz";
            btnMnmz.UseMnemonic = false;
            btnMnmz.UseVisualStyleBackColor = true;
            btnMnmz.Click += btnMnmz_Click;
            // 
            // btnClose
            // 
            resources.ApplyResources(btnClose, "btnClose");
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Image = Properties.Resources.pngClose;
            btnClose.Name = "btnClose";
            btnClose.UseMnemonic = false;
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblPanelTitle
            // 
            resources.ApplyResources(lblPanelTitle, "lblPanelTitle");
            lblPanelTitle.ForeColor = Color.White;
            lblPanelTitle.Name = "lblPanelTitle";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.pngXtndLogo_WinHubX;
            resources.ApplyResources(pictureBox2, "pictureBox2");
            pictureBox2.Name = "pictureBox2";
            pictureBox2.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(pictureBox1);
            panel1.Name = "panel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.BackColor = Color.FromArgb(64, 60, 59);
            pictureBox1.Image = Properties.Resources.pngLogoWHX;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(64, 60, 59);
            panel2.Controls.Add(pnlNav);
            panel2.Controls.Add(btnReinstallaApp);
            panel2.Controls.Add(btnmonitoraggio);
            panel2.Controls.Add(btnTools);
            panel2.Controls.Add(btnCreaISO);
            panel2.Controls.Add(btnDebloat);
            panel2.Controls.Add(btnSettaggi);
            panel2.Controls.Add(btnOffice);
            panel2.Controls.Add(btnWin);
            panel2.Controls.Add(btnHome);
            resources.ApplyResources(panel2, "panel2");
            panel2.Name = "panel2";
            // 
            // pnlNav
            // 
            pnlNav.BackColor = Color.FromArgb(0, 126, 249);
            resources.ApplyResources(pnlNav, "pnlNav");
            pnlNav.Name = "pnlNav";
            // 
            // btnReinstallaApp
            // 
            btnReinstallaApp.Cursor = Cursors.Hand;
            resources.ApplyResources(btnReinstallaApp, "btnReinstallaApp");
            btnReinstallaApp.FlatAppearance.BorderSize = 0;
            btnReinstallaApp.ForeColor = SystemColors.Window;
            btnReinstallaApp.Image = Properties.Resources.pngAddApp;
            btnReinstallaApp.Name = "btnReinstallaApp";
            btnReinstallaApp.UseVisualStyleBackColor = true;
            btnReinstallaApp.Click += btnReinstallaApp_Click;
            // 
            // btnmonitoraggio
            // 
            btnmonitoraggio.Cursor = Cursors.Hand;
            resources.ApplyResources(btnmonitoraggio, "btnmonitoraggio");
            btnmonitoraggio.FlatAppearance.BorderSize = 0;
            btnmonitoraggio.ForeColor = SystemColors.Window;
            btnmonitoraggio.Image = Properties.Resources.pngMonitoraggio;
            btnmonitoraggio.Name = "btnmonitoraggio";
            btnmonitoraggio.UseVisualStyleBackColor = true;
            btnmonitoraggio.Click += btnmonitoraggio_Click;
            // 
            // btnTools
            // 
            btnTools.Cursor = Cursors.Hand;
            resources.ApplyResources(btnTools, "btnTools");
            btnTools.FlatAppearance.BorderSize = 0;
            btnTools.ForeColor = SystemColors.Window;
            btnTools.Image = Properties.Resources.pngTools;
            btnTools.Name = "btnTools";
            btnTools.UseVisualStyleBackColor = true;
            btnTools.Click += btnTools_Click;
            // 
            // btnCreaISO
            // 
            btnCreaISO.Cursor = Cursors.Hand;
            resources.ApplyResources(btnCreaISO, "btnCreaISO");
            btnCreaISO.FlatAppearance.BorderSize = 0;
            btnCreaISO.ForeColor = SystemColors.Window;
            btnCreaISO.Image = Properties.Resources.pngCreaISO;
            btnCreaISO.Name = "btnCreaISO";
            btnCreaISO.UseVisualStyleBackColor = true;
            btnCreaISO.Click += btnCreaISO_Click;
            // 
            // btnDebloat
            // 
            btnDebloat.Cursor = Cursors.Hand;
            resources.ApplyResources(btnDebloat, "btnDebloat");
            btnDebloat.FlatAppearance.BorderSize = 0;
            btnDebloat.ForeColor = SystemColors.Window;
            btnDebloat.Image = Properties.Resources.pngprocessi;
            btnDebloat.Name = "btnDebloat";
            btnDebloat.UseVisualStyleBackColor = true;
            btnDebloat.Click += btnDebloat_Click;
            // 
            // btnSettaggi
            // 
            btnSettaggi.Cursor = Cursors.Hand;
            resources.ApplyResources(btnSettaggi, "btnSettaggi");
            btnSettaggi.FlatAppearance.BorderSize = 0;
            btnSettaggi.ForeColor = SystemColors.Window;
            btnSettaggi.Image = Properties.Resources.pngSettaggi;
            btnSettaggi.Name = "btnSettaggi";
            btnSettaggi.UseVisualStyleBackColor = true;
            btnSettaggi.Click += btnSettaggi_Click;
            // 
            // btnOffice
            // 
            btnOffice.Cursor = Cursors.Hand;
            resources.ApplyResources(btnOffice, "btnOffice");
            btnOffice.FlatAppearance.BorderSize = 0;
            btnOffice.ForeColor = SystemColors.Window;
            btnOffice.Image = Properties.Resources.pngOfficeHome;
            btnOffice.Name = "btnOffice";
            btnOffice.UseVisualStyleBackColor = true;
            btnOffice.Click += btnOffice_Click;
            // 
            // btnWin
            // 
            btnWin.Cursor = Cursors.Hand;
            resources.ApplyResources(btnWin, "btnWin");
            btnWin.FlatAppearance.BorderSize = 0;
            btnWin.ForeColor = SystemColors.Window;
            btnWin.Image = Properties.Resources.pngWin;
            btnWin.Name = "btnWin";
            btnWin.UseVisualStyleBackColor = true;
            btnWin.Click += btnWin_Click;
            // 
            // btnHome
            // 
            btnHome.Cursor = Cursors.Hand;
            resources.ApplyResources(btnHome, "btnHome");
            btnHome.FlatAppearance.BorderSize = 0;
            btnHome.ForeColor = SystemColors.Window;
            btnHome.Image = Properties.Resources.pngHome;
            btnHome.Name = "btnHome";
            btnHome.UseVisualStyleBackColor = true;
            btnHome.Click += btnHome_Click;
            // 
            // PnlFormLoader
            // 
            resources.ApplyResources(PnlFormLoader, "PnlFormLoader");
            PnlFormLoader.BackColor = Color.FromArgb(37, 38, 39);
            PnlFormLoader.Name = "PnlFormLoader";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            Load += Form1_Load;
            Resize += Form1_Resize;
            tableLayoutPanel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel3;
        private Panel panel1;
        private Panel panel2;
        private PictureBox pictureBox1;
        private Panel pnlNav;
        public Button btnReinstallaApp;
        public Button btnmonitoraggio;
        public Button btnTools;
        public Button btnCreaISO;
        public Button btnDebloat;
        public Button btnSettaggi;
        public Button btnOffice;
        public Button btnWin;
        public Button btnHome;
        private Button btnMnmz;
        private Button btnClose;
        public Label lblPanelTitle;
        private PictureBox pictureBox2;
        public Panel PnlFormLoader;
        private Button btnFullScreen;
        public PictureBox pictureBox4;
    }
}
