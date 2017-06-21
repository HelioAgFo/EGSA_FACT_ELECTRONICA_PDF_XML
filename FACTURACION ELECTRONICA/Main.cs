using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FACT_ELECTRONICA_PDF_XML
{
    public partial class form_Main : Form
    {
        public form_Main()
        {
            InitializeComponent();
        }

        private void administradorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form_Administrador_CFDI form_AdministradorCFDI = new form_Administrador_CFDI();
            form_AdministradorCFDI.Show();
        }

        private void procesamientoCFDIsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form_CFDI_Process_Manager form_ProcessManager = new form_CFDI_Process_Manager();
            form_ProcessManager.Show();
        }
    }
}
