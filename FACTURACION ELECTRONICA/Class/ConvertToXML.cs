using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Data;

namespace FACT_ELECTRONICA_PDF_XML
{
    class ConvertToXML
    {
        public Utils.Utilerias Util = new Utils.Utilerias();
        public static string sNivel0 = "Comprobante";
        public int iLevelAnt = 0;
        private string[] fNivelesCol(string sColumnName)
        {
            string[] sColumNameSplit = sColumnName.Split(new Char[] { '/' });
            return sColumNameSplit;
        }

        public XmlTextWriter fAddLevesXML(DataSet datasetCFDI_XML, XmlTextWriter writerXML, string sActualColumnLevel_2, string sActualColumnLevel_3)
        {
            string sCellValue;
            string sActualColumnLevel_4;
            sActualColumnLevel_4 = "";
            foreach (DataRow datarowCFDI_XML in datasetCFDI_XML.Tables[0].Rows)
            {
                sActualColumnLevel_4 = "";
                foreach (DataColumn datacolCFDI_XML in datasetCFDI_XML.Tables[0].Columns)
                {
                    string[] sColumnNameSplit = fNivelesCol(datacolCFDI_XML.ColumnName);
                    sCellValue = datarowCFDI_XML[datacolCFDI_XML, DataRowVersion.Current].ToString();
                    if (sColumnNameSplit[0] != "null" && sCellValue != "")
                    {
                        switch (sColumnNameSplit.LongLength)
                        {
                            case 1:
                                writerXML.WriteAttributeString(sColumnNameSplit[0], sCellValue);
                                sActualColumnLevel_2 = sNivel0;
                                iLevelAnt = 1;
                                break;
                            case 2:
                                if (sActualColumnLevel_2 != sColumnNameSplit[0])
                                {
                                    for (int i = 0; i < iLevelAnt - 1; i++)
                                        writerXML.WriteEndElement();
                                    sActualColumnLevel_2 = sColumnNameSplit[0];
                                    writerXML.WriteStartElement("cfdi:" + sActualColumnLevel_2);
                                    if (sColumnNameSplit[1] != "null")
                                        writerXML.WriteAttributeString(sColumnNameSplit[1], sCellValue);
                                }
                                else if (sActualColumnLevel_2 == sColumnNameSplit[0])
                                {
                                    if (sColumnNameSplit[1] != "null")
                                        writerXML.WriteAttributeString(sColumnNameSplit[1], sCellValue);
                                }
                                iLevelAnt = 2;
                                break;
                            case 3:
                                if (sActualColumnLevel_2 == sColumnNameSplit[0] && sActualColumnLevel_3 != sColumnNameSplit[1])
                                {
                                    if (iLevelAnt == 3)
                                        writerXML.WriteEndElement();
                                    if (iLevelAnt == 4)
                                    {
                                        writerXML.WriteEndElement();
                                        writerXML.WriteEndElement();
                                    }
                                    sActualColumnLevel_3 = sColumnNameSplit[1];
                                    writerXML.WriteStartElement("cfdi:" + sActualColumnLevel_3);
                                    if (sColumnNameSplit[2] != "null")
                                        writerXML.WriteAttributeString(sColumnNameSplit[2], sCellValue);
                                }
                                else if (sActualColumnLevel_2 == sColumnNameSplit[0] && sActualColumnLevel_3 == sColumnNameSplit[1])
                                {
                                    if (sColumnNameSplit[2] != "null")
                                        writerXML.WriteAttributeString(sColumnNameSplit[2], sCellValue);
                                }
                                else if (sActualColumnLevel_2 == sColumnNameSplit[0])
                                {
                                    sActualColumnLevel_3 = sColumnNameSplit[1];
                                    writerXML.WriteStartElement("cfdi:" + sActualColumnLevel_3);
                                    if (sColumnNameSplit[2] != "null")
                                        writerXML.WriteAttributeString(sColumnNameSplit[2], sCellValue);
                                }
                                iLevelAnt = 3;
                                break;
                            case 4:
                                if (sActualColumnLevel_2 == sColumnNameSplit[0] && sActualColumnLevel_3 == sColumnNameSplit[1] && sActualColumnLevel_4 != sColumnNameSplit[2])
                                {
                                    if (iLevelAnt == 4)
                                        writerXML.WriteEndElement();
                                    sActualColumnLevel_4 = sColumnNameSplit[2];
                                    writerXML.WriteStartElement("cfdi:" + sActualColumnLevel_4);
                                    if (sColumnNameSplit[3] != "null")
                                        writerXML.WriteAttributeString(sColumnNameSplit[3], sCellValue);
                                }
                                else if (sActualColumnLevel_2 == sColumnNameSplit[0] && sActualColumnLevel_3 == sColumnNameSplit[1] && sActualColumnLevel_4 == sColumnNameSplit[2])
                                {
                                    if (sColumnNameSplit[3] != "null")
                                        writerXML.WriteAttributeString(sColumnNameSplit[3], sCellValue);
                                }
                                iLevelAnt = 4;
                                break;
                        }
                    }
                    else if (sColumnNameSplit[0] == "null" && sColumnNameSplit[1] == "SQL")
                    {
                        DataSet datasetCFDI_XML_Nested = new DataSet();
                        DataRetrieve dataRetrieve = new DataRetrieve();
                        datasetCFDI_XML_Nested = dataRetrieve.SelectDataRetrieve(sCellValue, Util.ConnectionString);
                        writerXML = fAddLevesXML(datasetCFDI_XML_Nested, writerXML, sActualColumnLevel_2, sActualColumnLevel_3);
                        sActualColumnLevel_3 = "";
                    }
                }

            }
            return writerXML;
        }
    }
}
