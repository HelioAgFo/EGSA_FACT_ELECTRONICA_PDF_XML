using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using FACT_ELECTRONICA_PDF_XML.View;

namespace FACT_ELECTRONICA_PDF_XML.Utils
{
    class Utilerias
    {
        /// <summary>
        /// This id can come from a database or be hard coded 
        /// by each service integrator or service reseller.
        /// </summary>
        public String INTEGRATOR_ID;
        public String AltaIntegrador_ID;

        public string RFC;
        public string ConnectionString;
        public string XMLPath;
        public string PDFPath;
        public string QRCodePath;


        private List<ConfigList> confList = new List<ConfigList>();
        private List<Paths> pathsList = new List<Paths>();
        //public string RFC = "PNU1111251F3";
        //public string RFC = "SUL010720JN8";
        
        private IInterface _view;
        public Utilerias()
        {
            _view = null;
            confList = ReturnConfigValues("DEFAULT");
            pathsList = ReturnPathValues();
            foreach (ConfigList cl in confList)
            {
                RFC = cl.RFC;
                ConnectionString = cl.ConnectionString;
                AltaIntegrador_ID = cl.AltaIntegrador_ID;
                INTEGRATOR_ID = cl.INTEGRATOR_ID;
            }
            foreach (Paths pa in pathsList)
            {
                XMLPath = pa.XMLPath;
                PDFPath = pa.PDFPath;
                QRCodePath = pa.QRCodePath;
            }
        }
        public Utilerias(IInterface Inter)
        {
            _view = Inter;
        }
        public string CreateToken(string RFC, Int64 transactionID, ref string token)
        {
            string result = string.Empty;
            try
            {
                #region Build Token
                using (var securitySrv = new SrvSecurity.SeguridadClient())
                {
                    var serviceToken = securitySrv.ObtenerToken(RFC, ref transactionID);
                    var toHash = String.Format("{0}|{1}", INTEGRATOR_ID, serviceToken);
                    token = Security.Hash(toHash);
                }
                #endregion
            }
            #region Security service exceptions
            catch (FaultException<SrvSecurity.FallaServicio> serviceFault)
            {
                using (var errorForm = new Forms.Error(TranslateFault.ToServiceError(serviceFault.Detail)))
                {
                    errorForm.ShowDialog();
                }
            }
            catch (FaultException<SrvSecurity.FallaSesion> sessionFauld)
            {
                _view.ShowSessionError(TranslateFault.ToSessionError(sessionFauld.Detail));
                result = TranslateFault.ToSessionError(sessionFauld.Detail).Description;
            }
            #endregion
            catch (Exception ex)
            {
                _view.ShowUnknownError(ex.Message);
                result = ex.Message;
            }
            return result;
        }

        public string CreateTokenAlta(string RFC, Int64 transactionID, ref string token)
        {
            string result = string.Empty;
            try
            {
                #region Build Token
                using (var securitySrv = new SrvSecurity.SeguridadClient())
                {
                    var serviceToken = securitySrv.ObtenerToken(RFC, ref transactionID);
                    var toHash = String.Format("{0}|{1}|{2}", INTEGRATOR_ID, AltaIntegrador_ID, serviceToken);
                    token = Security.Hash(toHash);
                }
                #endregion
            }
            #region Security service exceptions
            catch (FaultException<SrvSecurity.FallaServicio> serviceFault)
            {
                using (var errorForm = new Forms.Error(TranslateFault.ToServiceError(serviceFault.Detail)))
                {
                    errorForm.ShowDialog();
                }
            }
            catch (FaultException<SrvSecurity.FallaSesion> sessionFauld)
            {
                _view.ShowSessionError(TranslateFault.ToSessionError(sessionFauld.Detail));
                result = TranslateFault.ToSessionError(sessionFauld.Detail).Description;
            }
            #endregion
            catch (Exception ex)
            {
                _view.ShowUnknownError(ex.Message);
                result = ex.Message;
            }
            return result;
        }
        public List<ConfigList> ReturnConfigValues(string sData)
        {
            DataSet dataSetConfig = new DataSet();
            System.IO.FileStream streamRead = new System.IO.FileStream("config.xml", System.IO.FileMode.Open);
            dataSetConfig.ReadXml(streamRead);
            streamRead.Close();

            foreach (DataRow dr in dataSetConfig.Tables["CompanyValue"].Rows)
                if (dr["key"].ToString() == sData)
                    confList.Add(new ConfigList
                    {
                        ConnectionString = dr["ConnectionString"].ToString(),
                        RFC = dr["RFC"].ToString(),
                        INTEGRATOR_ID = dr["INTEGRATOR_ID"].ToString(),
                        AltaIntegrador_ID = dr["AltaIntegrador_ID"].ToString()
                    });
            return confList;
        }
        public List<Paths> ReturnPathValues()
        {
            DataSet dataSetConfig = new DataSet();
            System.IO.FileStream streamRead = new System.IO.FileStream("config.xml", System.IO.FileMode.Open);
            dataSetConfig.ReadXml(streamRead);
            streamRead.Close();
            pathsList.Add(new Paths
            {
                XMLPath = dataSetConfig.Tables["Paths"].Rows[0]["XMLPath"].ToString(),
                PDFPath = dataSetConfig.Tables["Paths"].Rows[0]["PDFPath"].ToString(),
                QRCodePath = dataSetConfig.Tables["Paths"].Rows[0]["QRCodePath"].ToString()
            });
            return pathsList;
        }
        public class ConfigList
        {
            public string ConnectionString { set; get; }
            public string RFC { set; get; }
            public string INTEGRATOR_ID { set; get; }
            public string AltaIntegrador_ID { set; get; }
        }
        public class Paths
        {
            public string XMLPath { set; get; }
            public string PDFPath { set; get; }
            public string QRCodePath { set; get; }
        }
    }
}
