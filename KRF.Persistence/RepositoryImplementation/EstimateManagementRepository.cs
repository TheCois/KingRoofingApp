using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using KRF.Core.Entities.Product;
using StructureMap;
using KRF.Core.Entities.ValueList;
using KRF.Core.DTO.Product;
using KRF.Core.Entities.Sales;
using KRF.Core.DTO.Sales;

namespace KRF.Persistence.RepositoryImplementation
{
    public class EstimateManagementRepository : IEstimateManagementRepository
    {
        private readonly IEstimateManagement _IEstimateManagement;
        /// <summary>
        /// Constructor
        /// </summary>
        public EstimateManagementRepository()
        {
            _IEstimateManagement = ObjectFactory.GetInstance<IEstimateManagement>();
        }

        public int Create(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            return _IEstimateManagement.Create(estimate, estimateItem);
        }

        public Estimate Edit(Estimate estimate, IList<EstimateItem> estimateItem)
        {
            return _IEstimateManagement.Edit(estimate, estimateItem);
        }

        public bool Delete(int estimateId)
        {
            return _IEstimateManagement.Delete(estimateId);
        }

        public EstimateDTO ListAll()
        {
            return _IEstimateManagement.ListAll();
        }

        public EstimateDTO ListAllEstimatesForACustomer(int customerId)
        {
            throw new NotImplementedException();
        }

        public EstimateDTO Select(int estimateId)
        {
            return _IEstimateManagement.Select(estimateId);
        }

        public int SaveDocument(EstimateDocument estimateDocument)
        {
            return _IEstimateManagement.SaveDocument(estimateDocument);
        }

        public bool DeleteEstimateDocument(int estimateDocumentID)
        {
            return _IEstimateManagement.DeleteEstimateDocument(estimateDocumentID);
        }

        public EstimateDocument GetEstimateDocument(int estimateDocumentID)
        {
            return _IEstimateManagement.GetEstimateDocument(estimateDocumentID);
        }

        public IList<EstimateDocument> GetEstimateDocuments(int estimateID)
        {
            return _IEstimateManagement.GetEstimateDocuments(estimateID);
        }

        public byte[] CreateProposalDocument(int estimateID)
        {
            return _IEstimateManagement.CreateProposalDocument(estimateID);
        }
    }
}
