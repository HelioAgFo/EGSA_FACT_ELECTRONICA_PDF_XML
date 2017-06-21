using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FACT_ELECTRONICA_PDF_XML.View
{
    interface IInterface
    {
        void ShowSessionError(Entity.SessionError error);
        void ShowUnknownError(String message);

        System.Guid UUID { get; set; }
        string RFCReceptor { get; set; }
        string InvcNbr { get; set; }
        string InvcDate { get; set; }
        string Estatus { set; }
    }
}
