using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using FACT_ELECTRONICA_PDF_XML.Utils;
using System.Xml.Linq;

namespace FACT_ELECTRONICA_PDF_XML
{
    class CreatePDF
    {
        DataRetrieve dataRetrieve = new DataRetrieve();
        private Utilerias Util = new Utilerias();
        public void fCreatePDF(DataSet dsXML, string sXMLSAT, string sShipperId)
        {
            XDocument XMLTimbradoSelladoFirmado = new XDocument();
            XMLTimbradoSelladoFirmado = XDocument.Parse(sXMLSAT);

            XElement signatureSAT = new XElement("{http://www.sat.gob.mx/TimbreFiscalDigital}TimbreFiscalDigital");
            string uuid = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("UUID").Value;
            string FechaTimbrado = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("FechaTimbrado").Value;
            string selloCFD = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("selloCFD").Value;
            string noCertificadoSAT = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("noCertificadoSAT").Value;
            string selloSAT = XMLTimbradoSelladoFirmado.Root.Descendants(signatureSAT.Name).ToArray()[0].Attribute("selloSAT").Value;

        #region FILL DATASET WITH ADDITIONAL INFO FROM HEADER
                DataSet datasetComplementoEGSA = new DataSet();
                DataRow drEGSA = dsXML.Tables["EGSA"].NewRow();
                string sComplementoEGSAHeader;

                sComplementoEGSAHeader = "select * from xvr_ComplementoEGSAHeader where ShipperId = '" + sShipperId + "'";
                datasetComplementoEGSA = dataRetrieve.SelectDataRetrieve(sComplementoEGSAHeader, Util.ConnectionString);
                foreach (DataRow dr in datasetComplementoEGSA.Tables[0].Rows)
                    foreach (DataColumn dc in datasetComplementoEGSA.Tables[0].Columns)
                        drEGSA[dc.ColumnName] = dr[dc, DataRowVersion.Current].ToString();
                drEGSA["Comprobante_Id"] = dsXML.Tables["Comprobante"].Rows[0]["Comprobante_Id"].ToString();
                dsXML.Tables["EGSA"].Rows.Add(drEGSA);
                #endregion

                #region FILL DATASET WITH ADDITIONAL INFO FROM LINE
                DataSet datasetComplementoEGSALine = new DataSet();
                DataTable dataTableLineXML = new DataTable();
                DataTable dataTableLineNotes = new DataTable();

                string sComplementoEGSALine;
                string sLotSerNbr;
                string sTipoLoteSerie;
                string sLineRefComplemento;
                List<string> lLotSerNbrByLineRef = new List<string>();
                string sInvtId;
                string sInvtIdXML = "";
                string sDescr;
                string sDescrXML = "";
                string sQtyShip;
                string sQtyShipXML;
                int iQtyShipXML = 0;
                int iQtyShipLotSer;
                int iRowCount;
                int i;
                string sPDFPath = Util.PDFPath;

                string sSerie = dsXML.Tables["Comprobante"].Rows[0]["serie"].ToString();
                string sFolio = dsXML.Tables["Comprobante"].Rows[0]["folio"].ToString();
                string sRFCCliente = dsXML.Tables["Receptor"].Rows[0]["rfc"].ToString();
                string sFactura = sSerie + sFolio;

                sComplementoEGSALine = "select * from xvr_ComplementoEGSALine where Sample = 0 and ShipperId = '" + sShipperId + "' Order By LineRef";
                datasetComplementoEGSA = dataRetrieve.SelectDataRetrieve(sComplementoEGSALine, Util.ConnectionString);

                dataTableLineNotes = datasetComplementoEGSA.Tables[0];
                dataTableLineXML = dsXML.Tables["Concepto"];


                if (dataTableLineNotes.Rows.Count > 0)
                {
                    sInvtId = dataTableLineNotes.Rows[0]["InvtId"].ToString().Trim().ToUpper();
                    sDescr = dataTableLineNotes.Rows[0]["Descr"].ToString().Trim().ToUpper();
                    sQtyShip = dataTableLineNotes.Rows[0]["QtyShip"].ToString().Trim().ToUpper();
                    iQtyShipLotSer = Convert.ToInt32(Math.Round(Convert.ToDecimal(sQtyShip, System.Globalization.CultureInfo.InvariantCulture)));
                    lLotSerNbrByLineRef.Add(dataTableLineNotes.Rows[0]["LotSerNbr"].ToString().Trim().ToUpper());
                    sTipoLoteSerie = dataTableLineNotes.Rows[0]["LotSerTrack"].ToString().Trim().ToUpper();
                    sLotSerNbr = dataTableLineNotes.Rows[0]["sNoteText"].ToString();

                    iRowCount = 0;
                    sInvtIdXML = dataTableLineXML.Rows[iRowCount]["noIdentificacion"].ToString().Trim();
                    sDescrXML = dataTableLineXML.Rows[iRowCount]["descripcion"].ToString().Trim();
                    sQtyShipXML = dataTableLineXML.Rows[iRowCount]["cantidad"].ToString().Trim();
                    iQtyShipXML = Convert.ToInt32(Math.Round(Convert.ToDecimal(sQtyShipXML, System.Globalization.CultureInfo.InvariantCulture)));
                    
                    sLineRefComplemento = "";
                    for (i = 0; i < dataTableLineNotes.Rows.Count; i++)
                    {
                        if (sLineRefComplemento != dataTableLineNotes.Rows[i]["LineRef"].ToString())
                        {
                            sLineRefComplemento = dataTableLineNotes.Rows[i]["LineRef"].ToString();
                            if (sInvtIdXML == sInvtId && sDescrXML == sDescr && iQtyShipXML == iQtyShipLotSer)
                            {
                                if (lLotSerNbrByLineRef[0].ToString().Trim() != "")
                                    sLotSerNbr = sTipoLoteSerie + string.Join(", ", lLotSerNbrByLineRef) + "\r\n" + sLotSerNbr;
                                dsXML.Tables["Concepto"].Rows[iRowCount]["Notas"] = sLotSerNbr;
                                lLotSerNbrByLineRef.Clear();

                                if (iRowCount + 1 < dataTableLineXML.Rows.Count)
                                    iRowCount++;
                                sInvtIdXML = dataTableLineXML.Rows[iRowCount]["noIdentificacion"].ToString().Trim();
                                sDescrXML = dataTableLineXML.Rows[iRowCount]["descripcion"].ToString().Trim();
                                sQtyShipXML = dataTableLineXML.Rows[iRowCount]["cantidad"].ToString().Trim();
                                iQtyShipXML = Convert.ToInt32(Math.Round(Convert.ToDecimal(sQtyShipXML, System.Globalization.CultureInfo.InvariantCulture)));


                                sInvtId = dataTableLineNotes.Rows[i]["InvtId"].ToString().Trim().ToUpper();
                                sDescr = dataTableLineNotes.Rows[i]["Descr"].ToString().Trim().ToUpper();
                                sQtyShip = dataTableLineNotes.Rows[i]["QtyShip"].ToString().Trim().ToUpper();
                                iQtyShipLotSer = Convert.ToInt32(Math.Round(Convert.ToDecimal(sQtyShip, System.Globalization.CultureInfo.InvariantCulture)));
                                lLotSerNbrByLineRef.Add(dataTableLineNotes.Rows[i]["LotSerNbr"].ToString().Trim().ToUpper());
                                sTipoLoteSerie = dataTableLineNotes.Rows[i]["LotSerTrack"].ToString().Trim().ToUpper();
                                sLotSerNbr = dataTableLineNotes.Rows[i]["sNoteText"].ToString();
                            }
                            else
                            {
                                sInvtId = dataTableLineNotes.Rows[i]["InvtId"].ToString().Trim().ToUpper();
                                sDescr = dataTableLineNotes.Rows[i]["Descr"].ToString().Trim().ToUpper();
                                sQtyShip = dataTableLineNotes.Rows[i]["QtyShip"].ToString().Trim().ToUpper();
                                iQtyShipLotSer = Convert.ToInt32(Math.Round(Convert.ToDecimal(sQtyShip, System.Globalization.CultureInfo.InvariantCulture)));
                                lLotSerNbrByLineRef.Add(dataTableLineNotes.Rows[i]["LotSerNbr"].ToString().Trim().ToUpper());
                                sTipoLoteSerie = dataTableLineNotes.Rows[i]["LotSerTrack"].ToString().Trim().ToUpper();
                            }

                        }
                        else
                        {
                            iQtyShipLotSer = iQtyShipLotSer + Convert.ToInt32(Math.Round(Convert.ToDecimal(sQtyShip, System.Globalization.CultureInfo.InvariantCulture)));
                            lLotSerNbrByLineRef.Add(dataTableLineNotes.Rows[i]["LotSerNbr"].ToString());
                        }
                    }
                    if (sInvtIdXML == sInvtId.Trim() && sDescrXML == sDescr.Trim() && iQtyShipXML == iQtyShipLotSer)
                    {
                        sLotSerNbr = dataTableLineNotes.Rows[i - 1]["LotSerNbr"].ToString().Trim().ToUpper();
                        sTipoLoteSerie = dataTableLineNotes.Rows[i - 1]["LotSerTrack"].ToString().Trim().ToUpper();
                        if (sLotSerNbr != "")
                            sLotSerNbr = sTipoLoteSerie + string.Join(", ", lLotSerNbrByLineRef) + "\r\n" + dataTableLineNotes.Rows[i - 1]["sNoteText"].ToString();
                        else
                            sLotSerNbr = dataTableLineNotes.Rows[i - 1]["sNoteText"].ToString();
                        dsXML.Tables["Concepto"].Rows[iRowCount]["Notas"] = sLotSerNbr;
                    }
                    lLotSerNbrByLineRef.Clear();
                }


                #endregion



                #region CREATE CRYSTAL REPORT, ASSIGN VARIABLES FROM SAT & EXPORT TO PDF
                ReportDocument cryRpt = new ReportDocument();
                string AppPathCrystal = AppDomain.CurrentDomain.BaseDirectory;
                if (sRFCCliente.Trim() == "XEXX010101000")
                    AppPathCrystal = AppPathCrystal + @"\Reports\CFDIEGSAE.rpt";
                else
                    AppPathCrystal = AppPathCrystal + @"\Reports\CFDIEGSA.rpt";
                cryRpt.Load(AppPathCrystal); // se carga el crystalrepor para poder escribir en el
                cryRpt.SetDataSource(dsXML);
                cryRpt.SetParameterValue("ShipperId", sShipperId.ToUpper());
                cryRpt.SetParameterValue("uuid", uuid);
                cryRpt.SetParameterValue("FechaTimbrado", FechaTimbrado);
                cryRpt.SetParameterValue("selloCFD", selloCFD);
                cryRpt.SetParameterValue("noCertificadoSAT", noCertificadoSAT);
                cryRpt.SetParameterValue("selloSAT", selloSAT);

                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, sPDFPath + sFactura + ".pdf"); // ubicacion donde se guardaran los arcivos
                dataRetrieve.SelectDataRetrieve("update xCFDIXML set End_DateTime = GETDATE(), status = 'C' where shipperid = '" + sShipperId + "'", Util.ConnectionString);
                cryRpt.Close();
                cryRpt.Dispose();
                #endregion
        }
    }
}
