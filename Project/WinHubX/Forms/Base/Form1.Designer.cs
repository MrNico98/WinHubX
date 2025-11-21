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
            pictureBox3 = new PictureBox();
            picMinimizzaApp = new PictureBox();
            picEspandiApp = new PictureBox();
            picCloseApp = new PictureBox();
            pictureBoxlblalto = new PictureBox();
            lblPanelTitle = new Label();
            panel1 = new Panel();
            pictureBoxLogoForm1 = new PictureBox();
            panel2 = new Panel();
            picImpostazioniApp = new PictureBox();
            pnlNav = new CuoreUI.Controls.cuiPanel();
            btnmonitoraggio = new Button();
            btnDebloat = new Button();
            btnSettaggi = new Button();
            btnOffice = new Button();
            btnWin = new Button();
            btnHome = new Button();
            PnlFormLoader = new Panel();
            cuiFormRounder1 = new CuoreUI.Components.cuiFormRounder();
            tableLayoutPanel1.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picMinimizzaApp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picEspandiApp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCloseApp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxlblalto).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogoForm1).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImpostazioniApp).BeginInit();
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
            panel3.Controls.Add(pictureBox3);
            panel3.Controls.Add(picMinimizzaApp);
            panel3.Controls.Add(picEspandiApp);
            panel3.Controls.Add(picCloseApp);
            panel3.Controls.Add(pictureBoxlblalto);
            panel3.Controls.Add(lblPanelTitle);
            panel3.Name = "panel3";
            // 
            // pictureBox3
            // 
            resources.ApplyResources(pictureBox3, "pictureBox3");
            pictureBox3.Cursor = Cursors.Hand;
            pictureBox3.Image = Properties.Resources.pngFrecciaHome;
            pictureBox3.Name = "pictureBox3";
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // picMinimizzaApp
            // 
            resources.ApplyResources(picMinimizzaApp, "picMinimizzaApp");
            picMinimizzaApp.Cursor = Cursors.Hand;
            picMinimizzaApp.Image = Properties.Resources.pngMinimizzaForm1;
            picMinimizzaApp.Name = "picMinimizzaApp";
            picMinimizzaApp.TabStop = false;
            picMinimizzaApp.Click += btnMnmz_Click;
            // 
            // picEspandiApp
            // 
            resources.ApplyResources(picEspandiApp, "picEspandiApp");
            picEspandiApp.Cursor = Cursors.Hand;
            picEspandiApp.Image = Properties.Resources.pngEspandiForm1;
            picEspandiApp.Name = "picEspandiApp";
            picEspandiApp.TabStop = false;
            picEspandiApp.Click += btnFullScreen_Click;
            // 
            // picCloseApp
            // 
            resources.ApplyResources(picCloseApp, "picCloseApp");
            picCloseApp.Cursor = Cursors.Hand;
            picCloseApp.Image = Properties.Resources.pngChiudiForm1;
            picCloseApp.Name = "picCloseApp";
            picCloseApp.TabStop = false;
            picCloseApp.Click += btnClose_Click;
            // 
            // pictureBoxlblalto
            // 
            resources.ApplyResources(pictureBoxlblalto, "pictureBoxlblalto");
            pictureBoxlblalto.Name = "pictureBoxlblalto";
            pictureBoxlblalto.TabStop = false;
            // 
            // lblPanelTitle
            // 
            resources.ApplyResources(lblPanelTitle, "lblPanelTitle");
            lblPanelTitle.ForeColor = Color.White;
            lblPanelTitle.Name = "lblPanelTitle";
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(pictureBoxLogoForm1);
            panel1.Name = "panel1";
            // 
            // pictureBoxLogoForm1
            // 
            resources.ApplyResources(pictureBoxLogoForm1, "pictureBoxLogoForm1");
            pictureBoxLogoForm1.BackColor = Color.FromArgb(64, 60, 59);
            pictureBoxLogoForm1.Image = Properties.Resources.pngLogoWinHubX;
            pictureBoxLogoForm1.Name = "pictureBoxLogoForm1";
            pictureBoxLogoForm1.TabStop = false;
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.BackColor = Color.FromArgb(64, 60, 59);
            panel2.Controls.Add(picImpostazioniApp);
            panel2.Controls.Add(pnlNav);
            panel2.Controls.Add(btnmonitoraggio);
            panel2.Controls.Add(btnDebloat);
            panel2.Controls.Add(btnSettaggi);
            panel2.Controls.Add(btnOffice);
            panel2.Controls.Add(btnWin);
            panel2.Controls.Add(btnHome);
            panel2.Name = "panel2";
            // 
            // picImpostazioniApp
            // 
            resources.ApplyResources(picImpostazioniApp, "picImpostazioniApp");
            picImpostazioniApp.Cursor = Cursors.Hand;
            picImpostazioniApp.Image = Properties.Resources.pngImpostazioniForm1;
            picImpostazioniApp.Name = "picImpostazioniApp";
            picImpostazioniApp.TabStop = false;
            picImpostazioniApp.Click += cuiPictureBox1_Click;
            // 
            // pnlNav
            // 
            resources.ApplyResources(pnlNav, "pnlNav");
            pnlNav.BackColor = Color.FromArgb(0, 126, 249);
            pnlNav.Name = "pnlNav";
            pnlNav.OutlineThickness = 1F;
            pnlNav.PanelColor = Color.FromArgb(0, 126, 249);
            pnlNav.PanelOutlineColor = Color.FromArgb(0, 126, 249);
            pnlNav.Rounding = new Padding(10);
            // 
            // btnmonitoraggio
            // 
            resources.ApplyResources(btnmonitoraggio, "btnmonitoraggio");
            btnmonitoraggio.Cursor = Cursors.Hand;
            btnmonitoraggio.FlatAppearance.BorderSize = 0;
            btnmonitoraggio.ForeColor = SystemColors.Window;
            btnmonitoraggio.Image = Properties.Resources.pngMonitoraggioForm1;
            btnmonitoraggio.Name = "btnmonitoraggio";
            btnmonitoraggio.UseVisualStyleBackColor = true;
            btnmonitoraggio.Click += btnmonitoraggio_Click;
            // 
            // btnDebloat
            // 
            resources.ApplyResources(btnDebloat, "btnDebloat");
            btnDebloat.Cursor = Cursors.Hand;
            btnDebloat.FlatAppearance.BorderSize = 0;
            btnDebloat.ForeColor = SystemColors.Window;
            btnDebloat.Image = Properties.Resources.pngDebloatForm1;
            btnDebloat.Name = "btnDebloat";
            btnDebloat.UseVisualStyleBackColor = true;
            btnDebloat.Click += btnDebloat_Click;
            // 
            // btnSettaggi
            // 
            resources.ApplyResources(btnSettaggi, "btnSettaggi");
            btnSettaggi.Cursor = Cursors.Hand;
            btnSettaggi.FlatAppearance.BorderSize = 0;
            btnSettaggi.ForeColor = SystemColors.Window;
            btnSettaggi.Image = Properties.Resources.pngTweaksForm1;
            btnSettaggi.Name = "btnSettaggi";
            btnSettaggi.UseVisualStyleBackColor = true;
            btnSettaggi.Click += btnSettaggi_Click;
            // 
            // btnOffice
            // 
            resources.ApplyResources(btnOffice, "btnOffice");
            btnOffice.Cursor = Cursors.Hand;
            btnOffice.FlatAppearance.BorderSize = 0;
            btnOffice.ForeColor = SystemColors.Window;
            btnOffice.Image = Properties.Resources.pngOfficeForm1;
            btnOffice.Name = "btnOffice";
            btnOffice.UseVisualStyleBackColor = true;
            btnOffice.Click += btnOffice_Click;
            // 
            // btnWin
            // 
            resources.ApplyResources(btnWin, "btnWin");
            btnWin.Cursor = Cursors.Hand;
            btnWin.FlatAppearance.BorderSize = 0;
            btnWin.ForeColor = SystemColors.Window;
            btnWin.Image = Properties.Resources.pngWindowsForm1;
            btnWin.Name = "btnWin";
            btnWin.UseVisualStyleBackColor = true;
            btnWin.Click += btnWin_Click;
            // 
            // btnHome
            // 
            resources.ApplyResources(btnHome, "btnHome");
            btnHome.Cursor = Cursors.Hand;
            btnHome.FlatAppearance.BorderSize = 0;
            btnHome.ForeColor = SystemColors.Window;
            btnHome.Image = Properties.Resources.pngHomeForm1;
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
            // cuiFormRounder1
            // 
            cuiFormRounder1.OutlineColor = Color.FromArgb(32, 128, 128, 128);
            cuiFormRounder1.Rounding = 10;
            cuiFormRounder1.TargetForm = this;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(64, 60, 59);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            Load += Form1_Load;
            tableLayoutPanel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)picMinimizzaApp).EndInit();
            ((System.ComponentModel.ISupportInitialize)picEspandiApp).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCloseApp).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxlblalto).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogoForm1).EndInit();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picImpostazioniApp).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private Panel panel2;
        private PictureBox pictureBoxLogoForm1;
        public Button btnmonitoraggio;
        public Button btnDebloat;
        public Button btnSettaggi;
        public Button btnOffice;
        public Button btnWin;
        public Button btnHome;
        public Label lblPanelTitle;
        public Panel PnlFormLoader;
        private CuoreUI.Controls.cuiPanel pnlNav;
        public Panel panel3;
        public PictureBox pictureBoxlblalto;
        public CuoreUI.Controls.cuiPictureBox cuiPictureBox4;
        private PictureBox picCloseApp;
        private PictureBox picEspandiApp;
        private PictureBox picMinimizzaApp;
        private PictureBox picImpostazioniApp;
        public PictureBox pictureBox3;
        private CuoreUI.Components.cuiFormRounder cuiFormRounder1;
    }
}
