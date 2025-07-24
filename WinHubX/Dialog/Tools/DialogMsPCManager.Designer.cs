namespace WinHubX.Dialog.Tools
{
    partial class DialogMsPCManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogMsPCManager));
            btnDownload = new Button();
            btnClose = new Button();
            lblInfoTool = new Label();
            imgTool = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)imgTool).BeginInit();
            SuspendLayout();
            // 
            // btnDownload
            // 
            resources.ApplyResources(btnDownload, "btnDownload");
            btnDownload.Cursor = Cursors.Hand;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.ForeColor = Color.White;
            btnDownload.Name = "btnDownload";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
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
            // lblInfoTool
            // 
            resources.ApplyResources(lblInfoTool, "lblInfoTool");
            lblInfoTool.ForeColor = Color.Coral;
            lblInfoTool.Name = "lblInfoTool";
            // 
            // imgTool
            // 
            resources.ApplyResources(imgTool, "imgTool");
            imgTool.Image = Properties.Resources.pngMPM;
            imgTool.Name = "imgTool";
            imgTool.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.White;
            label1.Name = "label1";
            // 
            // DialogMsPCManager
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(label1);
            Controls.Add(btnDownload);
            Controls.Add(btnClose);
            Controls.Add(lblInfoTool);
            Controls.Add(imgTool);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DialogMsPCManager";
            ((System.ComponentModel.ISupportInitialize)imgTool).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnDownload;
        private Button btnClose;
        private Label lblInfoTool;
        private PictureBox imgTool;
        private Label label1;
    }
}