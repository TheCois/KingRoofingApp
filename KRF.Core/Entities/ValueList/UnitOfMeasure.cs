using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class UnitOfMeasure : ValueList
    {
        /// <summary>
        /// Holds the UnitOfMeasure ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the UnitOfMeasure information.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
