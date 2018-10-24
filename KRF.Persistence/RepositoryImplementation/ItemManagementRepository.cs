using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using KRF.Core.Entities.Product;
using StructureMap;
using KRF.Core.Entities.ValueList;
using KRF.Core.DTO.Product;

namespace KRF.Persistence.RepositoryImplementation
{
    public class ItemManagementRepository : IItemManagementRepository
    {
        private readonly IItemManagement _ItemManagement;
        /// <summary>
        /// Constructor
        /// </summary>
        public ItemManagementRepository()
        {
            _ItemManagement = ObjectFactory.GetInstance<IItemManagement>();
        }

        public int Create(Item item)
        {
            return _ItemManagement.Create(item);
        }

        public Item Edit(Item item)
        {
            return _ItemManagement.Edit(item);
        }

        public bool Delete(int itemId)
        {
            return _ItemManagement.Delete(itemId);
        }

        public IList<Item> GetAllItems(bool isActive = true)
        {
            return _ItemManagement.GetAllItems(isActive);
        }

        public Item GetItem(int itemId)
        {
            return _ItemManagement.GetItem(itemId);
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
            return _ItemManagement.GetCategories();
        }

        public IList<Manufacturer> GetManufacturers()
        {
            return _ItemManagement.GetManufacturers();
        }

        public IList<UnitOfMeasure> GetUnitOfMeasures()
        {
            return _ItemManagement.GetUnitOfMeasures();
        }


        public ProductDTO GetProduct()
        {
            return _ItemManagement.GetProduct();
        }
        public ProductDTO GetInventory()
        {
            return _ItemManagement.GetInventory();
        }
        public ProductDTO GetInventoryAudit(int inventoryID)
        {
            return _ItemManagement.GetInventoryAudit(inventoryID);
        }
        /// <summary>
        /// Update Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        public bool UpdateInventory(List<Inventory> inventories)
        {
            return _ItemManagement.UpdateInventory(inventories);
        }
        /// <summary>
        /// Delete Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        public bool DeleteInventory(List<Inventory> inventories)
        {
            return _ItemManagement.DeleteInventory(inventories);
        }
    }
}
