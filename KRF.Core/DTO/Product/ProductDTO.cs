using System.Collections.Generic;
using KRF.Core.Entities.ValueList;
using KRF.Core.Entities.Product;
namespace KRF.Core.DTO.Product
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class ProductDTO
    {
        /// <summary>
        /// Holds the Item List
        /// </summary>
        public IList<Item> Items { get; set; }

        /// <summary>
        /// Holds Category List
        /// </summary>
        public IList<Category> Categories { get; set; }

        /// <summary>
        /// Holds Item Types List
        /// </summary>
        public IList<ItemType> ItemTypes { get; set; }

        /// <summary>
        /// Holds Manufacturer List
        /// </summary>
        public IList<Manufacturer> Manufacturers { get; set; }

        /// <summary>
        /// Holds Unit of measure list
        /// </summary>
        public IList<UnitOfMeasure> UnitsOfMeasure { get; set; }
        
        /// <summary>
        /// Holds Unit of assembly list
        /// </summary>
        public IList<Assembly> Assemblies { get; set; }
        /// <summary>
        /// Holds Unit of inventory list
        /// </summary>
        public IList<Inventory> Inventories { get; set; }
        /// <summary>
        /// Holds Unit of inventory audit list
        /// </summary>
        public IList<InventoryAudit> InventoryAudits { get; set; }
    }
}
