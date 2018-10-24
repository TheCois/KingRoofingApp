using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.Actors;
using KRF.Core.Entities.Sales;
using KRF.Core.DTO.Sales;

namespace KRF.Core.FunctionalContracts
{
    public interface IEstimateManagement
    {
        /// <summary>
        /// Create an estimate for a particular customer
        /// </summary>
        /// <param name="customerId">Customer unique identifier</param>
        /// <param name="estimate">Estimate details</param>
        /// <returns>Newly created estimate id.</returns>
        int Create(Estimate estimate, IList<EstimateItem> estimateItem);

        /// <summary>
        /// Edit an customer's estimate
        /// </summary>
        /// <param name="customerId">Customer unique identifier</param>
        /// <param name="estimate">Updated estimate details</param>
        /// <returns>Updated estimate details</returns>
        Estimate Edit(Estimate estimate, IList<EstimateItem> estimateItem);

        /// <summary>
        /// Delete an existing estimate.
        /// </summary>
        /// <param name="estimateId">Estimate unique identifier</param>
        /// <returns>True - if successful else false.</returns>
        bool Delete(int estimateId);

        /// <summary>
        /// List all estimates inside the system.
        /// </summary>
        /// <returns>Estimate list</returns>
        EstimateDTO ListAll();

        /// <summary>
        /// List all estimates associated with a customer
        /// </summary>
        /// <param name="customerId">Customer unique identifier</param>
        /// <returns>Estimate list.</returns>
        IList<EstimateDTO> ListAllEstimatesForACustomer(int customerId);

        /// <summary>
        /// Select a particular estimate based on its identifier
        /// </summary>
        /// <param name="estimateId">Estimate unique identifier</param>
        /// <returns>Estimate detail.</returns>
        EstimateDTO Select(int estimateId);


        /// <summary>
        /// Save Estimate document
        /// </summary>
        /// <param name="estimateDocument"></param>
        /// <returns></returns>
        int SaveDocument(EstimateDocument estimateDocument);

        /// <summary>
        /// Delete Estimate Document
        /// </summary>
        /// <param name="estimateDocumentID"></param>
        /// <returns></returns>
        bool DeleteEstimateDocument(int estimateDocumentID);

        /// <summary>
        /// Get a estimate document
        /// </summary>
        /// <param name="estimateDocumentID"></param>
        /// <returns></returns>
        EstimateDocument GetEstimateDocument(int estimateDocumentID);

        /// <summary>
        /// Get list of Estimate document
        /// </summary>
        /// <param name="estimateID"></param>
        /// <returns></returns>
        IList<EstimateDocument> GetEstimateDocuments(int estimateID);


        /// <summary>
        /// Create document dynamically
        /// </summary>
        /// <param name="estimateID"></param>
        byte[] CreateProposalDocument(int estimateID);
    }
}
