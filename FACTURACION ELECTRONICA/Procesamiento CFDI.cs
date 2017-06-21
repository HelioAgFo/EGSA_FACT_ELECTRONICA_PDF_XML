using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;

namespace FACT_ELECTRONICA_PDF_XML
{
    public partial class form_CFDI_Process_Manager : Form, View.IMain
    {
        MainController timbradoCFD;
        private bool timerRunning;
        private bool CFDIRunning;
        Timer timer = new Timer();
        public form_CFDI_Process_Manager()
        {
            InitializeComponent();
            timerRunning = false;
            CFDIRunning = false;
            timer.Interval = 3000;
            timer.Tick += new EventHandler(timer_Tick);
            timbradoCFD = new MainController(this);
        }

        #region IMain implementation

        public string ResultStr
        {
            set { this.txtboxMessages.Text = value; }
        }

        public Bitmap BarcodeQR
        {
            set { this.barcodeQR.Image = value; }
        }

        public event EventHandler void_GeneraXML;
        public event EventHandler void_CancelaXML;

        public string ShipperIdStr
        {
            get { return this.txtboxShipperId.Text; }
            set { this.txtboxShipperId.Text = value; }
        }

        public string RFCReceptor
        {
            get { return this.lRFC.Text; }
            set { this.lRFC.Text = value; }
        }

        public string InvcNbr
        {
            get { return this.lInvcNbr.Text; }
            set { this.lInvcNbr.Text = value; }
        }

        public string InvcDate
        {
            get { return this.lInvcDate.Text; }
            set { this.lInvcDate.Text = value; }
        }
        public System.Guid UUID
        {
            get
            {
                Guid UUID = Guid.Empty;
                string sUUID = this.lUUID.Text.ToString();
                UUID = new Guid(sUUID);
                return UUID;
            }
            set
            {
                this.lUUID.Text = value.ToString();
            }
        }
        public string Estatus
        {
            get { return this.lStatus.Text; }
            set { this.lStatus.Text = value; }
        }

        public void ShowSessionError(Entity.SessionError error)
        {
            MessageBox.Show(String.Format("Status {0}: {1}", error.Status.ToString(), error.Description), "CFDI - SESSION ERROR");
        }

        public void ShowUnknownError(String message)
        {
            MessageBox.Show(String.Format("Error: {0}", message), "CFDI - UNKNOWN ERROR");
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            if (timerRunning)
            {
                timer.Stop();
                timerRunning = false;
                btn_StartProcess.Text = "START";
            }
            else
            {
                timer.Start();
                timerRunning = true;
                timer_Tick(this, EventArgs.Empty);
                btn_StartProcess.Text = "STOP";
                Refresh();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!CFDIRunning)
            {
                string[][] sShipperIdForCFDI = timbradoCFD.LookupShippersForCFDI();
                foreach (string[] st in sShipperIdForCFDI)
                {
                    CFDIRunning = true;
                    txtboxShipperId.Text = st[4].ToString();
                    if (st[0].ToString() == "1")
                        void_CancelaXML(this, EventArgs.Empty);
                    else
                        void_GeneraXML(this, EventArgs.Empty);
                    Refresh();
                    txtboxShipperId.Text = "";
                    txtboxMessages.Text = "";
                    barcodeQR.Image = null;
                    Refresh();
                    Application.DoEvents();
                }
                CFDIRunning = false;
            }
            //throw new NotImplementedException();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
