using KRF.Core.Entities.ValueList;
using KRF.Core.Entities.Master;
using System.Collections.Generic;
namespace KRF.Core.DTO.Master
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class FleetDTO
    {
        public IList<Fleet> Fleets { get; set; }
        public IList<FleetService> FleetServices { get; set; }
        public IList<FleetAssignment> FleetAssignments { get; set; }
        public IList<FleetStatus> FleetStatus { get; set; }
        public IList<KRF.Core.Entities.Employee.Employee> Employes { get; set; }
    }
}
