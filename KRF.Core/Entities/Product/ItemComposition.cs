using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.Product
{
    public class ItemComposition
    {
        /// <summary>
        /// Holds item detail
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Item quantity to be included in assembly.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal PercentageOfItem { get; set; }
    }
}
