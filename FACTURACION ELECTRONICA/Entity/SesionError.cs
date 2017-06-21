using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FACT_ELECTRONICA_PDF_XML.Entity
{
    public class SessionError
    {
        /// <summary>
        /// Federal taxpayers registration --> our user id
        /// </summary>
        public String RFC { get; set; }

        /// <summary>
        /// This number is intend to be used by tecnical support only.
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// Brief description of the client situation.
        /// </summary>
        public String Description { get; set; }
    }
}
