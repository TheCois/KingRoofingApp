using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class OpportunitySource : ValueList
    {
        /// <summary>
        /// Holds the OpportunitySource ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the OpportunitySource Name information.
        /// </summary>
        public string Name { get; set; }
    }
}
