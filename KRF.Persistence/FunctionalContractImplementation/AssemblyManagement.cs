using KRF.Core.Entities.Product;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions;
using System.Data;
using KRF.Core.DTO.Product;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class AssemblyManagement : IAssemblyManagement
    {
        /// <summary>
        /// Create an Assembly
        /// </summary>
        /// <param name="assembly">Assembly details</param>
        /// <returns>Newly created Assembly identifier</returns>
        public int Create(AssemblyItemDTO assembly)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    try
                    {
                        decimal totalMaterialCost = 0;
                        decimal totalLabourCost = 0;
                        foreach (var i in assembly.assemblyItem)
                        {
                            var item = conn.Get<Item>(i.ItemId);
                            switch (item.ItemTypeId)
                            {
                                case (int) Core.Enums.ItemType.Labor:
                                    totalLabourCost += Convert.ToDecimal(i.RetailCost);
                                    break;
                                case (int) Core.Enums.ItemType.Material:
                                    totalMaterialCost += Convert.ToDecimal(i.RetailCost);
                                    break;
                            }
                        }

                        assembly.assembly.MaterialCost = totalMaterialCost;
                        assembly.assembly.LaborCost = totalLabourCost;

                        var id = conn.Insert(assembly.assembly);
                        var inventories = conn.GetList<Inventory>().ToList();
                        foreach (var i in assembly.assemblyItem)
                        {
                            i.AssemblyId = id;
                            conn.Insert(i);
                            var item = conn.Get<Item>(i.ItemId);
                            if (item.ItemTypeId == (int) Core.Enums.ItemType.Labor)
                            {
                                totalLabourCost += Convert.ToDecimal(i.RetailCost);
                            }
                            else if (item.ItemTypeId == (int) Core.Enums.ItemType.Material)
                            {
                                totalMaterialCost += Convert.ToDecimal(i.RetailCost);
                            }

                            if (item.ItemTypeId == (int) Core.Enums.ItemType.Material &&
                                (inventories.All(p => p.ItemID != i.ItemId)))
                            {

                                //Create record in Inventory table with default quantity 0
                                conn.Insert(new Inventory()
                                    {ItemID = i.ItemId, Qty = 0, DateUpdated = DateTime.Now});
                            }
                        }

                        transactionScope.Complete();
                        return id;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        transactionScope.Dispose();
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Edit an Assembly based on updated Assembly details.
        /// </summary>
        /// <param name="assembly">Updated Assembly details.</param>
        /// <returns>Updated Assembly details.</returns>
        public Assembly Edit(AssemblyItemDTO assembly)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    try
                    {
                        var pgAssemblyItem = new PredicateGroup
                            {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                        pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq,
                            assembly.assembly.Id));

                        var assemblyItemList = conn.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                        var inventories = conn.GetList<Inventory>().ToList();
                        foreach (var i in assemblyItemList)
                        {
                            //if (assembly.assemblyItem.Where(k => k.ItemId == i.ItemId).Count() == 0)
                            //{
                            //    conn.Delete<AssemblyItem>(i.id);
                            //}
                            conn.Delete(i);
                        }

                        decimal totalMaterialCost = 0;
                        decimal totalLabourCost = 0;
                        foreach (var i in assembly.assemblyItem)
                        {
                            var item = conn.Get<Item>(i.ItemId);
                            if (item.ItemTypeId == (int) Core.Enums.ItemType.Labor)
                            {
                                totalLabourCost += Convert.ToDecimal(i.RetailCost);
                            }
                            else if (item.ItemTypeId == (int) Core.Enums.ItemType.Material)
                            {
                                totalMaterialCost += Convert.ToDecimal(i.RetailCost);
                            }

                            //if (i.id > 0)
                            //    conn.Update<AssemblyItem>(i);
                            //else
                            conn.Insert(i);
                            if (item.ItemTypeId == (int) Core.Enums.ItemType.Material &&
                                (!inventories.Any(p => p.ItemID == i.ItemId)))
                            {
                                //Create record in Inventory table with default quantity 0
                                conn.Insert(new Inventory()
                                    {ItemID = i.ItemId, Qty = 0, DateUpdated = DateTime.Now});
                            }
                        }

                        assembly.assembly.MaterialCost = totalMaterialCost;
                        assembly.assembly.LaborCost = totalLabourCost;
                        //assembly.assembly.IsItemAssembly = assembly.assembly.IsItemAssembly;
                        assembly.assembly.IsItemAssembly = false;
                        conn.Update(assembly.assembly);

                        transactionScope.Complete();

                        return assembly.assembly;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        transactionScope.Dispose();
                        return assembly.assembly;
                    }
                }
            }
        }

        /// <summary>
        /// Delete an Item Assembly.
        /// </summary>
        /// <param name="assemblyId">Item Assembly unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int assemblyId)
        {
            var isDeleted = false;
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    try
                    {
                        var assembly = conn.Get<Assembly>(assemblyId);

                        var pgAssemblyItem = new PredicateGroup
                            {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                        pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq,
                            assemblyId));

                        var assemblyItemList = conn.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                        foreach (var assemblyItem in assemblyItemList)
                        {
                            conn.Delete(assemblyItem);
                        }

                        isDeleted = conn.Delete(assembly);
                        transactionScope.Complete();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        transactionScope.Dispose();
                    }
                }
            }

            return isDeleted;
        }

        /// <summary>
        /// Get all Assembly created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active item Assemblies else return all</param>
        /// <returns>List of item assemblies.</returns>
        public IList<Assembly> GetAllAssemblies(bool isActive = true)
        {
            IList<Assembly> assemblyList = new List<Assembly>();
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                try
                {
                    assemblyList = conn.GetList<Assembly>().ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return assemblyList;
        }

        /// <summary>
        /// Get Item Assembly details based on item id.
        /// </summary>
        /// <param name="assemblyId">Assembly's unique identifier</param>
        /// <returns>Assembly details.</returns>
        public AssemblyItemDTO GetAssembly(int assemblyId)
        {
            AssemblyItemDTO assemblyItemDto = null;
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                try
                {
                    var assembly = conn.Get<Assembly>(assemblyId);

                    var pgAssemblyItem = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq,
                        assemblyId));

                    var assemblyItemList = conn.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                    assemblyItemDto = new AssemblyItemDTO
                        {assembly = assembly, assemblyItem = assemblyItemList.ToList()};

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return assemblyItemDto;
        }

        /// <summary>
        /// Get Assembly By AssmeblyID
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public AssemblyItemDTO GetAssembly(int assemblyId, IDbConnection conn)
        {
            AssemblyItemDTO assemblyItemDto = null;

            try
            {
                var assembly = conn.Get<Assembly>(assemblyId);

                var pgAssemblyItem = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                pgAssemblyItem.Predicates.Add(
                    Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyId));

                var assemblyItemList = conn.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                assemblyItemDto = new AssemblyItemDTO {assembly = assembly, assemblyItem = assemblyItemList.ToList()};

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return assemblyItemDto;
        }

        public IList<Assembly> SearchAssembly(string searchText)
        {
            throw new NotImplementedException();
        }

        public bool SetActive(int assemblyId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public bool AddItemToAssembly(int assemblyId, AssemblyItem assemblyItem)
        {
            throw new NotImplementedException();
        }

        //public bool AddItemsToAssembly(int AssemblyId, IList<ItemComposition> itemCompositions)
        //{
        //    throw new NotImplementedException();
        //}

        public decimal CalculateTotalCost(int assemblyId)
        {
            throw new NotImplementedException();
        }
    }
}