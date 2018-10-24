using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class CustomerType : ValueList
    {
        /// <summary>
        /// Holds the CustomerType ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the CustomerType Name information.
        /// </summary>
        public string Name { get; set; }
    }
}
