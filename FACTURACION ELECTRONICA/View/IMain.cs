using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FACT_ELECTRONICA_PDF_XML.View
{
    interface IMain : IInterface
    {
        string ResultStr { set; }
        Bitmap BarcodeQR { set; }
        string ShipperIdStr { get; set; }
        event EventHandler void_GeneraXML;
        event EventHandler void_CancelaXML;
    }
}
