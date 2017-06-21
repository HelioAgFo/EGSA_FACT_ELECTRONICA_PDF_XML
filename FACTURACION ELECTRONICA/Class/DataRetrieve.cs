using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using FACT_ELECTRONICA_PDF_XML.Utils;

namespace FACT_ELECTRONICA_PDF_XML
{
    class DataRetrieve
    {
        public DataSet SelectDataRetrieve(string sNivelXML, string sConnectionString)
        {
            string sSQL = sNivelXML;
            SqlConnection sqlconnectionCFDI_XML = new SqlConnection(sConnectionString);
            SqlCommand sqlcommandCFDI_XML = new SqlCommand(sSQL, sqlconnectionCFDI_XML);
            sqlconnectionCFDI_XML.Open();
            SqlDataAdapter sqladapterCFDI_XML = new SqlDataAdapter(sqlcommandCFDI_XML);
            DataSet datasetCFDI_XML = new DataSet();
            sqladapterCFDI_XML.Fill(datasetCFDI_XML);
            sqlconnectionCFDI_XML.Close();

            return datasetCFDI_XML;
        }
    }
}
