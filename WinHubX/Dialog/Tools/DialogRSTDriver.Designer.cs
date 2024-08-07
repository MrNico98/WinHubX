﻿namespace WinHubX.Dialog.Tools
{
    partial class DialogRSTDriver
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogRSTDriver));
            label1 = new Label();
            btnDownload = new Button();
            btnClose = new Button();
            lblInfoTool = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Product Sans Black", 25.8F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(325, 45);
            label1.Name = "label1";
            label1.Size = new Size(151, 110);
            label1.TabIndex = 86;
            label1.Text = "RST\r\nDriver";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnDownload
            // 
            btnDownload.Cursor = Cursors.Hand;
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.FlatStyle = FlatStyle.Flat;
            btnDownload.Font = new Font("Product Sans Black", 25.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDownload.ForeColor = Color.White;
            btnDownload.Location = new Point(218, 417);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(360, 70);
            btnDownload.TabIndex = 85;
            btnDownload.Text = "Salva i Driver";
            btnDownload.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // btnClose
            // 
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Image = Properties.Resources.pngClose;
            btnClose.Location = new Point(733, 12);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(55, 55);
            btnClose.TabIndex = 84;
            btnClose.UseMnemonic = false;
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblInfoTool
            // 
            lblInfoTool.AutoSize = true;
            lblInfoTool.Font = new Font("Product Sans", 15F);
            lblInfoTool.ForeColor = Color.Coral;
            lblInfoTool.Location = new Point(70, 158);
            lblInfoTool.Name = "lblInfoTool";
            lblInfoTool.Size = new Size(659, 256);
            lblInfoTool.TabIndex = 83;
            lblInfoTool.Text = resources.GetString("lblInfoTool.Text");
            lblInfoTool.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DialogRSTDriver
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 38, 39);
            ClientSize = new Size(800, 500);
            Controls.Add(label1);
            Controls.Add(btnDownload);
            Controls.Add(btnClose);
            Controls.Add(lblInfoTool);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DialogRSTDriver";
            Text = "DialogRSTDriver";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btnDownload;
        private Button btnClose;
        private Label lblInfoTool;
    }
}