using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.MISC;

namespace KRF.Core.Entities.Sales
{
    public class CustomerAddress
    {
        /// <summary>
        /// Holds ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds Address1
        /// </summary>
        public string Address1 { get; set; }
        /// <summary>
        /// Holds Address2
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// Holds City
        /// </summary>
        public int City { get; set; }
        /// <summary>
        /// Holds State
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// Holds ZipCode
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Holds CustomerID
        /// </summary>
        public int LeadID { get; set; }
    }
}
