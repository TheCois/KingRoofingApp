using System.Collections.Generic;
using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;

namespace KRF.Persistence.RepositoryImplementation
{
    public class FleetManagementRepository : IFleetManagementRepository
    {
        private readonly IFleetManagement fleetManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public FleetManagementRepository()
        {
            fleetManagement_ = ObjectFactory.GetInstance<IFleetManagement>();
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
            return fleetManagement_.CreateFleet(fleet, fleetServices, fleetAssignments);
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
            return fleetManagement_.EditFleet(fleet, fleetServices, fleetAssignments);
        }
        /// <summary>
        /// Active/Inactive Fleet status
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleFleetStatus(int fleetId, bool tobeEnabled)
        {
            return fleetManagement_.ToggleFleetStatus(fleetId, tobeEnabled);
        }
        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <returns></returns>
        public FleetDTO GetFleetDetails()
        {
            return fleetManagement_.GetFleetDetails();
        }
        /// <summary>
        /// Get Fleet Detail by FleetID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public FleetDTO GetFleetDetail(int fleetId)
        {
            return fleetManagement_.GetFleetDetail(fleetId);
        }

    }
}
