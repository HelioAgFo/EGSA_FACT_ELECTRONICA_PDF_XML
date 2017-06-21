using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using FACT_ELECTRONICA_PDF_XML.Utils;
using System.IO;
using System.Xml.Linq;

namespace FACT_ELECTRONICA_PDF_XML
{
    public partial class form_Administrador_CFDI : Form
    {
        private Utilerias Util = new Utilerias();
        DataRetrieve dataRetrieve = new DataRetrieve();
        string uuid;
        string FechaTimbrado;
        string selloCFD;
        string noCertificadoSAT;
        string selloSAT;
        string sInvcNbr;
        string sXMLSAT;

        public form_Administrador_CFDI()
        {
            InitializeComponent();
        }

        private void txtboxShipperId_Leave(object sender, EventArgs e)
        {
            DataSet dataSetStatus = new DataSet();
            dataSetStatus = dataRetrieve.SelectDataRetrieve("Select Status, XMLSAT, InvcNbr From xCFDIXML Where ShipperID = '" + txtboxShipperId.Text.Trim() + "'", Util.ConnectionString);
            foreach (DataRow dr in dataSetStatus.Tables[0].Rows)
            {
                XDocument XMLTimbradoSelladoFirmado = new XDocument();
                sXMLSAT = dr[1].ToString();
                XMLTimbradoSelladoFirmado = XDocument.Parse(sXMLSAT);

                XElement signatureSAT = new XElement("{http://www.sat.gob.mx/TimbreFiscalDigital}TimbreFiscalDigital");
                uuid = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("UUID").Value;
                FechaTimbrado = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("FechaTimbrado").Value;
                selloCFD = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("selloCFD").Value;
                noCertificadoSAT = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("noCertificadoSAT").Value;
                selloSAT = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("selloSAT").Value;
                sInvcNbr = dr[2].ToString();
                switch (dr[0].ToString())
                {
                    case "O": lblStatusCFDI.Text = "O - XML OPEN - INICIO DE XML";
                        btnGenerarPDF.Enabled = false;
                        break;
                    case "P": lblStatusCFDI.Text = "P - XML PARSER";
                        btnGenerarPDF.Enabled = false;
                        break;
                    case "U": lblStatusCFDI.Text = "U - XML SAT";
                        btnGenerarPDF.Enabled = true;
                        break;
                    case "Q": lblStatusCFDI.Text = "Q - XML QRCODE";
                        btnGenerarPDF.Enabled = true;
                        break;
                    case "X": lblStatusCFDI.Text = "X - XML SAVED ON DISC";
                        btnGenerarPDF.Enabled = true;
                        break;
                    case "C": lblStatusCFDI.Text = "C - COMPLETED";
                        btnGenerarPDF.Enabled = true;
                        break;
                    case "V": lblStatusCFDI.Text = "V - VOID";
                        btnGenerarPDF.Enabled = false;
                        break;
                }
            }
        }

        private void btnGenerarPDF_Click(object sender, EventArgs e)
        {
            DataSet dsXML = new DataSet();
            string AppPathXSD = AppDomain.CurrentDomain.BaseDirectory;
            AppPathXSD = AppPathXSD + @"\Reports\cfdv32_EGSA.xsd";
            dsXML.ReadXmlSchema(AppPathXSD);
            dsXML.ReadXml(new StringReader(sXMLSAT.ToString()));

            CreatePDF crearPDF = new CreatePDF();
            crearPDF.fCreatePDF(dsXML, sXMLSAT, txtboxShipperId.Text);
        }


    }
}
