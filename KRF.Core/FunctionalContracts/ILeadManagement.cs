using KRF.Core.Entities.Sales;
using System;
using System.Collections.Generic;
using KRF.Core.DTO.Sales;

namespace KRF.Core.FunctionalContracts
{
    public interface ILeadManagement
    {
        /// <summary>
        /// Create an Lead
        /// </summary>
        /// <param name="Lead">Lead details</param>
        /// <returns>Newly created Lead identifier</returns>
        int Create(Lead lead, IList<CustomerAddress> customerAddress);

        /// <summary>
        /// Edit an Lead based on updated Lead details.
        /// </summary>
        /// <param name="Lead">Updated Lead details.</param>
        /// <returns>Updated Lead details.</returns>
        Lead Edit(Lead lead, IList<CustomerAddress> customerAddress);

        /// <summary>
        /// Create Job Address
        /// </summary>
        /// <param name="customerAddress"></param>
        /// <returns></returns>
        int CreateJobAddress(IList<CustomerAddress> customerAddress);

        /// <summary>
        /// Edit Job Address
        /// </summary>
        /// <param name="customerAddress"></param>
        /// <returns></returns>
        bool EditJobAddress(IList<CustomerAddress> customerAddress);

        /// <summary>
        /// Delete Job Address
        /// </summary>
        /// <param name="JobAddID"></param>
        /// <returns></returns>
        bool DeleteJobAddress(int JobAddID);

        /// <summary>
        /// Delete an  Lead.
        /// </summary>
        /// <param name="LeadId"> Lead unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool Delete(int Id);

        /// <summary>
        /// Get all Lead created in the system.
        /// </summary>
        /// <param name="isActive">If true - returns only active  Leads else return all</param>
        /// <returns>List of  Leads.</returns>
        LeadDTO GetLeads(Func<Lead, bool> predicate, bool isActive = true);

        /// <summary>
        /// Get  Lead details based on  id.
        /// </summary>
        /// <param name="LeadId">Lead's unique identifier</param>
        /// <returns>Lead details.</returns>
        LeadDTO GetLead(int Id);

        /// <summary>
        /// Search and filter Lead based on search text.
        /// </summary>
        /// <param name="searchText">Search text which need to be mapped with any of  Lead related fields.</param>
        /// <returns> Lead list.</returns>
        IList<Lead> SearchLead(string searchText);

        /// <summary>
        /// Set  Lead to active / Deactive
        /// </summary>
        /// <param name="LeadId"> Lead unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        bool SetActive(int LeadId, bool isActive);
    }
}
