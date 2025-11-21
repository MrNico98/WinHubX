namespace WinHubX.Forms.Settaggi
{
    partial class FormPersonalizzazione
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPersonalizzazione));
            toolTip1 = new ToolTip(components);
            panello = new Panel();
            panel76 = new Panel();
            radio_disabilitaendtask = new CheckBox();
            radio_abilitaendtask = new CheckBox();
            radio_eliminapowershell = new CheckBox();
            radio_apripowershell = new CheckBox();
            radio_eliminaapricmd = new CheckBox();
            radio_destrolegacy = new CheckBox();
            radio_destrodefault = new CheckBox();
            radio_apricmd = new CheckBox();
            panel77 = new Panel();
            radio_disacopilot = new CheckBox();
            radio_abilicopilot = new CheckBox();
            radio_disattivafx = new CheckBox();
            radio_attivafx = new CheckBox();
            radio_disabilitarecall = new CheckBox();
            radio_abilitarecall = new CheckBox();
            radio_disabilitasuggeriti = new CheckBox();
            radio_abilitasuggeriti = new CheckBox();
            radio_disabilitaottimizzaricerca = new CheckBox();
            radio_disabilitaricercainternet = new CheckBox();
            radio_abilitaRicercainternet = new CheckBox();
            radio_ottimizzaricerca = new CheckBox();
            panel78 = new Panel();
            radio_orologionascondioradata = new CheckBox();
            radio_orologiomostraoradata = new CheckBox();
            radio_orologiomostradatasecondi = new CheckBox();
            radio_orologiostandard = new CheckBox();
            radio_orologiomostrasecondi = new CheckBox();
            btnSettaggiExplorerVerdi = new CuoreUI.Controls.cuiButton();
            btnResetVerdi = new CuoreUI.Controls.cuiButton();
            cuiButton1Verdi = new CuoreUI.Controls.cuiButton();
            progressBar1 = new CuoreUI.Controls.cuiProgressBarHorizontal();
            label1 = new Label();
            label4 = new Label();
            label3 = new Label();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panello.SuspendLayout();
            panel76.SuspendLayout();
            panel77.SuspendLayout();
            panel78.SuspendLayout();
            SuspendLayout();
            // 
            // panello
            // 
            panello.Controls.Add(panel76);
            panello.Controls.Add(panel77);
            panello.Controls.Add(panel78);
            panello.Controls.Add(btnSettaggiExplorerVerdi);
            panello.Controls.Add(btnResetVerdi);
            panello.Controls.Add(cuiButton1Verdi);
            panello.Controls.Add(progressBar1);
            panello.Controls.Add(label1);
            panello.Controls.Add(label4);
            panello.Controls.Add(label3);
            resources.ApplyResources(panello, "panello");
            panello.Name = "panello";
            // 
            // panel76
            // 
            panel76.Controls.Add(radio_disabilitaendtask);
            panel76.Controls.Add(radio_abilitaendtask);
            panel76.Controls.Add(radio_eliminapowershell);
            panel76.Controls.Add(radio_apripowershell);
            panel76.Controls.Add(radio_eliminaapricmd);
            panel76.Controls.Add(radio_destrolegacy);
            panel76.Controls.Add(radio_destrodefault);
            panel76.Controls.Add(radio_apricmd);
            resources.ApplyResources(panel76, "panel76");
            panel76.Name = "panel76";
            // 
            // radio_disabilitaendtask
            // 
            resources.ApplyResources(radio_disabilitaendtask, "radio_disabilitaendtask");
            radio_disabilitaendtask.ForeColor = Color.White;
            radio_disabilitaendtask.Name = "radio_disabilitaendtask";
            radio_disabilitaendtask.UseVisualStyleBackColor = true;
            radio_disabilitaendtask.CheckedChanged += panel76_CheckedChanged;
            // 
            // radio_abilitaendtask
            // 
            resources.ApplyResources(radio_abilitaendtask, "radio_abilitaendtask");
            radio_abilitaendtask.ForeColor = Color.White;
            radio_abilitaendtask.Name = "radio_abilitaendtask";
            radio_abilitaendtask.UseVisualStyleBackColor = true;
            radio_abilitaendtask.CheckedChanged += panel76_CheckedChanged;
            // 
            // radio_eliminapowershell
            // 
            resources.ApplyResources(radio_eliminapowershell, "radio_eliminapowershell");
            radio_eliminapowershell.ForeColor = Color.White;
            radio_eliminapowershell.Name = "radio_eliminapowershell";
            radio_eliminapowershell.UseVisualStyleBackColor = true;
            radio_eliminapowershell.CheckedChanged += panel76_CheckedChanged;
            // 
            // radio_apripowershell
            // 
            resources.ApplyResources(radio_apripowershell, "radio_apripowershell");
            radio_apripowershell.ForeColor = Color.White;
            radio_apripowershell.Name = "radio_apripowershell";
            radio_apripowershell.UseVisualStyleBackColor = true;
            radio_apripowershell.CheckedChanged += panel76_CheckedChanged;
            // 
            // radio_eliminaapricmd
            // 
            resources.ApplyResources(radio_eliminaapricmd, "radio_eliminaapricmd");
            radio_eliminaapricmd.ForeColor = Color.White;
            radio_eliminaapricmd.Name = "radio_eliminaapricmd";
            radio_eliminaapricmd.UseVisualStyleBackColor = true;
            radio_eliminaapricmd.CheckedChanged += panel76_CheckedChanged;
            // 
            // radio_destrolegacy
            // 
            resources.ApplyResources(radio_destrolegacy, "radio_destrolegacy");
            radio_destrolegacy.ForeColor = Color.White;
            radio_destrolegacy.Name = "radio_destrolegacy";
            radio_destrolegacy.UseVisualStyleBackColor = true;
            radio_destrolegacy.CheckedChanged += panel76_CheckedChanged;
            // 
            // radio_destrodefault
            // 
            resources.ApplyResources(radio_destrodefault, "radio_destrodefault");
            radio_destrodefault.ForeColor = Color.White;
            radio_destrodefault.Name = "radio_destrodefault";
            radio_destrodefault.UseVisualStyleBackColor = true;
            radio_destrodefault.CheckedChanged += panel76_CheckedChanged;
            // 
            // radio_apricmd
            // 
            resources.ApplyResources(radio_apricmd, "radio_apricmd");
            radio_apricmd.ForeColor = Color.White;
            radio_apricmd.Name = "radio_apricmd";
            radio_apricmd.UseVisualStyleBackColor = true;
            radio_apricmd.CheckedChanged += panel76_CheckedChanged;
            // 
            // panel77
            // 
            panel77.Controls.Add(radio_disacopilot);
            panel77.Controls.Add(radio_abilicopilot);
            panel77.Controls.Add(radio_disattivafx);
            panel77.Controls.Add(radio_attivafx);
            panel77.Controls.Add(radio_disabilitarecall);
            panel77.Controls.Add(radio_abilitarecall);
            panel77.Controls.Add(radio_disabilitasuggeriti);
            panel77.Controls.Add(radio_abilitasuggeriti);
            panel77.Controls.Add(radio_disabilitaottimizzaricerca);
            panel77.Controls.Add(radio_disabilitaricercainternet);
            panel77.Controls.Add(radio_abilitaRicercainternet);
            panel77.Controls.Add(radio_ottimizzaricerca);
            resources.ApplyResources(panel77, "panel77");
            panel77.Name = "panel77";
            // 
            // radio_disacopilot
            // 
            resources.ApplyResources(radio_disacopilot, "radio_disacopilot");
            radio_disacopilot.ForeColor = Color.White;
            radio_disacopilot.Name = "radio_disacopilot";
            radio_disacopilot.UseVisualStyleBackColor = true;
            radio_disacopilot.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_abilicopilot
            // 
            resources.ApplyResources(radio_abilicopilot, "radio_abilicopilot");
            radio_abilicopilot.ForeColor = Color.White;
            radio_abilicopilot.Name = "radio_abilicopilot";
            radio_abilicopilot.UseVisualStyleBackColor = true;
            radio_abilicopilot.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_disattivafx
            // 
            resources.ApplyResources(radio_disattivafx, "radio_disattivafx");
            radio_disattivafx.ForeColor = Color.White;
            radio_disattivafx.Name = "radio_disattivafx";
            radio_disattivafx.UseVisualStyleBackColor = true;
            radio_disattivafx.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_attivafx
            // 
            resources.ApplyResources(radio_attivafx, "radio_attivafx");
            radio_attivafx.ForeColor = Color.White;
            radio_attivafx.Name = "radio_attivafx";
            radio_attivafx.UseVisualStyleBackColor = true;
            radio_attivafx.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_disabilitarecall
            // 
            resources.ApplyResources(radio_disabilitarecall, "radio_disabilitarecall");
            radio_disabilitarecall.ForeColor = Color.White;
            radio_disabilitarecall.Name = "radio_disabilitarecall";
            radio_disabilitarecall.UseVisualStyleBackColor = true;
            radio_disabilitarecall.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_abilitarecall
            // 
            resources.ApplyResources(radio_abilitarecall, "radio_abilitarecall");
            radio_abilitarecall.ForeColor = Color.White;
            radio_abilitarecall.Name = "radio_abilitarecall";
            radio_abilitarecall.UseVisualStyleBackColor = true;
            radio_abilitarecall.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_disabilitasuggeriti
            // 
            resources.ApplyResources(radio_disabilitasuggeriti, "radio_disabilitasuggeriti");
            radio_disabilitasuggeriti.ForeColor = Color.White;
            radio_disabilitasuggeriti.Name = "radio_disabilitasuggeriti";
            radio_disabilitasuggeriti.UseVisualStyleBackColor = true;
            radio_disabilitasuggeriti.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_abilitasuggeriti
            // 
            resources.ApplyResources(radio_abilitasuggeriti, "radio_abilitasuggeriti");
            radio_abilitasuggeriti.ForeColor = Color.White;
            radio_abilitasuggeriti.Name = "radio_abilitasuggeriti";
            radio_abilitasuggeriti.UseVisualStyleBackColor = true;
            radio_abilitasuggeriti.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_disabilitaottimizzaricerca
            // 
            resources.ApplyResources(radio_disabilitaottimizzaricerca, "radio_disabilitaottimizzaricerca");
            radio_disabilitaottimizzaricerca.ForeColor = Color.White;
            radio_disabilitaottimizzaricerca.Name = "radio_disabilitaottimizzaricerca";
            radio_disabilitaottimizzaricerca.UseVisualStyleBackColor = true;
            radio_disabilitaottimizzaricerca.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_disabilitaricercainternet
            // 
            resources.ApplyResources(radio_disabilitaricercainternet, "radio_disabilitaricercainternet");
            radio_disabilitaricercainternet.ForeColor = Color.White;
            radio_disabilitaricercainternet.Name = "radio_disabilitaricercainternet";
            radio_disabilitaricercainternet.UseVisualStyleBackColor = true;
            radio_disabilitaricercainternet.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_abilitaRicercainternet
            // 
            resources.ApplyResources(radio_abilitaRicercainternet, "radio_abilitaRicercainternet");
            radio_abilitaRicercainternet.ForeColor = Color.White;
            radio_abilitaRicercainternet.Name = "radio_abilitaRicercainternet";
            radio_abilitaRicercainternet.UseVisualStyleBackColor = true;
            radio_abilitaRicercainternet.CheckedChanged += panel77_CheckChanged;
            // 
            // radio_ottimizzaricerca
            // 
            resources.ApplyResources(radio_ottimizzaricerca, "radio_ottimizzaricerca");
            radio_ottimizzaricerca.ForeColor = Color.White;
            radio_ottimizzaricerca.Name = "radio_ottimizzaricerca";
            radio_ottimizzaricerca.UseVisualStyleBackColor = true;
            radio_ottimizzaricerca.CheckedChanged += panel77_CheckChanged;
            // 
            // panel78
            // 
            panel78.Controls.Add(radio_orologionascondioradata);
            panel78.Controls.Add(radio_orologiomostraoradata);
            panel78.Controls.Add(radio_orologiomostradatasecondi);
            panel78.Controls.Add(radio_orologiostandard);
            panel78.Controls.Add(radio_orologiomostrasecondi);
            resources.ApplyResources(panel78, "panel78");
            panel78.Name = "panel78";
            // 
            // radio_orologionascondioradata
            // 
            resources.ApplyResources(radio_orologionascondioradata, "radio_orologionascondioradata");
            radio_orologionascondioradata.ForeColor = Color.White;
            radio_orologionascondioradata.Name = "radio_orologionascondioradata";
            radio_orologionascondioradata.UseVisualStyleBackColor = true;
            radio_orologionascondioradata.CheckedChanged += radio_orologio_CheckedChanged;
            // 
            // radio_orologiomostraoradata
            // 
            resources.ApplyResources(radio_orologiomostraoradata, "radio_orologiomostraoradata");
            radio_orologiomostraoradata.ForeColor = Color.White;
            radio_orologiomostraoradata.Name = "radio_orologiomostraoradata";
            radio_orologiomostraoradata.UseVisualStyleBackColor = true;
            radio_orologiomostraoradata.CheckedChanged += radio_orologio_CheckedChanged;
            // 
            // radio_orologiomostradatasecondi
            // 
            resources.ApplyResources(radio_orologiomostradatasecondi, "radio_orologiomostradatasecondi");
            radio_orologiomostradatasecondi.ForeColor = Color.White;
            radio_orologiomostradatasecondi.Name = "radio_orologiomostradatasecondi";
            radio_orologiomostradatasecondi.UseVisualStyleBackColor = true;
            radio_orologiomostradatasecondi.CheckedChanged += radio_orologio_CheckedChanged;
            // 
            // radio_orologiostandard
            // 
            resources.ApplyResources(radio_orologiostandard, "radio_orologiostandard");
            radio_orologiostandard.ForeColor = Color.White;
            radio_orologiostandard.Name = "radio_orologiostandard";
            radio_orologiostandard.UseVisualStyleBackColor = true;
            radio_orologiostandard.CheckedChanged += radio_orologio_CheckedChanged;
            // 
            // radio_orologiomostrasecondi
            // 
            resources.ApplyResources(radio_orologiomostrasecondi, "radio_orologiomostrasecondi");
            radio_orologiomostrasecondi.ForeColor = Color.White;
            radio_orologiomostrasecondi.Name = "radio_orologiomostrasecondi";
            radio_orologiomostrasecondi.UseVisualStyleBackColor = true;
            radio_orologiomostrasecondi.CheckedChanged += radio_orologio_CheckedChanged;
            // 
            // btnSettaggiExplorerVerdi
            // 
            resources.ApplyResources(btnSettaggiExplorerVerdi, "btnSettaggiExplorerVerdi");
            btnSettaggiExplorerVerdi.CheckButton = false;
            btnSettaggiExplorerVerdi.Checked = false;
            btnSettaggiExplorerVerdi.CheckedBackground = Color.White;
            btnSettaggiExplorerVerdi.CheckedForeColor = Color.White;
            btnSettaggiExplorerVerdi.CheckedImageTint = Color.White;
            btnSettaggiExplorerVerdi.CheckedOutline = Color.White;
            btnSettaggiExplorerVerdi.Content = "  Preferenze Explorer";
            btnSettaggiExplorerVerdi.DialogResult = DialogResult.None;
            btnSettaggiExplorerVerdi.ForeColor = Color.White;
            btnSettaggiExplorerVerdi.HoverBackground = Color.White;
            btnSettaggiExplorerVerdi.HoverForeColor = Color.White;
            btnSettaggiExplorerVerdi.HoverImageTint = Color.White;
            btnSettaggiExplorerVerdi.HoverOutline = Color.White;
            btnSettaggiExplorerVerdi.Image = Properties.Resources.pngExplorerPersonalizzazioneTweaks;
            btnSettaggiExplorerVerdi.ImageAutoCenter = true;
            btnSettaggiExplorerVerdi.ImageExpand = new Point(0, 0);
            btnSettaggiExplorerVerdi.ImageOffset = new Point(0, 0);
            btnSettaggiExplorerVerdi.Name = "btnSettaggiExplorerVerdi";
            btnSettaggiExplorerVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnSettaggiExplorerVerdi.NormalForeColor = Color.White;
            btnSettaggiExplorerVerdi.NormalImageTint = Color.White;
            btnSettaggiExplorerVerdi.NormalOutline = Color.White;
            btnSettaggiExplorerVerdi.OutlineThickness = 1F;
            btnSettaggiExplorerVerdi.PressedBackground = Color.White;
            btnSettaggiExplorerVerdi.PressedForeColor = Color.Black;
            btnSettaggiExplorerVerdi.PressedImageTint = Color.Black;
            btnSettaggiExplorerVerdi.PressedOutline = Color.White;
            btnSettaggiExplorerVerdi.Rounding = new Padding(8);
            btnSettaggiExplorerVerdi.TextAlignment = StringAlignment.Center;
            btnSettaggiExplorerVerdi.TextOffset = new Point(0, 0);
            btnSettaggiExplorerVerdi.Click += btnSettaggiExplorer_Click;
            // 
            // btnResetVerdi
            // 
            resources.ApplyResources(btnResetVerdi, "btnResetVerdi");
            btnResetVerdi.CheckButton = false;
            btnResetVerdi.Checked = false;
            btnResetVerdi.CheckedBackground = Color.FromArgb(0, 126, 249);
            btnResetVerdi.CheckedForeColor = Color.FromArgb(0, 126, 249);
            btnResetVerdi.CheckedImageTint = Color.FromArgb(0, 126, 249);
            btnResetVerdi.CheckedOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.Content = "  Reset";
            btnResetVerdi.DialogResult = DialogResult.None;
            btnResetVerdi.ForeColor = Color.White;
            btnResetVerdi.HoverBackground = Color.FromArgb(0, 126, 249);
            btnResetVerdi.HoverForeColor = Color.White;
            btnResetVerdi.HoverImageTint = Color.White;
            btnResetVerdi.HoverOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.Image = Properties.Resources.pngRipristinaDefenderTweaks;
            btnResetVerdi.ImageAutoCenter = true;
            btnResetVerdi.ImageExpand = new Point(0, 0);
            btnResetVerdi.ImageOffset = new Point(0, 0);
            btnResetVerdi.Name = "btnResetVerdi";
            btnResetVerdi.NormalBackground = Color.FromArgb(37, 38, 39);
            btnResetVerdi.NormalForeColor = Color.White;
            btnResetVerdi.NormalImageTint = Color.White;
            btnResetVerdi.NormalOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.OutlineThickness = 1F;
            btnResetVerdi.PressedBackground = Color.FromArgb(0, 126, 249);
            btnResetVerdi.PressedForeColor = Color.Black;
            btnResetVerdi.PressedImageTint = Color.Black;
            btnResetVerdi.PressedOutline = Color.FromArgb(0, 126, 249);
            btnResetVerdi.Rounding = new Padding(8);
            btnResetVerdi.TextAlignment = StringAlignment.Center;
            btnResetVerdi.TextOffset = new Point(0, 0);
            btnResetVerdi.Click += btn_resetselezione_Click;
            // 
            // cuiButton1Verdi
            // 
            resources.ApplyResources(cuiButton1Verdi, "cuiButton1Verdi");
            cuiButton1Verdi.CheckButton = false;
            cuiButton1Verdi.Checked = false;
            cuiButton1Verdi.CheckedBackground = Color.FromArgb(46, 125, 60);
            cuiButton1Verdi.CheckedForeColor = Color.FromArgb(46, 125, 60);
            cuiButton1Verdi.CheckedImageTint = Color.FromArgb(46, 125, 60);
            cuiButton1Verdi.CheckedOutline = Color.FromArgb(46, 125, 60);
            cuiButton1Verdi.Content = "  Avvia";
            cuiButton1Verdi.DialogResult = DialogResult.None;
            cuiButton1Verdi.ForeColor = Color.White;
            cuiButton1Verdi.HoverBackground = Color.FromArgb(46, 125, 50);
            cuiButton1Verdi.HoverForeColor = Color.White;
            cuiButton1Verdi.HoverImageTint = Color.White;
            cuiButton1Verdi.HoverOutline = Color.FromArgb(46, 125, 50);
            cuiButton1Verdi.Image = Properties.Resources.pngCheckCreaISO;
            cuiButton1Verdi.ImageAutoCenter = true;
            cuiButton1Verdi.ImageExpand = new Point(0, 0);
            cuiButton1Verdi.ImageOffset = new Point(0, 0);
            cuiButton1Verdi.Name = "cuiButton1Verdi";
            cuiButton1Verdi.NormalBackground = Color.FromArgb(37, 38, 39);
            cuiButton1Verdi.NormalForeColor = Color.White;
            cuiButton1Verdi.NormalImageTint = Color.White;
            cuiButton1Verdi.NormalOutline = Color.FromArgb(46, 125, 50);
            cuiButton1Verdi.OutlineThickness = 1F;
            cuiButton1Verdi.PressedBackground = Color.FromArgb(46, 125, 50);
            cuiButton1Verdi.PressedForeColor = Color.Black;
            cuiButton1Verdi.PressedImageTint = Color.Black;
            cuiButton1Verdi.PressedOutline = Color.FromArgb(46, 125, 50);
            cuiButton1Verdi.Rounding = new Padding(8);
            cuiButton1Verdi.TextAlignment = StringAlignment.Center;
            cuiButton1Verdi.TextOffset = new Point(0, 0);
            cuiButton1Verdi.Click += btnAvviaSelezionati_Click;
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
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ForeColor = Color.FromArgb(0, 126, 249);
            label3.Name = "label3";
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // FormPersonalizzazione
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(panello);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormPersonalizzazione";
            Load += FormPersonalizzazione_Load;
            panello.ResumeLayout(false);
            panello.PerformLayout();
            panel76.ResumeLayout(false);
            panel76.PerformLayout();
            panel77.ResumeLayout(false);
            panel77.PerformLayout();
            panel78.ResumeLayout(false);
            panel78.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel32;
        private Panel panel4;
        private Panel panel7;
        private Label label2;
        private ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel panello;
        private CuoreUI.Controls.cuiButton btnSettaggiExplorerVerdi;
        private CuoreUI.Controls.cuiButton btnResetVerdi;
        private CuoreUI.Controls.cuiButton cuiButton1Verdi;
        private CuoreUI.Controls.cuiProgressBarHorizontal progressBar1;
        private Label label1;
        private Label label4;
        private Label label3;
        private Panel panel78;
        private CheckBox radio_orologionascondioradata;
        private CheckBox radio_orologiomostraoradata;
        private CheckBox radio_orologiomostradatasecondi;
        private CheckBox radio_orologiostandard;
        private CheckBox radio_orologiomostrasecondi;
        private Panel panel77;
        private CheckBox radio_disacopilot;
        private CheckBox radio_abilicopilot;
        private CheckBox radio_disattivafx;
        private CheckBox radio_attivafx;
        private CheckBox radio_disabilitarecall;
        private CheckBox radio_abilitarecall;
        private CheckBox radio_disabilitasuggeriti;
        private CheckBox radio_abilitasuggeriti;
        private CheckBox radio_disabilitaottimizzaricerca;
        private CheckBox radio_disabilitaricercainternet;
        private CheckBox radio_abilitaRicercainternet;
        private CheckBox radio_ottimizzaricerca;
        private Panel panel76;
        private CheckBox radio_disabilitaendtask;
        private CheckBox radio_abilitaendtask;
        private CheckBox radio_eliminapowershell;
        private CheckBox radio_apripowershell;
        private CheckBox radio_eliminaapricmd;
        private CheckBox radio_destrolegacy;
        private CheckBox radio_destrodefault;
        private CheckBox radio_apricmd;
    }
}