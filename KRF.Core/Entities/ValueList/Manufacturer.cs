using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class Manufacturer : ValueList
    {
        /// <summary>
        /// Holds the Manufacturer ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the Manufacturer Name information.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
