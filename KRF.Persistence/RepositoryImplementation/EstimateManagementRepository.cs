using System;
using System.Collections.Generic;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using KRF.Core.Entities.Sales;
using KRF.Core.DTO.Sales;

namespace KRF.Persistence.RepositoryImplementation
{
    public class EstimateManagementRepository : IEstimateManagementRepository
    {
        private readonly IEstimateManagement iEstimateManagement_;
        /// <summary>
        /// Constructor
        /// </summary>
        public EstimateManagementRepository()
        {
            iEstimateManagement_ = ObjectFactory.GetInstance<IEstimateManagement>();
        }

        public int Create(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            return iEstimateManagement_.Create(estimate, estimateItem);
        }

        public Estimate Edit(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            return iEstimateManagement_.Edit(estimate, estimateItem);
        }

        public bool Delete(int estimateId)
        {
            return iEstimateManagement_.Delete(estimateId);
        }

        public EstimateDTO ListAll()
        {
            return iEstimateManagement_.ListAll();
        }

        public EstimateDTO ListAllEstimatesForACustomer(int customerId)
        {
            throw new NotImplementedException();
        }

        public EstimateDTO Select(int estimateId)
        {
            return iEstimateManagement_.Select(estimateId);
        }

        public int SaveDocument(EstimateDocument estimateDocument)
        {
            return iEstimateManagement_.SaveDocument(estimateDocument);
        }

        public bool DeleteEstimateDocument(int estimateDocumentID)
        {
            return iEstimateManagement_.DeleteEstimateDocument(estimateDocumentID);
        }

        public EstimateDocument GetEstimateDocument(int estimateDocumentID)
        {
            return iEstimateManagement_.GetEstimateDocument(estimateDocumentID);
        }

        public IList<EstimateDocument> GetEstimateDocuments(int estimateID)
        {
            return iEstimateManagement_.GetEstimateDocuments(estimateID);
        }

        public byte[] CreateProposalDocument(int estimateID)
        {
            return iEstimateManagement_.CreateProposalDocument(estimateID);
        }
    }
}
