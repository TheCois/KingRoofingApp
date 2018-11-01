using System;
using System.Collections.Generic;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using KRF.Core.Entities.Product;
using KRF.Core.DTO.Product;

namespace KRF.Persistence.RepositoryImplementation
{
    public class AssemblyManagementRepository : IAssemblyManagementRepository
    {
        private readonly IAssemblyManagement _AssemblyManagement;
        /// <summary>
        /// Constructor
        /// </summary>
        public AssemblyManagementRepository()
        {
            _AssemblyManagement = ObjectFactory.GetInstance<IAssemblyManagement>();
        }

        public int Create(AssemblyItemDTO assembly)
        {
           int id = _AssemblyManagement.Create(assembly);
           return id;
        }

        public Assembly Edit(AssemblyItemDTO assembly)
        {
            Assembly a = _AssemblyManagement.Edit(assembly);
            return a;
        }

        public bool Delete(int assemblyId)
        {
            return _AssemblyManagement.Delete(assemblyId);
        }

        public IList<Assembly> GetAllAssemblies(bool isActive = true)
        {
            return _AssemblyManagement.GetAllAssemblies(isActive);
        }

        public AssemblyItemDTO GetAssembly(int assemblyId)
        {
            return _AssemblyManagement.GetAssembly(assemblyId);
        }

        public IList<Assembly> SearchAssembly(string searchText)
        {
            throw new NotImplementedException();
        }

        public bool SetActive(int assemblyId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public bool AddItemToAssembly(int AssemblyId, KRF.Core.Entities.Product.AssemblyItem assemblyItem)
        {
            throw new NotImplementedException();
        }

        public bool AddItemsToAssembly(int AssemblyId, IList<ItemComposition> itemCompositions)
        {
            throw new NotImplementedException();
        }

        public decimal CalculateTotalCost(int AssemblyId)
        {
            throw new NotImplementedException();
        }
    }
}
