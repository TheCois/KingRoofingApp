using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class OpportunityStatus : ValueList
    {
        /// <summary>
        /// Holds the OpportunityStatus ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the OpportunityStatus Name information.
        /// </summary>
        public string Status { get; set; }
    }
}
