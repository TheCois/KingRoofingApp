using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class ItemType : ValueList
    {
        /// <summary>
        /// Holds the Item type ID information.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds the Item Type description information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
