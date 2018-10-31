using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;

namespace KRF.Persistence.RepositoryImplementation
{
    public class VendorManagementRepository : IVendorManagementRepository
    {
        private readonly IVendorManagement vendorManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public VendorManagementRepository()
        {
            vendorManagement_ = ObjectFactory.GetInstance<IVendorManagement>();
        }
        /// <summary>
        /// Create Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public int CreateVendor(Vendor vendor)
        {
            return vendorManagement_.CreateVendor(vendor);
        }
        /// <summary>
        /// Edit Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public bool EditVendor(Vendor vendor)
        {
            return vendorManagement_.EditVendor(vendor);
        }
        /// <summary>
        /// Toggle vendor active field
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool SetActiveInactiveVendor(int vendorId, bool active)
        {
            return vendorManagement_.SetActiveInactiveVendor(vendorId, active);
        }
        /// <summary>
        /// Get Vendor by vendorID
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public VendorDTO GetVendor(int vendorId)
        {
            return vendorManagement_.GetVendor(vendorId);
        }
        /// <summary>
        /// Get all Vendors
        /// </summary>
        /// <returns></returns>
        public VendorDTO ListAllVendors()
        {
            return vendorManagement_.ListAllVendors();
        }
        
    }
}
