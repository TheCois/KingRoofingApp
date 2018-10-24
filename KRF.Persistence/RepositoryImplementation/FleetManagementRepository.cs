using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class FleetManagementRepository : IFleetManagementRepository
    {
        private readonly IFleetManagement _FleetManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public FleetManagementRepository()
        {
            _FleetManagement = ObjectFactory.GetInstance<IFleetManagement>();
        }
        /// <summary>
        /// Create new fleet record in DB
        /// </summary>
        /// <param name="fleet"></param>
        /// <param name="fleetServices"></param>
        /// <param name="fleetAssignments"></param>
        /// <returns></returns>
        public int Create(Fleet fleet, List<FleetService> fleetServices, List<FleetAssignment> fleetAssignments)
        {
            return _FleetManagement.CreateFleet(fleet, fleetServices, fleetAssignments);
        }
        /// <summary>
        /// Edit Fleet record
        /// </summary>
        /// <param name="fleet"></param>
        /// <param name="fleetServices"></param>
        /// <param name="fleetAssignments"></param>
        /// <returns></returns>
        public bool Edit(Fleet fleet, List<FleetService> fleetServices, List<FleetAssignment> fleetAssignments)
        {
            return _FleetManagement.EditFleet(fleet, fleetServices, fleetAssignments);
        }
        /// <summary>
        /// Active/Inactive Fleet status
        /// </summary>
        /// <param name="fleetID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleFleetStatus(int fleetID, bool tobeEnabled)
        {
            return _FleetManagement.ToggleFleetStatus(fleetID, tobeEnabled);
        }
        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <returns></returns>
        public FleetDTO GetFleetDetails()
        {
            return _FleetManagement.GetFleetDetails();
        }
        /// <summary>
        /// Get Fleet Detail by FleetID
        /// </summary>
        /// <param name="fleetID"></param>
        /// <returns></returns>
        public FleetDTO GetFleetDetail(int fleetID)
        {
            return _FleetManagement.GetFleetDetail(fleetID);
        }

    }
}
