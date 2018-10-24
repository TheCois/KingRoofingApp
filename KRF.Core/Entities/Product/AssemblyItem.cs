using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.Product
{
    public class AssemblyItem
    {

        /// <summary>
        /// Holds item id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Holds item id
        /// </summary>
        public int ItemId { get; set; }
        
        /// <summary>
        /// Holds assembly id
        /// </summary>
        public int AssemblyId { get; set; }
        
        /// <summary>
        /// Item quantity to be included in assembly.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal PercentageOfItem { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal TaxPercent { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal CostPercent { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal RetailCost { get; set; }
    }
}
