namespace WinHubX.Forms.Personalizzazione_office
{
    partial class FormAggiungiRimuoviAppOffice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAggiungiRimuoviAppOffice));
            tableLayoutPanel2 = new TableLayoutPanel();
            panel5 = new Panel();
            progressBar1 = new ProgressBar();
            btnBack = new Button();
            groupBoxInfo = new GroupBox();
            pictureBox1 = new PictureBox();
            lblVersion = new Label();
            panel6 = new Panel();
            groupBoxActions = new GroupBox();
            labelAggiungi = new Label();
            labelRimuovi = new Label();
            btnAvvia = new Button();
            panelSelection = new Panel();
            lblSelection = new Label();
            tableLayoutPanel2.SuspendLayout();
            panel5.SuspendLayout();
            groupBoxInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel6.SuspendLayout();
            groupBoxActions.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
            tableLayoutPanel2.Controls.Add(panel5, 0, 0);
            tableLayoutPanel2.Controls.Add(panel6, 1, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // panel5
            // 
            resources.ApplyResources(panel5, "panel5");
            panel5.Controls.Add(progressBar1);
            panel5.Controls.Add(btnBack);
            panel5.Controls.Add(groupBoxInfo);
            panel5.Name = "panel5";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // btnBack
            // 
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(btnBack, "btnBack");
            btnBack.Image = Properties.Resources.pngBackArrow;
            btnBack.Name = "btnBack";
            btnBack.UseMnemonic = false;
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // groupBoxInfo
            // 
            resources.ApplyResources(groupBoxInfo, "groupBoxInfo");
            groupBoxInfo.Controls.Add(pictureBox1);
            groupBoxInfo.Controls.Add(lblVersion);
            groupBoxInfo.ForeColor = Color.Coral;
            groupBoxInfo.Name = "groupBoxInfo";
            groupBoxInfo.TabStop = false;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // lblVersion
            // 
            resources.ApplyResources(lblVersion, "lblVersion");
            lblVersion.ForeColor = Color.White;
            lblVersion.Name = "lblVersion";
            // 
            // panel6
            // 
            resources.ApplyResources(panel6, "panel6");
            panel6.Controls.Add(groupBoxActions);
            panel6.Name = "panel6";
            // 
            // groupBoxActions
            // 
            resources.ApplyResources(groupBoxActions, "groupBoxActions");
            groupBoxActions.Controls.Add(labelAggiungi);
            groupBoxActions.Controls.Add(labelRimuovi);
            groupBoxActions.Controls.Add(btnAvvia);
            groupBoxActions.Controls.Add(panelSelection);
            groupBoxActions.Controls.Add(lblSelection);
            groupBoxActions.ForeColor = Color.Coral;
            groupBoxActions.Name = "groupBoxActions";
            groupBoxActions.TabStop = false;
            // 
            // labelAggiungi
            // 
            resources.ApplyResources(labelAggiungi, "labelAggiungi");
            labelAggiungi.ForeColor = Color.White;
            labelAggiungi.Name = "labelAggiungi";
            // 
            // labelRimuovi
            // 
            resources.ApplyResources(labelRimuovi, "labelRimuovi");
            labelRimuovi.ForeColor = Color.White;
            labelRimuovi.Name = "labelRimuovi";
            // 
            // btnAvvia
            // 
            resources.ApplyResources(btnAvvia, "btnAvvia");
            btnAvvia.Cursor = Cursors.Hand;
            btnAvvia.FlatAppearance.BorderColor = Color.Coral;
            btnAvvia.FlatAppearance.BorderSize = 2;
            btnAvvia.ForeColor = Color.White;
            btnAvvia.Name = "btnAvvia";
            btnAvvia.UseVisualStyleBackColor = true;
            btnAvvia.Click += BtnInstall_Click;
            // 
            // panelSelection
            // 
            resources.ApplyResources(panelSelection, "panelSelection");
            panelSelection.BorderStyle = BorderStyle.FixedSingle;
            panelSelection.Cursor = Cursors.Hand;
            panelSelection.Name = "panelSelection";
            // 
            // lblSelection
            // 
            resources.ApplyResources(lblSelection, "lblSelection");
            lblSelection.ForeColor = Color.White;
            lblSelection.Name = "lblSelection";
            // 
            // FormAggiungiRimuoviAppOffice
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(tableLayoutPanel2);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAggiungiRimuoviAppOffice";
            tableLayoutPanel2.ResumeLayout(false);
            panel5.ResumeLayout(false);
            groupBoxInfo.ResumeLayout(false);
            groupBoxInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel6.ResumeLayout(false);
            groupBoxActions.ResumeLayout(false);
            groupBoxActions.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel5;
        private GroupBox groupBoxInfo;
        private Label lblVersion;
        private Panel panel6;
        private GroupBox groupBoxActions;
        private Panel panelSelection;
        private Label lblSelection;
        private Button btnAvvia;
        private Button btnBack;
        private ProgressBar progressBar1;
        private Label labelRimuovi;
        private Label labelAggiungi;
        private PictureBox pictureBox1;
    }
}