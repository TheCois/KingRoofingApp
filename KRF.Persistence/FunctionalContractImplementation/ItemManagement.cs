using KRF.Core.Entities.Product;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions;
using KRF.Core.Entities.ValueList;
using KRF.Core.DTO.Product;
using System.Data;
using System.Transactions;
using NLog;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class ItemManagement : IItemManagement
    {
        private Logger logger_;

        public ItemManagement()
        {
            logger_ = LogManager.GetCurrentClassLogger();
        }
        /// <summary>
        /// Create an item
        /// </summary>
        /// <param name="item">Item details</param>
        /// <returns>Newly created item identifier</returns>
        public int Create(Item item)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var id = conn.Insert(item);
                UpdateInventoryRecord(item, conn);
                return id;
            }
        }

        private void UpdateInventoryRecord(Item item, IDbConnection conn)
        {
            var inventoryList = conn.GetList<Inventory>().Where(x => x.ItemID == item.Id).ToList();
            if (item.IsInventoryItem)
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
                    conn.Insert(inventory);
                }
                else
                {
                    inventory.DateUpdated = DateTime.Now;
                    conn.Update(inventory);
                }
            }
            else if (inventoryList.Count == 1)
            {
                var inventory = inventoryList.FirstOrDefault();
                conn.Delete(inventory);
            }
        }

        /// <summary>
        /// Edit an item based on updated item details.
        /// </summary>
        /// <param name="item">Updated item details.</param>
        /// <returns>Updated item details.</returns>
        public Item Edit(Item item)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                conn.Update(item);

                var pgAssemblyItem = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.ItemId, Operator.Eq, item.Id));

                var assemblyItems = conn.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                UpdateAssemblyItems(item.Price, assemblyItems, conn);

                UpdateInventoryRecord(item, conn);

                return item;
            }
        }

        private void UpdateAssemblyItems(decimal itemPrice, IEnumerable<AssemblyItem> assemblyItems, IDbConnection conn)
        {
            foreach (var assemblyItem in assemblyItems)
            {
                assemblyItem.Price = itemPrice;
                var value = (assemblyItem.Price * (assemblyItem.PercentageOfItem / 100));
                assemblyItem.Value = value;
                var cost = value + ((value * assemblyItem.TaxPercent) / 100);
                assemblyItem.Cost = cost;
                var retailCost = (cost / (1 - assemblyItem.CostPercent / 100));
                assemblyItem.RetailCost = retailCost;

                IAssemblyManagement assemblyMgt = new AssemblyManagement();

                var assemblyDto = assemblyMgt.GetAssembly(assemblyItem.AssemblyId, conn);

                foreach (var ai in assemblyDto.assemblyItem.Where(p => p.id == assemblyItem.id))
                {
                    ai.Price = assemblyItem.Price;
                    ai.Value = assemblyItem.Value;
                    ai.Cost = assemblyItem.Cost;
                    ai.RetailCost = assemblyItem.RetailCost;
                }

                assemblyDto.assembly.TotalCost = assemblyDto.assemblyItem.Sum(p => p.Cost);
                assemblyDto.assembly.TotalRetailCost = assemblyDto.assemblyItem.Sum(p => p.RetailCost);
                assemblyMgt.Edit(assemblyDto);
            }
        }

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="itemId">Item unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int itemId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Item>(s => s.Id, Operator.Eq, itemId));

                conn.Open();
                var isDeleted = conn.Delete<Item>(predicateGroup);
                return isDeleted;
            }
        }

        /// <summary>
        /// Get all items created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active items else return all</param>
        public IList<Item> GetAllItems(bool isActive = true)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Item> items = conn.GetList<Item>().ToList();
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Item>(s => s.Id, Operator.Eq, itemId));
                conn.Open();
                var item = conn.Get<Item>(itemId);
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Category> categories = conn.GetList<Category>().ToList();
                return categories;
            }
        }

        public IList<Manufacturer> GetManufacturers()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Manufacturer> manufacturers = conn.GetList<Manufacturer>().ToList();
                return manufacturers;
            }
        }

        public IList<UnitOfMeasure> GetUnitOfMeasures()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<UnitOfMeasure> unitOfMeasures = conn.GetList<UnitOfMeasure>().ToList();
                return unitOfMeasures;
            }
        }

        /// <summary>
        /// Get Product entities
        /// </summary>
        /// <returns> Items and its dependent value List objects and Assembly list</returns>
        public ProductDTO GetProduct()
        {
            logger_.Info("Entering ItemManagement.GetProduct()");
            var product = new ProductDTO();
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<ItemType> itemTypes = conn.GetList<ItemType>().ToList();
                IList<Category> categories = conn.GetList<Category>().ToList();
                IList<Manufacturer> manufacturers = conn.GetList<Manufacturer>().ToList();
                IList<UnitOfMeasure> unitOfMeasures = conn.GetList<UnitOfMeasure>().ToList();
                IList<Item> items = conn.GetList<Item>().ToList();
                IList<Assembly> assemblies = conn.GetList<Assembly>().ToList();

                product.ItemTypes = itemTypes.Where(p => p.Active).ToList();
                product.Categories = categories.Where(p => p.Active).ToList();
                product.Manufacturers = manufacturers.Where(p => p.Active).ToList();
                product.UnitsOfMeasure = unitOfMeasures.Where(p => p.Active).ToList();
                product.Items = items.OrderBy(p => p.Code).ToList();
                product.Assemblies = assemblies;
                logger_.Info("Leaving ItemManagement.GetProduct()");
                return product;
            }
        }

        /// <summary>
        /// Get Inventory detail
        /// </summary>
        /// <returns></returns>
        public ProductDTO GetInventory()
        {
            var product = new ProductDTO();
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Item> items = conn.GetList<Item>().Where(x => x.IsInventoryItem).ToList();
                IList<Inventory> inventories = conn.GetList<Inventory>().ToList();
                IList<ItemType> itemTypes = conn.GetList<ItemType>().ToList();
                IList<Category> categories = conn.GetList<Category>().ToList();
                IList<Manufacturer> manufacturers = conn.GetList<Manufacturer>().ToList();
                IList<UnitOfMeasure> unitOfMeasures = conn.GetList<UnitOfMeasure>().ToList();

                product.Items = items;
                product.Inventories = inventories;
                product.ItemTypes = itemTypes.Where(p => p.Active).ToList();
                product.Categories = categories.Where(p => p.Active).ToList();
                product.Manufacturers = manufacturers.Where(p => p.Active).ToList();
                product.UnitsOfMeasure = unitOfMeasures.Where(p => p.Active).ToList();
                return product;
            }
        }

        /// <summary>
        /// Get Inventory audit detail
        /// </summary>
        /// <returns></returns>
        public ProductDTO GetInventoryAudit(int inventoryId)
        {
            var product = new ProductDTO();
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<InventoryAudit>(s => s.ID, Operator.Eq, inventoryId));
                product.InventoryAudits = conn.GetList<InventoryAudit>(predicateGroup).ToList();
                IList<Item> items = conn.GetList<Item>().ToList();
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
                    var dbConnection = new DataAccessFactory();
                    using (var conn = dbConnection.CreateConnection())
                    {
                        conn.Open();
                        foreach (var inventory in inventories)
                        {
                            var curInventory = conn.Get<Inventory>(inventory.ID);
                            if (curInventory.Qty == inventory.Qty) continue;
                            var auditItem = new InventoryAudit
                            {
                                AssemblyID = curInventory.AssemblyID,
                                Comment = curInventory.Comment,
                                DateCreated = curInventory.DateUpdated,
                                DateUpdated = DateTime.Now,
                                ItemID = curInventory.ItemID,
                                Qty = Math.Abs(curInventory.Qty - inventory.Qty),
                                Type = curInventory.Qty > inventory.Qty ? "Deleted" : "Added"
                            };

                            curInventory.Comment = "";
                            curInventory.Qty = inventory.Qty;
                            conn.Update(curInventory);
                            conn.Insert(auditItem);
                        }
                    }

                    transactionScope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
            var result = false;
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    var dbConnection = new DataAccessFactory();
                    using (var conn = dbConnection.CreateConnection())
                    {
                        conn.Open();
                        foreach (var inventory in inventories)
                        {
                            var invToBeDeleted = conn.Get<Inventory>(inventory.ID);
                            result = conn.Delete(invToBeDeleted);
                        }
                    }

                    transactionScope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
