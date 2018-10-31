using KRF.Core.DTO.Product;
using KRF.Core.Entities.Product;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KRF.Core.FunctionalContracts
{
    public interface IAssemblyManagement
    {
        /// <summary>
        /// Create an Assembly
        /// </summary>
        /// <param name="Assembly">Assembly details</param>
        /// <returns>Newly created Assembly identifier</returns>
        int Create(AssemblyItemDTO assembly);

        /// <summary>
        /// Edit an Assembly based on updated Assembly details.
        /// </summary>
        /// <param name="Assembly">Updated Assembly details.</param>
        /// <returns>Updated Assembly details.</returns>
        Assembly Edit(AssemblyItemDTO assembly);

        /// <summary>
        /// Delete an Item Assembly.
        /// </summary>
        /// <param name="AssemblyId">Item Assembly unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool Delete(int assemblyId);

        /// <summary>
        /// Get all Assembly created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active item Assemblies else return all</param>
        /// <returns>List of item assemblies.</returns>
        IList<Assembly> GetAllAssemblies(bool isActive = true);

        /// <summary>
        /// Get Item Assembly details based on assembly id.
        /// </summary>
        /// <param name="AssemblyId">Assembly's unique identifier</param>
        /// <returns>Assembly details.</returns>
        AssemblyItemDTO GetAssembly(int assemblyId);
        /// <summary>
        /// Get Item Assembly details based on assembly id.
        /// </summary>
        /// <param name="AssemblyId">Assembly's unique identifier</param>
        /// <returns>Assembly details.</returns>
        AssemblyItemDTO GetAssembly(int assemblyId, IDbConnection conn);

        /// <summary>
        /// Search and filter Assembly based on search text.
        /// </summary>
        /// <param name="searchText">Search text which need to be mapped with any of Item Assembly related fields.</param>
        /// <returns>Item Assembly list.</returns>
        IList<Assembly> SearchAssembly(string searchText);

        /// <summary>
        /// Set Item Assembly to active / Deactive
        /// </summary>
        /// <param name="AssemblyId">Item Assembly unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool SetActive(int assemblyId, bool isActive);

        /// <summary>
        /// Add an item to item assembly.
        /// </summary>
        /// <param name="AssemblyId">Item assembly unique identifier</param>
        /// <param name="itemComposition">Item composition details</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool AddItemToAssembly(int AssemblyId, AssemblyItem assemblyItem);

        /// <summary>
        /// Add a set of items to item assembly.
        /// </summary>
        /// <param name="AssemblyId">Item assembly unique identifier</param>
        /// <param name="itemCompositions">List of item composition.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        //bool AddItemsToAssembly(int AssemblyId, IList<ItemComposition> itemCompositions);

        /// <summary>
        /// Calculate and return the total cost of an item assembly.
        /// </summary>
        /// <param name="AssemblyId">item assembly unique identifier</param>
        /// <returns>Total cost of an item assembly.</returns>
        decimal CalculateTotalCost(int AssemblyId);

    }
}
