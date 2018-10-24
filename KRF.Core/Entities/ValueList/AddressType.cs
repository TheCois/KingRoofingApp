using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class AddressType : ValueList
    {
        /// <summary>
        /// Holds the AddressType ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the AddressType Name information.
        /// </summary>
        public string Name { get; set; }
    }
}
