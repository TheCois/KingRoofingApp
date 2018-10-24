using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.ValueList;

namespace KRF.Core.Entities.Product
{
    public class Item
    {
        /// <summary>
        /// Item Identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Item type ID
        /// </summary>
        public int ItemTypeId { get; set; }

        /// <summary>
        /// Item's code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Items name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description about the Item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Product grouping Category 
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Item manufacturer details
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// Unit of Measure of the item.
        /// </summary>
        public int UnitOfMeasureId { get; set; }

        /// <summary>
        /// Item's price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Labor cost for the item.
        /// </summary>
        public decimal LaborCost { get; set; }

        /// <summary>
        /// Mark-up percentage. 
        /// TODO - Need clarity on what we mean by this and how to calculate per-division.
        /// </summary>
        //public decimal MarkUpPercentage { get; set; }

        /// <summary>
        /// Purchase Order Text.
        /// TODO - Need to understand what this field is all about.
        /// </summary>
        public string PurchaseOrderText { get; set; }

        /// <summary>
        /// Determines whether the item cost is taxable or not.
        /// </summary>
        public bool IsTaxable { get; set; }

        /// <summary>
        /// Active status.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Active status.
        /// </summary>
        public bool IsInventoryItem { get; set; }
    }
}
