namespace WinHubX.Forms.Settaggi
{
    partial class FormUpdate
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdate));
            btnBack = new Button();
            DisabilitaUpdate = new CheckedListBox();
            btnAvviaSelezionatiUpda = new Button();
            AbilitaUpdate = new CheckedListBox();
            label2 = new Label();
            label1 = new Label();
            btnUpdateEssential = new Button();
            btnResetUpdate = new Button();
            progressBar1 = new ProgressBar();
            toolTip1 = new ToolTip(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            SuspendLayout();
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
            // DisabilitaUpdate
            // 
            resources.ApplyResources(DisabilitaUpdate, "DisabilitaUpdate");
            DisabilitaUpdate.BackColor = Color.FromArgb(37, 38, 39);
            DisabilitaUpdate.BorderStyle = BorderStyle.None;
            DisabilitaUpdate.Cursor = Cursors.Hand;
            DisabilitaUpdate.ForeColor = Color.White;
            DisabilitaUpdate.FormattingEnabled = true;
            DisabilitaUpdate.Items.AddRange(new object[] { resources.GetString("DisabilitaUpdate.Items"), resources.GetString("DisabilitaUpdate.Items1"), resources.GetString("DisabilitaUpdate.Items2"), resources.GetString("DisabilitaUpdate.Items3"), resources.GetString("DisabilitaUpdate.Items4") });
            DisabilitaUpdate.Name = "DisabilitaUpdate";
            // 
            // btnAvviaSelezionatiUpda
            // 
            resources.ApplyResources(btnAvviaSelezionatiUpda, "btnAvviaSelezionatiUpda");
            btnAvviaSelezionatiUpda.Cursor = Cursors.Hand;
            btnAvviaSelezionatiUpda.FlatAppearance.BorderSize = 0;
            btnAvviaSelezionatiUpda.ForeColor = Color.White;
            btnAvviaSelezionatiUpda.Name = "btnAvviaSelezionatiUpda";
            btnAvviaSelezionatiUpda.UseVisualStyleBackColor = true;
            btnAvviaSelezionatiUpda.Click += btnAvviaSelezionatiUpda_Click;
            // 
            // AbilitaUpdate
            // 
            resources.ApplyResources(AbilitaUpdate, "AbilitaUpdate");
            AbilitaUpdate.BackColor = Color.FromArgb(37, 38, 39);
            AbilitaUpdate.BorderStyle = BorderStyle.None;
            AbilitaUpdate.Cursor = Cursors.Hand;
            AbilitaUpdate.ForeColor = Color.White;
            AbilitaUpdate.FormattingEnabled = true;
            AbilitaUpdate.Items.AddRange(new object[] { resources.GetString("AbilitaUpdate.Items"), resources.GetString("AbilitaUpdate.Items1"), resources.GetString("AbilitaUpdate.Items2"), resources.GetString("AbilitaUpdate.Items3"), resources.GetString("AbilitaUpdate.Items4") });
            AbilitaUpdate.Name = "AbilitaUpdate";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.White;
            label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
            // 
            // btnUpdateEssential
            // 
            resources.ApplyResources(btnUpdateEssential, "btnUpdateEssential");
            btnUpdateEssential.Cursor = Cursors.Hand;
            btnUpdateEssential.FlatAppearance.BorderSize = 0;
            btnUpdateEssential.ForeColor = Color.White;
            btnUpdateEssential.Name = "btnUpdateEssential";
            btnUpdateEssential.UseVisualStyleBackColor = true;
            btnUpdateEssential.Click += btnUpdateEssential_Click;
            // 
            // btnResetUpdate
            // 
            resources.ApplyResources(btnResetUpdate, "btnResetUpdate");
            btnResetUpdate.Cursor = Cursors.Hand;
            btnResetUpdate.FlatAppearance.BorderSize = 0;
            btnResetUpdate.ForeColor = Color.White;
            btnResetUpdate.Name = "btnResetUpdate";
            btnResetUpdate.UseVisualStyleBackColor = true;
            btnResetUpdate.Click += btnResetUpdate_Click;
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // FormUpdate
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(progressBar1);
            Controls.Add(btnResetUpdate);
            Controls.Add(btnUpdateEssential);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(AbilitaUpdate);
            Controls.Add(btnAvviaSelezionatiUpda);
            Controls.Add(DisabilitaUpdate);
            Controls.Add(btnBack);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormUpdate";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private CheckedListBox DisabilitaUpdate;
        private Button btnAvviaSelezionatiUpda;
        private CheckedListBox AbilitaUpdate;
        private Label label2;
        private Label label1;
        private Button btnUpdateEssential;
        private Button btnResetUpdate;
        private ProgressBar progressBar1;
        private ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}