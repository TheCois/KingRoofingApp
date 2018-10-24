using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.Sales
{
    public class Prospect
    {
        /// <summary>
        /// Prospect Identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Telephone
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public int City { get; set; }

        /// <summary>
        /// Holds the Job address information
        /// </summary>
        public int Country { get; set; }

        /// <summary>
        /// Zip
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public int Status { get; set; }
    }
}
