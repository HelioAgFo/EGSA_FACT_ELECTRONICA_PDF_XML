using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FACT_ELECTRONICA_PDF_XML.Entity
{
    public class ServiceError
    {
        /// <summary>
        /// Federal taxpayers registration --> our user id
        /// </summary>
        public String RFC { get; set; }

        /// <summary>
        /// This number is intend to be used by tecnical support only.
        /// </summary>
        public Int32 Number { get; set; }

        /// <summary>
        /// Error message that may be displayed to the user or be replaced.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// TimeSpan to be used by tecnical support only.
        /// </summary>
        public String EventID { get; set; }
    }
}
