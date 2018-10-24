using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class Division : ValueList
    {
        /// <summary>
        /// Holds the Division ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the Division Name information.
        /// </summary>
        public string Name { get; set; }
    }
}
