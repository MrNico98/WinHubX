﻿namespace WinHubX
{
    partial class FormHome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHome));
            imgHomeLogo = new PictureBox();
            btnChangelog = new Button();
            btnKofi = new Button();
            tgWinHubX = new Button();
            label3 = new Label();
            lblInfoOffice2024 = new Label();
            label1 = new Label();
            lblInfoWinAIO64 = new Label();
            ((System.ComponentModel.ISupportInitialize)imgHomeLogo).BeginInit();
            SuspendLayout();
            // 
            // imgHomeLogo
            // 
            imgHomeLogo.Image = Properties.Resources.homeLogo;
            imgHomeLogo.Location = new Point(598, 0);
            imgHomeLogo.Margin = new Padding(3, 2, 3, 2);
            imgHomeLogo.Name = "imgHomeLogo";
            imgHomeLogo.Size = new Size(303, 175);
            imgHomeLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            imgHomeLogo.TabIndex = 0;
            imgHomeLogo.TabStop = false;
            // 
            // btnChangelog
            // 
            btnChangelog.Cursor = Cursors.Hand;
            btnChangelog.FlatAppearance.BorderSize = 0;
            btnChangelog.FlatStyle = FlatStyle.Flat;
            btnChangelog.Font = new Font("Microsoft Sans Serif", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnChangelog.ForeColor = Color.Coral;
            btnChangelog.Location = new Point(622, 199);
            btnChangelog.Margin = new Padding(3, 2, 3, 2);
            btnChangelog.Name = "btnChangelog";
            btnChangelog.Size = new Size(279, 60);
            btnChangelog.TabIndex = 94;
            btnChangelog.Text = "v2.4.2.0 - Changelog";
            btnChangelog.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnChangelog.UseVisualStyleBackColor = true;
            btnChangelog.Click += btnChangelog_Click;
            // 
            // btnKofi
            // 
            btnKofi.Cursor = Cursors.Hand;
            btnKofi.FlatAppearance.BorderSize = 0;
            btnKofi.FlatStyle = FlatStyle.Flat;
            btnKofi.Font = new Font("Microsoft Sans Serif", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnKofi.ForeColor = Color.Coral;
            btnKofi.Image = Properties.Resources.pngCoffee;
            btnKofi.Location = new Point(2, 336);
            btnKofi.Margin = new Padding(3, 2, 3, 2);
            btnKofi.Name = "btnKofi";
            btnKofi.Size = new Size(280, 64);
            btnKofi.TabIndex = 95;
            btnKofi.Text = "se vuoi sostenere il\r\nprogetto, clicca qui!";
            btnKofi.TextImageRelation = TextImageRelation.TextBeforeImage;
            btnKofi.UseVisualStyleBackColor = true;
            btnKofi.Click += btnKofi_Click;
            // 
            // tgWinHubX
            // 
            tgWinHubX.Cursor = Cursors.Hand;
            tgWinHubX.FlatStyle = FlatStyle.Flat;
            tgWinHubX.ForeColor = Color.FromArgb(37, 38, 39);
            tgWinHubX.Image = Properties.Resources.pngTelegram;
            tgWinHubX.Location = new Point(772, 401);
            tgWinHubX.Margin = new Padding(3, 2, 3, 2);
            tgWinHubX.Name = "tgWinHubX";
            tgWinHubX.Size = new Size(57, 43);
            tgWinHubX.TabIndex = 96;
            tgWinHubX.UseVisualStyleBackColor = true;
            tgWinHubX.Click += tgWinHubX_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 10.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Coral;
            label3.Location = new Point(4, 414);
            label3.Name = "label3";
            label3.Size = new Size(714, 18);
            label3.TabIndex = 97;
            label3.Text = "Per assistenza (problemi WinHubX, come installare ISO, suggerimenti per WinHubX) contattaci su telegram";
            // 
            // lblInfoOffice2024
            // 
            lblInfoOffice2024.AutoSize = true;
            lblInfoOffice2024.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfoOffice2024.ForeColor = Color.Coral;
            lblInfoOffice2024.Location = new Point(778, 303);
            lblInfoOffice2024.Name = "lblInfoOffice2024";
            lblInfoOffice2024.Size = new Size(111, 24);
            lblInfoOffice2024.TabIndex = 98;
            lblInfoOffice2024.Text = "Developers:";
            lblInfoOffice2024.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Italic);
            label1.ForeColor = Color.Coral;
            label1.Location = new Point(803, 336);
            label1.Name = "label1";
            label1.Size = new Size(76, 20);
            label1.TabIndex = 99;
            label1.Text = "MrNico98";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblInfoWinAIO64
            // 
            lblInfoWinAIO64.AutoSize = true;
            lblInfoWinAIO64.Font = new Font("Microsoft Sans Serif", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInfoWinAIO64.ForeColor = Color.White;
            lblInfoWinAIO64.Location = new Point(2, 9);
            lblInfoWinAIO64.MaximumSize = new Size(595, 0);
            lblInfoWinAIO64.Name = "lblInfoWinAIO64";
            lblInfoWinAIO64.Size = new Size(593, 312);
            lblInfoWinAIO64.TabIndex = 92;
            lblInfoWinAIO64.Text = resources.GetString("lblInfoWinAIO64.Text");
            lblInfoWinAIO64.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FormHome
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            ClientSize = new Size(901, 458);
            Controls.Add(label1);
            Controls.Add(lblInfoOffice2024);
            Controls.Add(tgWinHubX);
            Controls.Add(label3);
            Controls.Add(btnKofi);
            Controls.Add(btnChangelog);
            Controls.Add(lblInfoWinAIO64);
            Controls.Add(imgHomeLogo);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 2, 3, 2);
            Name = "FormHome";
            Text = "FormHome";
            ((System.ComponentModel.ISupportInitialize)imgHomeLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox imgHomeLogo;
        private Button btnChangelog;
        private Button btnKofi;
        private Button tgWinHubX;
        private Label label3;
        private Label lblInfoOffice2024;
        private Label label1;
        private Label lblInfoWinAIO64;
    }
}