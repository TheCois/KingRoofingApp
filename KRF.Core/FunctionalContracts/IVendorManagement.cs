using KRF.Core.Entities.Master;
using KRF.Core.DTO.Master;

namespace KRF.Core.FunctionalContracts
{
    public interface IVendorManagement
    {
        /// <summary>
        /// Create Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        int CreateVendor(Vendor vendor);
        /// <summary>
        /// Edit Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        bool EditVendor(Vendor vendor);
        /// <summary>
        /// Toggle vendor active field
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool SetActiveInactiveVendor(int vendorID, bool active);
        /// <summary>
        /// Get Vendor by vendorID
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        VendorDTO GetVendor(int vendorID);
        /// <summary>
        /// Get all Vendors
        /// </summary>
        /// <returns></returns>
        VendorDTO ListAllVendors();

    }
}
