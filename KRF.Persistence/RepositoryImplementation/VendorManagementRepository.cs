using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class VendorManagementRepository : IVendorManagementRepository
    {
        private readonly IVendorManagement _VendorManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public VendorManagementRepository()
        {
            _VendorManagement = ObjectFactory.GetInstance<IVendorManagement>();
        }
        /// <summary>
        /// Create Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public int CreateVendor(Vendor vendor)
        {
            return _VendorManagement.CreateVendor(vendor);
        }
        /// <summary>
        /// Edit Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public bool EditVendor(Vendor vendor)
        {
            return _VendorManagement.EditVendor(vendor);
        }
        /// <summary>
        /// Toggle vendor active field
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool SetActiveInactiveVendor(int vendorID, bool active)
        {
            return _VendorManagement.SetActiveInactiveVendor(vendorID, active);
        }
        /// <summary>
        /// Get Vendor by vendorID
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public VendorDTO GetVendor(int vendorID)
        {
            return _VendorManagement.GetVendor(vendorID);
        }
        /// <summary>
        /// Get all Vendors
        /// </summary>
        /// <returns></returns>
        public VendorDTO ListAllVendors()
        {
            return _VendorManagement.ListAllVendors();
        }
        
    }
}
