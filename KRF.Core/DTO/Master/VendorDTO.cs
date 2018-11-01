using KRF.Core.Entities.Master;
using System.Collections.Generic;
namespace KRF.Core.DTO.Master
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class VendorDTO
    {
        public IList<Vendor> Vendors { get; set; }
    }
}
