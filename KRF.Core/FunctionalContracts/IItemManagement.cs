using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.Product;
using KRF.Core.Entities.ValueList;
using KRF.Core.DTO.Product;

namespace KRF.Core.FunctionalContracts
{
    public interface IItemManagement
    {
        /// <summary>
        /// Create an item
        /// </summary>
        /// <param name="item">Item details</param>
        /// <returns>Newly created item identifier</returns>
        int Create(Item item);

        /// <summary>
        /// Edit an item based on updated item details.
        /// </summary>
        /// <param name="item">Updated item details.</param>
        /// <returns>Updated item details.</returns>
        Item Edit(Item item);

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="itemId">Item unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool Delete(int itemId);

        /// <summary>
        /// Get all items created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active items else return all</param>
        /// <returns>List of items.</returns>
        IList<Item> GetAllItems(bool isActive = true);

        /// <summary>
        /// Get item details based on item id.
        /// </summary>
        /// <param name="itemId">Item's unique identifier</param>
        /// <returns>Item details.</returns>
        Item GetItem(int itemId);

        /// <summary>
        /// Search and filter items based on search text.
        /// </summary>
        /// <param name="searchText">Saerch text which need to be mapped with any of items related fields.</param>
        /// <returns>Item list.</returns>
        IList<Item> SearchItem(string searchText);

        /// <summary>
        /// Set Item to active / Deactive
        /// </summary>
        /// <param name="itemId">Item unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool SetActive(int itemId, bool isActive);

        /// <summary>
        /// Get Categories
        /// </summary>
        /// <returns>Category list</returns>
        IList<Category> GetCategories();

        /// <summary>
        /// Get Manufacturers
        /// </summary>
        /// <returns>Manufacturer list</returns>
        IList<Manufacturer> GetManufacturers();

        /// <summary>
        /// Get Unit of measures
        /// </summary>
        /// <returns>UnitOfMeasure list</returns>
        IList<UnitOfMeasure> GetUnitOfMeasures();

        /// <summary>
        /// Get Product Objects
        /// </summary>
        /// <returns></returns>
        ProductDTO GetProduct();

        /// <summary>
        /// Get Inventory detail
        /// </summary>
        /// <returns></returns>
        ProductDTO GetInventory();
        /// <summary>
        /// Get Inventory history detail
        /// </summary>
        /// <returns></returns>
        ProductDTO GetInventoryAudit(int inventoryID);
        /// <summary>
        /// Update Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        bool UpdateInventory(List<Inventory> inventories);
        /// <summary>
        /// Delete Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        bool DeleteInventory(List<Inventory> inventories);
    }
}
