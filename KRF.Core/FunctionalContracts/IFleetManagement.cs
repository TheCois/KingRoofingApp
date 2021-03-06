﻿using System.Collections.Generic;
using KRF.Core.Entities.Master;
using KRF.Core.DTO.Master;

namespace KRF.Core.FunctionalContracts
{
    public interface IFleetManagement
    {
        /// <summary>
        /// Create new fleet record in DB
        /// </summary>
        /// <param name="fleet"></param>
        /// <param name="fleetServices"></param>
        /// <param name="fleetAssignments"></param>
        /// <returns></returns>
        int CreateFleet(Fleet fleet, List<FleetService> fleetServices, List<FleetAssignment> fleetAssignments);
        /// <summary>
        /// Edit Fleet record
        /// </summary>
        /// <param name="fleet"></param>
        /// <param name="fleetServices"></param>
        /// <param name="fleetAssignments"></param>
        /// <returns></returns>
        bool EditFleet(Fleet fleet, List<FleetService> fleetServices, List<FleetAssignment> fleetAssignments);
        /// <summary>
        /// Active/Inactive Fleet status
        /// </summary>
        /// <param name="fleetID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        bool ToggleFleetStatus(int fleetID, bool tobeEnabled);
        /// <summary>
        /// Get Fleet Detail by FleetID
        /// </summary>
        /// <param name="fleetID"></param>
        /// <returns></returns>
        FleetDTO GetFleetDetail(int fleetID);
        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <returns></returns>
        FleetDTO GetFleetDetails();
    }
}
