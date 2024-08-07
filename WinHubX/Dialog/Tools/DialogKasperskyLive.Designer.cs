﻿namespace WinHubX.Dialog.Tools
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
            imgTool.Image = (Image)resources.GetObject("imgTool.Image");
            imgTool.Location = new Point(132, 9);
            imgTool.Margin = new Padding(3, 2, 3, 2);
            imgTool.Name = "imgTool";
            imgTool.Size = new Size(424, 148);
            imgTool.TabIndex = 0;
            imgTool.TabStop = false;
            // 
            // lblInfoTool
            // 
            lblInfoTool.AutoSize = true;
            lblInfoTool.Font = new Font("Microsoft Sans Serif", 15F);
            lblInfoTool.ForeColor = Color.Coral;
            lblInfoTool.Location = new Point(132, 173);
            lblInfoTool.Name = "lblInfoTool";
            lblInfoTool.Size = new Size(439, 100);
            lblInfoTool.TabIndex = 74;
            lblInfoTool.Text = "Kaspersky Live, è una iso basta su linux\r\nin grado di eseguire una scansione completa\r\ne approfondita del tuo PC, per cercare e eliminare\r\neventuali malware sul tuo dispositivo!";
            lblInfoTool.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Image = Properties.Resources.pngClose;
            btnClose.Location = new Point(641, 9);
            btnClose.Margin = new Padding(3, 2, 3, 2);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(48, 41);
            btnClose.TabIndex = 75;
            btnClose.UseMnemonic = false;
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnDownload
            // 
            btnDownload.Cursor = Cursors.Hand;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.FlatStyle = FlatStyle.Flat;
            btnDownload.Font = new Font("Microsoft Sans Serif", 25.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDownload.ForeColor = Color.White;
            btnDownload.Location = new Point(159, 312);
            btnDownload.Margin = new Padding(3, 2, 3, 2);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(367, 52);
            btnDownload.TabIndex = 76;
            btnDownload.Text = "Download ~685MB";
            btnDownload.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // DialogKasperskyLive
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 38, 39);
            ClientSize = new Size(700, 375);
            Controls.Add(btnDownload);
            Controls.Add(btnClose);
            Controls.Add(lblInfoTool);
            Controls.Add(imgTool);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "DialogKasperskyLive";
            Text = "Kaspersky Live, è una iso basta su linux\r\nin grado di eseguire una scansione completa\r\ne approfondita del tuo PC, per cercare e eliminare\r\neventuali malware sul tuo dispositivo!";
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