namespace WinHubX.Dialog.Tools
{
    partial class DialogKasperskyLive
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogKasperskyLive));
            imgTool = new PictureBox();
            lblInfoTool = new Label();
            btnClose = new Button();
            btnDownload = new Button();
            ((System.ComponentModel.ISupportInitialize)imgTool).BeginInit();
            SuspendLayout();
            // 
            // imgTool
            // 
            resources.ApplyResources(imgTool, "imgTool");
            imgTool.Name = "imgTool";
            imgTool.TabStop = false;
            // 
            // lblInfoTool
            // 
            resources.ApplyResources(lblInfoTool, "lblInfoTool");
            lblInfoTool.ForeColor = Color.Coral;
            lblInfoTool.Name = "lblInfoTool";
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
            // DialogKasperskyLive
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnDownload);
            Controls.Add(btnClose);
            Controls.Add(lblInfoTool);
            Controls.Add(imgTool);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DialogKasperskyLive";
            ((System.ComponentModel.ISupportInitialize)imgTool).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox imgTool;
        private Label lblInfoTool;
        private Button btnClose;
        private Button btnDownload;
    }
}