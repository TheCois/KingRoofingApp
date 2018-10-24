using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class RoofAge
    {
        /// <summary>
        /// Holds the RoofAge ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the RoofAge text.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
