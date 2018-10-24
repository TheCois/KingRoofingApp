using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.Sales;
using KRF.Core.Entities.Master;
using System.Collections.Generic;
namespace KRF.Core.DTO.Master
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class CrewDTO
    {
        public IList<Crew> Crews { get; set; }
        public IList<CrewDetail> CrewDetails { get; set; }
        public IList<KRF.Core.Entities.Employee.Employee> Employees { get; set; }

    }
}
