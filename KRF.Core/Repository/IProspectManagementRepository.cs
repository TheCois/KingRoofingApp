using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
using System.Collections.Generic;

namespace KRF.Core.Repository
{
    public interface IProspectManagementRepository
    {
        /// <summary>
        /// Create an Prospect
        /// </summary>
        /// <param name="Prospect">Prospect details</param>
        /// <returns>Newly created Prospect identifier</returns>
        int Create(Prospect prosppect);

        /// <summary>
        /// Edit an Prospect based on updated Prospect details.
        /// </summary>
        /// <param name="Prospect">Updated Prospect details.</param>
        /// <returns>Updated Prospect details.</returns>
        Prospect Edit(Prospect prospect);

        /// <summary>
        /// Save prospects
        /// </summary>
        /// <param name="prospects"></param>
        void SaveProspects(IList<Prospect> prospects);

        /// <summary>
        /// Delete an  Prospect.
        /// </summary>
        /// <param name="ProspectId"> Prospect unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool Delete(int Id);

        /// <summary>
        /// Get all Prospect created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active  Prospects else return all</param>
        /// <returns>List of  Prospects.</returns>
        ProspectDTO GetProspects(bool isActive = true);

        /// <summary>
        /// Get  Prospect details based on  id.
        /// </summary>
        /// <param name="ProspectId">Prospect's unique identifier</param>
        /// <returns>Prospect details.</returns>
        ProspectDTO GetProspect(int Id);

        /// <summary>
        /// Search and filter Prospect based on search text.
        /// </summary>
        /// <param name="searchText">Search text which need to be mapped with any of  Prospect related fields.</param>
        /// <returns> Prospect list.</returns>
        IList<Prospect> SearchProspect(string searchText);

        /// <summary>
        /// Set  Prospect to active / Deactive
        /// </summary>
        /// <param name="ProspectId"> Prospect unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool SetActive(int ProspectId, bool isActive);
    }
}
