using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinHubX.Impostazioni;

namespace WinHubX.DialogBlock
{
    public partial class Form_DialogBlock : Form
    {
        private readonly Form1 mainForm;

        public Form_DialogBlock(Form1 form)
        {
            InitializeComponent();
            mainForm = form ?? throw new ArgumentNullException(nameof(form), "Devi passare un riferimento a Form1!");
        }

        private void btnVerificaVerdi_Click(object sender, EventArgs e)
        {
            if (mainForm.btnHome == null)
            {
                MessageBox.Show("btnHome non è inizializzato!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            mainForm.LoadForm(new FormHome(), mainForm.btnHome, "Home");
            this.Close();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
