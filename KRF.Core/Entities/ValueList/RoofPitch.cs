using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class RoofPitch : ValueList
    {
        /// <summary>
        /// Holds the RoofPitch ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the RoofPitch value information.
        /// </summary>
        public decimal Value { get; set; }
    }
}
