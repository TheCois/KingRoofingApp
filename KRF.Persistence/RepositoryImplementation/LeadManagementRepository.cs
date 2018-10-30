using KRF.Core.DTO.Product;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System;
using System.Collections.Generic;

namespace KRF.Persistence.RepositoryImplementation
{
    public class LeadManagementRepository : ILeadManagementRepository
    {
        private readonly ILeadManagement leadManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public LeadManagementRepository()
        {
            leadManagement_ = ObjectFactory.GetInstance<ILeadManagement>();
        }


        /// <summary>
        /// Create an Lead
        /// </summary>
        /// <param name="lead">Lead details</param>
        /// <returns>Newly created Lead identifier</returns>
        public int Create(Lead lead)
        {
            return leadManagement_.Create(lead, new List<CustomerAddress>());
        }

        /// <summary>
        /// Edit an Lead based on updated Lead details.
        /// </summary>
        /// <param name="lead">Updated Lead details.</param>
        /// <returns>Updated Lead details.</returns>
        public Lead Edit(Lead lead)
        {
            return leadManagement_.Edit(lead, new List<CustomerAddress>());
        }

        /// <summary>
        /// Create an Lead
        /// </summary>
        /// <param name="lead">Lead details</param>
        /// <returns>Newly created Lead identifier</returns>
        public int Create(Lead lead, IList<CustomerAddress> customerAddress)
        {
            return leadManagement_.Create(lead, customerAddress);
        }

        /// <summary>
        /// Edit an Lead based on updated Lead details.
        /// </summary>
        /// <param name="lead">Updated Lead details.</param>
        /// <returns>Updated Lead details.</returns>
        public Lead Edit(Lead lead, IList<CustomerAddress> customerAddress)
        {
            return leadManagement_.Edit(lead, customerAddress);
        }

        /// <summary>
        /// Delete a Lead.
        /// </summary>
        /// <param name="id"> Lead unique identifier</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool Delete(int id)
        {
            return leadManagement_.Delete(id);
        }

        /// <summary>
        /// Get all Lead created in the system.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="isActive">If true - returns only active  Leads else return all</param>
        /// <returns>List of  Leads.</returns>
        public LeadDTO GetLeads(Func<Lead, bool> predicate, bool isActive = true)
        {
            return leadManagement_.GetLeads(predicate, isActive);
        }

        /// <summary>
        /// Get  Lead details based on  id.
        /// </summary>
        /// <param name="id">Lead's unique identifier</param>
        /// <returns>Lead details.</returns>
        public LeadDTO GetLead(int id)
        {
            return leadManagement_.GetLead(id);
        }

        /// <summary>
        /// Search and filter Lead based on search text.
        /// </summary>
        /// <param name="searchText">Search text which need to be mapped with any of  Lead related fields.</param>
        /// <returns> Lead list.</returns>
        public IList<Lead> SearchLead(string searchText)
        {
            return leadManagement_.SearchLead(searchText);
        }

        /// <summary>
        /// Set  Lead to active / Deactive
        /// </summary>
        /// <param name="leadId"> Lead unique identifier</param>
        /// <param name="isActive">True - active; False - Deactive.</param>
        /// <returns>True - if successful deletion; False - If failure.</returns>
        public bool SetActive(int leadId, bool isActive)
        {
            return leadManagement_.SetActive(leadId, isActive); ;
        }

        /// <summary>
        /// Create Job Address
        /// </summary>
        /// <param name="customerAddress"></param>
        /// <returns></returns>
        public int CreateJobAddress(IList<CustomerAddress> customerAddress)
        {
            return leadManagement_.CreateJobAddress(customerAddress);
        }

        /// <summary>
        /// Edit Job Address
        /// </summary>
        /// <param name="customerAddress"></param>
        /// <returns></returns>
        public bool EditJobAddress(IList<CustomerAddress> customerAddress)
        {
            return leadManagement_.EditJobAddress(customerAddress);
        }

        /// <summary>
        /// Delete Job Address
        /// </summary>
        /// <param name="jobAddId"></param>
        /// <returns></returns>
        public bool DeleteJobAddress(int jobAddId)
        {
            return leadManagement_.DeleteJobAddress(jobAddId);
        }
    }
}
