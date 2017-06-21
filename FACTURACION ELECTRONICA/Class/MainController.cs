using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Configuration;
using System.ServiceModel;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;
using System.Xml.Serialization;
using FACT_ELECTRONICA_PDF_XML;

namespace FACT_ELECTRONICA_PDF_XML
{
    class MainController
    {
        private Random _random;
        private View.IMain _view;
        public Utils.Utilerias Util = new Utils.Utilerias();
        DataRetrieve dataRetrieve = new DataRetrieve();

        public MainController(View.IMain view)
        {
            _view = view;
            _view.void_GeneraXML += void_GeneraXML;
            _view.void_CancelaXML += void_CancelaXML;
            _random = new Random();
        }
        public string[][] LookupShippersForCFDI()
        {
            DataSet dataSetLookUp = new DataSet();

            dataSetLookUp = dataRetrieve.SelectDataRetrieve("select * from xCFDIXML Where Status = 'O'", Util.ConnectionString);
            string[][] sShipperIdForCFDI = new string[dataSetLookUp.Tables[0].Rows.Count][];//{new string[dataSetLookUp.Tables[0].Rows.Count], new string[2]};
            int i = 0;
            foreach (DataRow dr in dataSetLookUp.Tables[0].Rows)
            {
                //Cancelled, InvcDate, InvcNbr, RFC, ShipperId
                sShipperIdForCFDI[i] = new string[6];
                sShipperIdForCFDI[i][0] = dr["Cancelled"].ToString();
                sShipperIdForCFDI[i][1] = dr["InvcDate"].ToString();
                _view.InvcDate = dr["InvcDate"].ToString();
                sShipperIdForCFDI[i][2] = dr["InvcNbr"].ToString();
                _view.InvcNbr = dr["InvcNbr"].ToString();
                sShipperIdForCFDI[i][3] = dr["RFC"].ToString();
                _view.RFCReceptor = dr["RFC"].ToString();
                sShipperIdForCFDI[i][4] = dr["ShipperId"].ToString();
                _view.ShipperIdStr = dr["ShipperId"].ToString();
                sShipperIdForCFDI[i][5] = dr["UUID"].ToString();
                if (dr["UUID"].ToString() != "")
                    _view.UUID = new System.Guid(dr["UUID"].ToString());
                i++;
            }
            return sShipperIdForCFDI;
        }
        void void_CancelaXML(object sender, EventArgs e)
        {
            using (var Cancelacion = new SrvCancelacion.CancelacionesClient())
            {
                long trsId = _random.Next();
                string token = string.Empty;

                string Error;
                //Utils.Utilerias Util = new Utils.Utilerias(_view);
                Error = Util.CreateToken(Util.RFC, trsId, ref token);
                if (!string.IsNullOrEmpty(Error))
                {
                    _view.ShowUnknownError(Error);
                    return;
                }
                try
                {
                    var respuesta = Cancelacion.CancelaOtros(Util.RFC, _view.RFCReceptor, token, ref trsId, _view.UUID);
                    foreach (var item in respuesta)
                    {
                        _view.Estatus = item.Estatus;
                        _view.UUID = item.UUID;
                        dataRetrieve.SelectDataRetrieve("update xCFDIXML set status = 'V' where shipperid = '" + _view.ShipperIdStr + "'", Util.ConnectionString);
                    }
                }
                catch (FaultException<SrvCancelacion.FallaValidacion> validationFault)
                {
                    using (var errorForm = new Forms.Error(TranslateFault.ToValidationError(validationFault.Detail)))
                    {
                        errorForm.ShowDialog();
                    }
                }
                catch (FaultException<SrvCancelacion.FallaServicio> serviceFault)
                {
                    using (var errorForm = new Forms.Error(TranslateFault.ToServiceError(serviceFault.Detail)))
                    {
                        errorForm.ShowDialog();
                    }
                }
                catch (FaultException<SrvCancelacion.FallaSesion> sessionFault)
                {
                    using (var errorForm = new Forms.Error(TranslateFault.ToSessionError(sessionFault.Detail)))
                    {
                        errorForm.ShowDialog();
                    }
                }
                catch (Exception ex)
                { _view.ShowUnknownError(ex.Message); }

            }
        }
        void void_GeneraXML(object sender, EventArgs e)
        {
            String sShipperId = _view.ShipperIdStr;
            DataSet datasetCFDI_XML = new DataSet();

            long transactionID = _random.Next();
            var token = String.Empty;
            string Error;

            ConvertToXML convertToXML = new ConvertToXML();
            MemoryStream memoryStreamXML = new MemoryStream();

            string sXMLPath = Util.XMLPath;
            
            string sQRCodePath = Util.QRCodePath;
            try
            {
                dataRetrieve.SelectDataRetrieve("update xCFDIXML set Start_DateTime = GETDATE() where shipperid = '" + _view.ShipperIdStr + "'", Util.ConnectionString);
                XmlTextWriter writerXML = new XmlTextWriter(memoryStreamXML, new UTF8Encoding(false));
                writerXML.WriteStartDocument();

                writerXML.WriteStartElement("cfdi", ConvertToXML.sNivel0, "http://www.sat.gob.mx/cfd/3");
                writerXML.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writerXML.WriteAttributeString("xsi", "schemaLocation", null, "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv32.xsd");


                datasetCFDI_XML = dataRetrieve.SelectDataRetrieve("select * from xvr_CFDI_XML Where [null/ShipperId] = '" + sShipperId + "'", Util.ConnectionString);
                writerXML = convertToXML.fAddLevesXML(datasetCFDI_XML, writerXML, "", "");
                
                
                
                writerXML.Flush();
                writerXML.Close();

                string XMLString = Encoding.UTF8.GetString(memoryStreamXML.ToArray());
                XDocument xmlParser = new XDocument();
                xmlParser = XDocument.Parse(XMLString);

                dataRetrieve.SelectDataRetrieve("update xCFDIXML set status = 'P', XMLParser = '" + xmlParser.ToString() + "' where shipperid = '" + _view.ShipperIdStr + "'", Util.ConnectionString);

                #region READ XML AND SEND TO SELLADO Y TIMBRADO
                Error = Util.CreateToken(Util.RFC, transactionID, ref token);
                if (!string.IsNullOrEmpty(Error))
                {
                    _view.ShowUnknownError(Error);
                    return;
                }

                transactionID = _random.Next();
                var invoiceXML = new SrvInvoices.ComprobanteXML();
                invoiceXML.DatosXML = XMLString;

                using (var invoiceSrv = new SrvInvoices.ComprobantesClient())
                {
                    var advertencias = invoiceSrv.SellaTimbraXML(ref invoiceXML, Util.RFC, token, ref transactionID);
                }
                transactionID = _random.Next();
                XDocument XMLTimbradoSelladoFirmado = new XDocument();
                XMLTimbradoSelladoFirmado = XDocument.Parse(invoiceXML.DatosXML);

                XElement signatureSAT = new XElement("{http://www.sat.gob.mx/TimbreFiscalDigital}TimbreFiscalDigital");
                string uuid = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("UUID").Value;
                string FechaTimbrado = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("FechaTimbrado").Value;
                string selloCFD = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("selloCFD").Value;
                string noCertificadoSAT = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("noCertificadoSAT").Value;
                string selloSAT = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("selloSAT").Value;

                dataRetrieve.SelectDataRetrieve("update xCFDIXML set status = 'U', UUID='" + uuid + "', XMLSAT = '" + XMLTimbradoSelladoFirmado.ToString() + "', InvcDate = '" + FechaTimbrado + "' where shipperid = '" + _view.ShipperIdStr + "'", Util.ConnectionString);
                dataRetrieve.SelectDataRetrieve("update SoShipHeader set InvcDate = '" + FechaTimbrado + "' where shipperid = '" + _view.ShipperIdStr + "'", Util.ConnectionString);
                #endregion

                #region READ DATASET FROM XML
                DataSet dsXML = new DataSet();
                string AppPathXSD = AppDomain.CurrentDomain.BaseDirectory;
                AppPathXSD = AppPathXSD + @"\Reports\cfdv32_EGSA.xsd";
                dsXML.ReadXmlSchema(AppPathXSD);
                dsXML.ReadXml(new StringReader(XMLTimbradoSelladoFirmado.ToString()));

                string sSerie;
                string sFolio;
                string sFactura;
                string sRFCCliente;

                sSerie = dsXML.Tables["Comprobante"].Rows[0]["serie"].ToString();
                sFolio = dsXML.Tables["Comprobante"].Rows[0]["folio"].ToString();
                sRFCCliente = dsXML.Tables["Receptor"].Rows[0]["rfc"].ToString();
                sFactura = sSerie + sFolio;
                #endregion


                #region CREATE QRCODE
                using (var repositorySrv = new SrvRepository.RepositorioClient())
                {
                    var infoQR = repositorySrv.ObtenerQR(Util.RFC, token, ref transactionID, uuid);

                    var memoryStream = new System.IO.MemoryStream(infoQR.Imagen);
                    _view.BarcodeQR = new System.Drawing.Bitmap(memoryStream);
                    System.Drawing.Bitmap imageQR = new System.Drawing.Bitmap(memoryStream);
                    imageQR.Save(sQRCodePath + sFactura + ".bmp");
                    _view.ResultStr = XMLTimbradoSelladoFirmado.ToString();
                    dataRetrieve.SelectDataRetrieve("update xCFDIXML set status = 'Q' where shipperid = '" + _view.ShipperIdStr + "'", Util.ConnectionString);
                }
                #endregion

                #region SAVE XML TO DISK
                StringBuilder lsOutputFile = new StringBuilder(string.Format(@"{0}\{1}{2}{3}", sXMLPath, sRFCCliente, sFactura, ".XML"));
                StringBuilder loXML = new StringBuilder();
                loXML.Append(XMLTimbradoSelladoFirmado.ToString());
                using (StreamWriter loSW = new StreamWriter(lsOutputFile.ToString(), false, Encoding.UTF8))
                {
                    loSW.Write(loXML.ToString());
                }
                dataRetrieve.SelectDataRetrieve("update xCFDIXML set status = 'X' where shipperid = '" + _view.ShipperIdStr + "'", Util.ConnectionString);
                #endregion

                CreatePDF crearPDF = new CreatePDF();
                crearPDF.fCreatePDF(dsXML, invoiceXML.DatosXML, _view.ShipperIdStr);
            }

            #region Invoice service exceptions
            catch (FaultException<SrvInvoices.FallaServicio> serviceFault)
            {
                using (var errorForm = new Forms.Error(TranslateFault.ToServiceError(serviceFault.Detail)))
                {
                    errorForm.ShowDialog();
                }
            }
            catch (FaultException<SrvInvoices.FallaSesion> sessionFauld)
            {
                using (var errorForm = new Forms.Error(TranslateFault.ToSessionError(sessionFauld.Detail)))
                {
                    errorForm.ShowDialog();
                }
            }
            catch (FaultException<SrvInvoices.FallaValidacion> validationFault)
            {
                using (var errorForm = new Forms.Error(TranslateFault.ToValidationError(validationFault.Detail)))
                {
                    errorForm.ShowDialog();
                }
            }
            #endregion
            //catch (Exception ex)
            //{
            //    _view.ShowUnknownError(ex.Message);
            //}
        }
    }
}
