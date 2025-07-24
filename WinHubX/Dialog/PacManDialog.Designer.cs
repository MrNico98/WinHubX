namespace WinHubX.Dialog
{
    partial class PacManDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PacManDialog));
            label1 = new Label();
            btnInstallaPacMan = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ForeColor = Color.Coral;
            label1.Name = "label1";
            // 
            // btnInstallaPacMan
            // 
            resources.ApplyResources(btnInstallaPacMan, "btnInstallaPacMan");
            btnInstallaPacMan.Cursor = Cursors.Hand;
            btnInstallaPacMan.FlatAppearance.BorderSize = 0;
            btnInstallaPacMan.ForeColor = Color.White;
            btnInstallaPacMan.Name = "btnInstallaPacMan";
            btnInstallaPacMan.UseVisualStyleBackColor = true;
            btnInstallaPacMan.Click += btnInstallaPacMan_Click;
            // 
            // PacManDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(37, 38, 39);
            Controls.Add(btnInstallaPacMan);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "PacManDialog";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btnInstallaPacMan;
    }
}