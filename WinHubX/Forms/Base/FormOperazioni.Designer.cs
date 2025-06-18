namespace WinHubX.Forms.Base
{
    partial class FormOperazioni
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOperazioni));
            lblStatus = new Label();
            progressBar = new ProgressBar();
            groupBox1 = new GroupBox();
            evLog = new RichTextBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // lblStatus
            // 
            resources.ApplyResources(lblStatus, "lblStatus");
            lblStatus.ForeColor = Color.WhiteSmoke;
            lblStatus.Name = "lblStatus";
            // 
            // progressBar
            // 
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.Name = "progressBar";
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(evLog);
            groupBox1.ForeColor = Color.White;
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // evLog
            // 
            resources.ApplyResources(evLog, "evLog");
            evLog.Name = "evLog";
            // 
            // FormOperazioni
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(64, 60, 59);
            Controls.Add(groupBox1);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Name = "FormOperazioni";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblStatus;
        private GroupBox groupBox1;
        private RichTextBox evLog;
        public ProgressBar progressBar;
    }
}