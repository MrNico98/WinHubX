using System.Windows.Forms;

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
            label1 = new Label();
            label4 = new Label();
            lblversioneoffice = new Label();
            flowPanelApps = new FlowLayoutPanel();
            panel6 = new Panel();
            flowPanelAppsInstall = new FlowLayoutPanel();
            label2 = new Label();
            panel80 = new Panel();
            flowPanelAppsRimuovi = new FlowLayoutPanel();
            label3 = new Label();
            panel20 = new Panel();
            btn_avviaVerdi = new CuoreUI.Controls.cuiButton();
            progressBar1 = new CuoreUI.Controls.cuiProgressBarHorizontal();
            tableLayoutPanel2.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            panel80.SuspendLayout();
            panel20.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
            tableLayoutPanel2.Controls.Add(panel5, 0, 0);
            tableLayoutPanel2.Controls.Add(panel6, 1, 0);
            tableLayoutPanel2.Controls.Add(panel80, 2, 0);
            tableLayoutPanel2.Controls.Add(panel20, 2, 1);
            tableLayoutPanel2.Controls.Add(progressBar1, 0, 1);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // panel5
            // 
            panel5.Controls.Add(label1);
            panel5.Controls.Add(label4);
            panel5.Controls.Add(lblversioneoffice);
            panel5.Controls.Add(flowPanelApps);
            resources.ApplyResources(panel5, "panel5");
            panel5.Name = "panel5";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.FromArgb(0, 126, 249);
            label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ForeColor = Color.FromArgb(0, 126, 249);
            label4.Name = "label4";
            // 
            // lblversioneoffice
            // 
            resources.ApplyResources(lblversioneoffice, "lblversioneoffice");
            lblversioneoffice.ForeColor = Color.White;
            lblversioneoffice.Name = "lblversioneoffice";
            // 
            // flowPanelApps
            // 
            resources.ApplyResources(flowPanelApps, "flowPanelApps");
            flowPanelApps.Name = "flowPanelApps";
            // 
            // panel6
            // 
            resources.ApplyResources(panel6, "panel6");
            panel6.Controls.Add(flowPanelAppsInstall);
            panel6.Controls.Add(label2);
            panel6.Name = "panel6";
            // 
            // flowPanelAppsInstall
            // 
            resources.ApplyResources(flowPanelAppsInstall, "flowPanelAppsInstall");
            flowPanelAppsInstall.Name = "flowPanelAppsInstall";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ForeColor = Color.FromArgb(0, 126, 249);
            label2.Name = "label2";
            // 
            // panel80
            // 
            panel80.Controls.Add(flowPanelAppsRimuovi);
            panel80.Controls.Add(label3);
            resources.ApplyResources(panel80, "panel80");
            panel80.Name = "panel80";
            // 
            // flowPanelAppsRimuovi
            // 
            resources.ApplyResources(flowPanelAppsRimuovi, "flowPanelAppsRimuovi");
            flowPanelAppsRimuovi.Name = "flowPanelAppsRimuovi";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.FromArgb(0, 126, 249);
            label3.Name = "label3";
            // 
            // panel20
            // 
            panel20.Controls.Add(btn_avviaVerdi);
            resources.ApplyResources(panel20, "panel20");
            panel20.Name = "panel20";
            // 
            // btn_avviaVerdi
            // 
            resources.ApplyResources(btn_avviaVerdi, "btn_avviaVerdi");
            btn_avviaVerdi.CheckButton = false;
            btn_avviaVerdi.Checked = false;
            btn_avviaVerdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            btn_avviaVerdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            btn_avviaVerdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            btn_avviaVerdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            btn_avviaVerdi.Content = "  Avvia";
            btn_avviaVerdi.DialogResult = DialogResult.None;
            btn_avviaVerdi.ForeColor = Color.White;
            btn_avviaVerdi.HoverBackground = Color.FromArgb(46, 125, 50);
            btn_avviaVerdi.HoverForeColor = Color.White;
            btn_avviaVerdi.HoverImageTint = Color.White;
            btn_avviaVerdi.HoverOutline = Color.FromArgb(46, 125, 50);
            btn_avviaVerdi.Image = Properties.Resources.pngCheckCreaISO;
            btn_avviaVerdi.ImageAutoCenter = true;
            btn_avviaVerdi.ImageExpand = new Point(0, 0);
            btn_avviaVerdi.ImageOffset = new Point(0, 0);
            btn_avviaVerdi.Name = "btn_avviaVerdi";
            btn_avviaVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btn_avviaVerdi.NormalForeColor = Color.White;
            btn_avviaVerdi.NormalImageTint = Color.White;
            btn_avviaVerdi.NormalOutline = Color.FromArgb(46, 125, 50);
            btn_avviaVerdi.OutlineThickness = 1F;
            btn_avviaVerdi.PressedBackground = Color.FromArgb(46, 125, 50);
            btn_avviaVerdi.PressedForeColor = Color.Black;
            btn_avviaVerdi.PressedImageTint = Color.Black;
            btn_avviaVerdi.PressedOutline = Color.FromArgb(46, 125, 50);
            btn_avviaVerdi.Rounding = new Padding(8);
            btn_avviaVerdi.TextAlignment = StringAlignment.Center;
            btn_avviaVerdi.TextOffset = new Point(0, 0);
            btn_avviaVerdi.Click += BtnInstall_Click;
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Background = Color.FromArgb(64, 128, 128, 128);
            progressBar1.Flipped = false;
            progressBar1.Foreground = Color.FromArgb(46, 125, 60);
            progressBar1.MaxValue = 100;
            progressBar1.Name = "progressBar1";
            progressBar1.Rounding = 8;
            progressBar1.Value = 0;
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
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel80.ResumeLayout(false);
            panel80.PerformLayout();
            panel20.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel6;
        private Panel panel80;
        private Label label2;
        private Label label3;
        private Panel panel5;
        private Label lblversioneoffice;
        private FlowLayoutPanel flowPanelApps;
        private Label label4;
        private Label label1;
        private Panel panel20;
        private CuoreUI.Controls.cuiButton btn_avviaVerdi;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar1;
        private FlowLayoutPanel flowPanelAppsInstall;
        private FlowLayoutPanel flowPanelAppsRimuovi;
    }
}