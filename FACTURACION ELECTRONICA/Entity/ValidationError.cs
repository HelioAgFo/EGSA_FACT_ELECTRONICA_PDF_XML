using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FACT_ELECTRONICA_PDF_XML.Entity
{
    public class ValidationError
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
        /// Brief description of the problem.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Xml node name.
        /// </summary>
        public String Node { get; set; }

        /// <summary>
        /// Brief description of the legal reason, if exists 
        /// </summary>
        public String Antecedent { get; set; }

        /// <summary>
        /// Suggestion for a solution, if there is.
        /// </summary>
        public String Suggestion { get; set; }

        /// <summary>
        /// TimeSpan to be used by tecnical support only.
        /// </summary>
        public String EventID { get; set; }
    }
}
