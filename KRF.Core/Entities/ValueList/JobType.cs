using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class JobType : ValueList
    {
        /// <summary>
        /// Holds the JobType ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the JobType Name information.
        /// </summary>
        public string Name { get; set; }
    }
}
