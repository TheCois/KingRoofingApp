using KRF.Core.DTO.Product;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class ProspectManagementRepository : IProspectManagementRepository
    {
        private readonly IProspectManagement _ProspectManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProspectManagementRepository()
        {
            _ProspectManagement = ObjectFactory.GetInstance<IProspectManagement>();
        }

        /// <summary>
        /// Create an Prospect
        /// </summary>
        /// <param name="Prospect">Prospect details</param>
        /// <returns>Newly created Prospect identifier</returns>
        public int Create(Prospect prospect)
        {
            return _ProspectManagement.Create(prospect);
        }

        /// <summary>
        /// Edit an Prospect based on updated Prospect details.
        /// </summary>
        /// <param name="Prospect">Updated Prospect details.</param>
        /// <returns>Updated Prospect details.</returns>
        public Prospect Edit(Prospect prospect)
        {
            return _ProspectManagement.Edit(prospect);
        }


        public void SaveProspects(IList<Prospect> prospects)
        {
            _ProspectManagement.SaveProspects(prospects);
        }
        /// <summary>
        /// Delete an  Prospect.
        /// </summary>
        /// <param name="ProspectId"> Prospect unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int id)
        {
            return _ProspectManagement.Delete(id);
        }

        /// <summary>
        /// Get all Prospect created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active  Prospects else return all</param>
        /// <returns>List of  Prospects.</returns>
        public ProspectDTO GetProspects(bool isActive = true)
        {
            return _ProspectManagement.GetProspects(isActive);
        }

        /// <summary>
        /// Get  Prospect details based on  id.
        /// </summary>
        /// <param name="ProspectId">Prospect's unique identifier</param>
        /// <returns>Prospect details.</returns>
        public ProspectDTO GetProspect(int id)
        {
            return _ProspectManagement.GetProspect(id);
        }

        /// <summary>
        /// Search and filter Prospect based on search text.
        /// </summary>
        /// <param name="searchText">Search text which need to be mapped with any of  Prospect related fields.</param>
        /// <returns> Prospect list.</returns>
        public IList<Prospect> SearchProspect(string searchText)
        {
            return _ProspectManagement.SearchProspect(searchText);
        }

        /// <summary>
        /// Set  Prospect to active / Deactive
        /// </summary>
        /// <param name="ProspectId"> Prospect unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool SetActive(int prospectId, bool isActive)
        {
            return _ProspectManagement.SetActive(prospectId, isActive);
        }
    }
}
