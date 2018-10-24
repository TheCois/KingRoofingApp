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
using System.Data;
using KRF.Core.DTO.Product;
using System.Transactions;
using System.Configuration;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class AssemblyManagement : IAssemblyManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public AssemblyManagement()
        {
            //_connectionString = ObjectFactory.GetInstance<IDatabaseConnection>().ConnectionString;
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        /// <summary>
        /// Create an Assembly
        /// </summary>
        /// <param name="Assembly">Assembly details</param>
        /// <returns>Newly created Assembly identifier</returns>
        public int Create(AssemblyItemDTO assembly)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        decimal totalMaterialCost = 0;
                        decimal totalLabourCost = 0;
                        foreach (var i in assembly.assemblyItem)
                        {
                            var item = sqlConnection.Get<Item>(i.ItemId);
                            if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Labor)
                            {
                                totalLabourCost += Convert.ToDecimal(i.RetailCost);
                            }
                            else if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Material)
                            {
                                totalMaterialCost += Convert.ToDecimal(i.RetailCost);
                            }
                        }
                        assembly.assembly.MaterialCost = totalMaterialCost;
                        assembly.assembly.LaborCost = totalLabourCost;

                        var id = sqlConnection.Insert<Assembly>(assembly.assembly);
                        var inventories = sqlConnection.GetList<Inventory>().ToList();
                        foreach (var i in assembly.assemblyItem)
                        {
                            i.AssemblyId = id;
                            var assemblyItemId = sqlConnection.Insert<AssemblyItem>(i);
                            var item = sqlConnection.Get<Item>(i.ItemId);
                            if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Labor)
                            {
                                totalLabourCost += Convert.ToDecimal(i.RetailCost);
                            }
                            else if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Material)
                            {
                                totalMaterialCost += Convert.ToDecimal(i.RetailCost);
                            }
                            
                            if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Material && (!inventories.Any(p=>p.ItemID == i.ItemId)))
                            {

                                //Create record in Inventory table with default quantity 0
                                sqlConnection.Insert<Inventory>(new Inventory() { ItemID = i.ItemId, Qty = 0, DateUpdated = DateTime.Now });
                            }
                        }

                        transactionScope.Complete();
                        return id;
                    }
                    catch (Exception e)
                    {
                        transactionScope.Dispose();
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Edit an Assembly based on updated Assembly details.
        /// </summary>
        /// <param name="Assembly">Updated Assembly details.</param>
        /// <returns>Updated Assembly details.</returns>
        public Assembly Edit(AssemblyItemDTO assembly)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        var pgAssemblyItem = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                        pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assembly.assembly.Id));

                        var assemblyItemList = sqlConnection.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                        var inventories = sqlConnection.GetList<Inventory>().ToList();
                        foreach (var i in assemblyItemList)
                        {
                            //if (assembly.assemblyItem.Where(k => k.ItemId == i.ItemId).Count() == 0)
                            //{
                            //    sqlConnection.Delete<AssemblyItem>(i.id);
                            //}
                            sqlConnection.Delete<AssemblyItem>(i);
                        }
                        decimal totalMaterialCost = 0;
                        decimal totalLabourCost = 0;
                        foreach (var i in assembly.assemblyItem)
                        {
                            Item item = sqlConnection.Get<Item>(i.ItemId);
                            if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Labor)
                            {
                                totalLabourCost += Convert.ToDecimal(i.RetailCost);
                            }
                            else if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Material)
                            {
                                totalMaterialCost += Convert.ToDecimal(i.RetailCost);
                            }
                            //if (i.id > 0)
                            //    sqlConnection.Update<AssemblyItem>(i);
                            //else
                            sqlConnection.Insert<AssemblyItem>(i);
                            if (item.ItemTypeId == (int)KRF.Core.Enums.ItemType.Material && (!inventories.Any(p => p.ItemID == i.ItemId)))
                            {
                                //Create record in Inventory table with default quantity 0
                                sqlConnection.Insert<Inventory>(new Inventory() { ItemID = i.ItemId, Qty = 0, DateUpdated = DateTime.Now });
                            }
                        }
                        assembly.assembly.MaterialCost = totalMaterialCost;
                        assembly.assembly.LaborCost = totalLabourCost;
                        //assembly.assembly.IsItemAssembly = assembly.assembly.IsItemAssembly;
                        assembly.assembly.IsItemAssembly = false;
                        sqlConnection.Update<Assembly>(assembly.assembly);

                        transactionScope.Complete();

                        return assembly.assembly;
                    }
                    catch (Exception e)
                    {
                        transactionScope.Dispose();
                        return assembly.assembly;
                    }
                }
            }
        }

        /// <summary>
        /// Delete an Item Assembly.
        /// </summary>
        /// <param name="AssemblyId">Item Assembly unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int assemblyId)
        {
            bool isDeleted = false;
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    try
                    {
                        var assembly = sqlConnection.Get<Assembly>(assemblyId);

                        var pgAssemblyItem = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                        pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyId));

                        var assemblyItemList = sqlConnection.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                        foreach (var assemblyItem in assemblyItemList)
                        {
                            var i = sqlConnection.Delete<AssemblyItem>(assemblyItem);
                        }

                        isDeleted = sqlConnection.Delete<Assembly>(assembly);
                        transactionScope.Complete();
                    }
                    catch (Exception e)
                    {
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
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                try
                {
                    assemblyList = sqlConnection.GetList<Assembly>().ToList();
                }
                catch (Exception e)
                {
                }
            }
            return assemblyList;
        }

        /// <summary>
        /// Get Item Assembly details based on item id.
        /// </summary>
        /// <param name="AssemblyId">Assembly's unique identifier</param>
        /// <returns>Assembly details.</returns>
        public AssemblyItemDTO GetAssembly(int assemblyId)
        {
            AssemblyItemDTO assemblyItemDTO = null;
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                try
                {
                    var assembly = sqlConnection.Get<Assembly>(assemblyId);

                    var pgAssemblyItem = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyId));

                    var assemblyItemList = sqlConnection.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                    assemblyItemDTO = new AssemblyItemDTO { assembly = assembly, assemblyItem = assemblyItemList.ToList() };

                }
                catch (Exception e)
                {
                }
            }
            return assemblyItemDTO;
        }
        /// <summary>
        /// Get Assmebly By AssmeblyID
        /// </summary>
        /// <param name="assemblyId"></param>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public AssemblyItemDTO GetAssembly(int assemblyId, SqlConnection sqlConnection)
        {
            AssemblyItemDTO assemblyItemDTO = null;

            try
            {
                var assembly = sqlConnection.Get<Assembly>(assemblyId);

                var pgAssemblyItem = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                pgAssemblyItem.Predicates.Add(Predicates.Field<AssemblyItem>(s => s.AssemblyId, Operator.Eq, assemblyId));

                var assemblyItemList = sqlConnection.GetList<AssemblyItem>(pgAssemblyItem).ToList();
                assemblyItemDTO = new AssemblyItemDTO { assembly = assembly, assemblyItem = assemblyItemList.ToList() };

            }
            catch (Exception e)
            {
            }

            return assemblyItemDTO;
        }

        public IList<Assembly> SearchAssembly(string searchText)
        {
            throw new NotImplementedException();
        }

        public bool SetActive(int assemblyId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public bool AddItemToAssembly(int AssemblyId, AssemblyItem assemblyItem)
        {
            throw new NotImplementedException();
        }

        //public bool AddItemsToAssembly(int AssemblyId, IList<ItemComposition> itemCompositions)
        //{
        //    throw new NotImplementedException();
        //}

        public decimal CalculateTotalCost(int AssemblyId)
        {
            throw new NotImplementedException();
        }
    }
}
