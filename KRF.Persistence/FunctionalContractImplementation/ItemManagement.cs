using KRF.Common;
using KRF.Core.Entities.Product;
using KRF.Core.FunctionalContracts;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using KRF.Core.Entities.ValueList;
using KRF.Core.DTO.Product;
using System.Configuration;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class ItemManagement : IItemManagement
    {
        private string _connectionString;

        public ItemManagement()
        {
            //_connectionString = ObjectFactory.GetInstance<IDatabaseConnection>().ConnectionString;
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        /// <summary>
        /// Create an item
        /// </summary>
        /// <param name="item">Item details</param>
        /// <returns>Newly created item identifier</returns>
        public int Create(Item item)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var id = sqlConnection.Insert<Item>(item);
                UpdateInventoryRecord(item, sqlConnection);
                return id;
            }
        }

        private void UpdateInventoryRecord(Item item, SqlConnection sqlConnection)
        {
            List<Inventory> inventoryList = sqlConnection.GetList<Inventory>().Where(x => x.ItemID == item.Id).ToList();
            if (item.IsInventoryItem == true)
            {
                var inventory = inventoryList.FirstOrDefault();
                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        ItemID = item.Id,
                        DateUpdated = DateTime.Now,
                        Type = "Added"
                    };
                    sqlConnection.Insert<Inventory>(inventory);
                }
                else
                {
                    inventory.DateUpdated = DateTime.Now;
                    sqlConnection.Update<Inventory>(inventory);
                }
            }
            else if (inventoryList.Count == 1)
            {
                var inventory = inventoryList.FirstOrDefault();
                sqlConnection.Delete<Inventory>(inventory);
            }
        }

        /// <summary>
        /// Edit an item based on updated item details.
        /// </summary>
        /// <param name="item">Updated item details.</param>
        /// <returns>Updated item details.</returns>
        public Item Edit(Item item)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var isEdited = sqlConnection.Update<Item>(item);

                var pgAssemblyItem = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.ItemId, Operator.Eq, item.Id));

                List<AssemblyItem> assemblyItems = sqlConnection.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                UpdateAssemblyItems(item.Price, assemblyItems, sqlConnection);

                UpdateInventoryRecord(item, sqlConnection);

                return item;
            }
        }

        private void UpdateAssemblyItems(decimal itemPrice, List<AssemblyItem> assemblyItems, SqlConnection sqlConnection)
        {
            foreach(AssemblyItem assemblyItem in assemblyItems)
            {
                decimal value = 0;
                decimal cost = 0;
                decimal retailCost = 0;
                assemblyItem.Price = itemPrice;
                value = (assemblyItem.Price * (assemblyItem.PercentageOfItem / 100));
                assemblyItem.Value = value;
                cost = value + ((value * assemblyItem.TaxPercent) / 100);
                assemblyItem.Cost = cost;
                retailCost = (cost / (1 - assemblyItem.CostPercent / 100));
                assemblyItem.RetailCost = retailCost;

                IAssemblyManagement assemblyMgt = new AssemblyManagement();

                AssemblyItemDTO assemblyDTO = assemblyMgt.GetAssembly(assemblyItem.AssemblyId, sqlConnection);
                
                foreach (AssemblyItem ai in assemblyDTO.assemblyItem.Where(p=>p.id == assemblyItem.id))
                {
                    ai.Price = assemblyItem.Price;
                    ai.Value = assemblyItem.Value;
                    ai.Cost = assemblyItem.Cost;
                    ai.RetailCost = assemblyItem.RetailCost;
                }
                assemblyDTO.assembly.TotalCost = assemblyDTO.assemblyItem.Sum(p => p.Cost);
                assemblyDTO.assembly.TotalRetailCost = assemblyDTO.assemblyItem.Sum(p => p.RetailCost);
                assemblyMgt.Edit(assemblyDTO);
            }
        }

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="itemId">Item unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int itemId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Item>(s => s.Id, Operator.Eq, itemId));

                sqlConnection.Open();
                var isDeleted = sqlConnection.Delete<Item>(predicateGroup);
                return isDeleted;
            }
        }

        /// <summary>
        /// Get all items created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active items else return all</param>
        public IList<Item> GetAllItems(bool isActive = true)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Item> items = sqlConnection.GetList<Item>().ToList();
                return items;
            }
        }

        /// <summary>
        /// Get item details based on item id.
        /// </summary>
        /// <param name="itemId">Item's unique identifier</param>
        /// <returns>Item details.</returns>
        public Item GetItem(int itemId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Item>(s => s.Id, Operator.Eq, itemId));
                sqlConnection.Open();
                Item item = sqlConnection.Get<Item>(itemId);
                return item;
            }
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
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Category> categories = sqlConnection.GetList<Category>().ToList();
                return categories;
            }
        }

        public IList<Manufacturer> GetManufacturers()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Manufacturer> manufacturers = sqlConnection.GetList<Manufacturer>().ToList();
                return manufacturers;
            }
        }

        public IList<UnitOfMeasure> GetUnitOfMeasures()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<UnitOfMeasure> unitOfMeasures = sqlConnection.GetList<UnitOfMeasure>().ToList();
                return unitOfMeasures;
            }
        }

        /// <summary>
        /// Get Product entities
        /// </summary>
        /// <returns> Items and its dependent Vlaue List objects and Assembly list</returns>
        public ProductDTO GetProduct()
        {
            ProductDTO product = new ProductDTO();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<ItemType> itemTypes = sqlConnection.GetList<ItemType>().ToList();
                IList<Category> categories = sqlConnection.GetList<Category>().ToList();
                IList<Manufacturer> manufacturers = sqlConnection.GetList<Manufacturer>().ToList();
                IList<UnitOfMeasure> unitOfMeasures = sqlConnection.GetList<UnitOfMeasure>().ToList();
                IList<Item> items = sqlConnection.GetList<Item>().ToList();
                IList<Assembly> assemblies = sqlConnection.GetList<Assembly>().ToList();

                product.ItemTypes = itemTypes.Where(p => p.Active == true).ToList();
                product.Categories = categories.Where(p=>p.Active == true).ToList();
                product.Manufacturers = manufacturers.Where(p => p.Active == true).ToList();
                product.UnitsOfMeasure = unitOfMeasures.Where(p => p.Active == true).ToList();
                product.Items = items.OrderBy(p=>p.Code).ToList();
                product.Assemblies = assemblies;
                return product;
            }
        }

        /// <summary>
        /// Get Inventory detail
        /// </summary>
        /// <returns></returns>
        public ProductDTO GetInventory()
        {
            ProductDTO product = new ProductDTO();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                IList<Item> items = sqlConnection.GetList<Item>().Where(x => x.IsInventoryItem).ToList();
                IList<Inventory> inventories = sqlConnection.GetList<Inventory>().ToList();
                IList<ItemType> itemTypes = sqlConnection.GetList<ItemType>().ToList();
                IList<Category> categories = sqlConnection.GetList<Category>().ToList();
                IList<Manufacturer> manufacturers = sqlConnection.GetList<Manufacturer>().ToList();
                IList<UnitOfMeasure> unitOfMeasures = sqlConnection.GetList<UnitOfMeasure>().ToList();

                product.Items = items;
                product.Inventories = inventories;
                product.ItemTypes = itemTypes.Where(p => p.Active == true).ToList();
                product.Categories = categories.Where(p => p.Active == true).ToList();
                product.Manufacturers = manufacturers.Where(p => p.Active == true).ToList();
                product.UnitsOfMeasure = unitOfMeasures.Where(p => p.Active == true).ToList();
                return product;
            }
        }
        /// <summary>
        /// Get Inventory audit detail
        /// </summary>
        /// <returns></returns>
        public ProductDTO GetInventoryAudit(int inventoryID)
        {
            ProductDTO product = new ProductDTO();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<InventoryAudit>(s => s.ID, Operator.Eq, inventoryID));
                product.InventoryAudits = sqlConnection.GetList<InventoryAudit>(predicateGroup).ToList();
                IList<Item> items = sqlConnection.GetList<Item>().ToList();
                product.Items = items;

                return product;
            }
        }
        /// <summary>
        /// Update Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        public bool UpdateInventory(List<Inventory> inventories)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    using (var sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        foreach (Inventory inventory in inventories)
                        {
                            Inventory curInventory = sqlConnection.Get<Inventory>(inventory.ID);
                            if (curInventory.Qty != inventory.Qty)
                            {
                                if(curInventory.Qty > inventory.Qty)
                                {
                                    curInventory.Type = "Deleted";
                                }
                                else
                                {
                                    curInventory.Type = "Added";
                                }
                                curInventory.Comment = "";
                                curInventory.Qty = inventory.Qty;
                                sqlConnection.Update<Inventory>(curInventory);
                            }
                        }
                    }
                    transactionScope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Delete Inventory
        /// </summary>
        /// <param name="inventories"></param>
        /// <returns></returns>
        public bool DeleteInventory(List<Inventory> inventories)
        {
            bool result = false;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    using (var sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        foreach (Inventory inventory in inventories)
                        {
                            Inventory invToBeDeleted = sqlConnection.Get<Inventory>(inventory.ID);
                            result = sqlConnection.Delete<Inventory>(invToBeDeleted);
                        }
                    }
                    transactionScope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
