namespace WinHubX.Forms.DebloatAvanzato
{
    partial class FormServizi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormServizi));
            button1 = new Button();
            DisabilitaServizi = new CheckedListBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            richTextBox1 = new RichTextBox();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Cursor = Cursors.Hand;
            button1.FlatAppearance.BorderSize = 0;
            button1.ForeColor = Color.White;
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += ModificaServiziButton_Click;
            // 
            // DisabilitaServizi
            // 
            resources.ApplyResources(DisabilitaServizi, "DisabilitaServizi");
            DisabilitaServizi.BackColor = Color.FromArgb(37, 38, 39);
            DisabilitaServizi.BorderStyle = BorderStyle.None;
            DisabilitaServizi.Cursor = Cursors.Hand;
            DisabilitaServizi.ForeColor = Color.White;
            DisabilitaServizi.FormattingEnabled = true;
            DisabilitaServizi.Name = "DisabilitaServizi";
            // 
            // richTextBox1
            // 
            resources.ApplyResources(richTextBox1, "richTextBox1");
            richTextBox1.BackColor = Color.FromArgb(37, 38, 39);
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Name = "richTextBox1";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // FormServizi
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(progressBar1);
            Controls.Add(richTextBox1);
            Controls.Add(DisabilitaServizi);
            Controls.Add(button1);
            Name = "FormServizi";
            Load += FormServizi_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private CheckedListBox DisabilitaServizi;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private RichTextBox richTextBox1;
        private ProgressBar progressBar1;
    }
}