using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class JobStatus : ValueList
    {
        /// <summary>
        /// Holds the JobStatus ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the JobStatus Name information.
        /// </summary>
        public string Status { get; set; }
    }
}
