using System;
using System.Collections.Generic;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using KRF.Core.Entities.Product;
using KRF.Core.Entities.ValueList;
using KRF.Core.DTO.Product;

namespace KRF.Persistence.RepositoryImplementation
{
    public class ItemManagementRepository : IItemManagementRepository
    {
        private readonly IItemManagement itemManagement_;
        /// <summary>
        /// Constructor
        /// </summary>
        public ItemManagementRepository()
        {
            itemManagement_ = ObjectFactory.GetInstance<IItemManagement>();
        }

        public int Create(Item item)
        {
            return itemManagement_.Create(item);
        }

        public Item Edit(Item item)
        {
            return itemManagement_.Edit(item);
        }

        public bool Delete(int itemId)
        {
            return itemManagement_.Delete(itemId);
        }

        public IList<Item> GetAllItems(bool isActive = true)
        {
            return itemManagement_.GetAllItems(isActive);
        }

        public Item GetItem(int itemId)
        {
            return itemManagement_.GetItem(itemId);
        }

        public IList<Item> SearchItem(string searchText)
        {
            throw new NotImplementedException();
        }

        public bool SetActive(int itemId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public IList<Category> GetCategories()
        {
            return itemManagement_.GetCategories();
        }

        public IList<Manufacturer> GetManufacturers()
        {
            return itemManagement_.GetManufacturers();
        }

        public IList<UnitOfMeasure> GetUnitOfMeasures()
        {
            return itemManagement_.GetUnitOfMeasures();
        }


        public ProductDTO GetProduct()
        {
            return itemManagement_.GetProduct();
        }
        public ProductDTO GetInventory()
        {
            return itemManagement_.GetInventory();
        }
        public ProductDTO GetInventoryAudit(int inventoryId)
        {
            return itemManagement_.GetInventoryAudit(inventoryId);
        }
        /// <summary>
        /// Update Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        public bool UpdateInventory(List<Inventory> inventories)
        {
            return itemManagement_.UpdateInventory(inventories);
        }
        /// <summary>
        /// Delete Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        public bool DeleteInventory(List<Inventory> inventories)
        {
            return itemManagement_.DeleteInventory(inventories);
        }
    }
}
