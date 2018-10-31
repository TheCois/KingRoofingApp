using System.Collections.Generic;
using KRF.Core.Entities.Master;

namespace KRF.Web.Models
{
    public class FleetData
    {
        /// <summary>
        /// Holds Fleet Information
        /// </summary>
        public Fleet Fleet { get; set; }
        /// <summary>
        /// Holds Fleet Service Information
        /// </summary>
        public List<FleetService> FleetServices { get; set; }
        /// <summary>
        /// Holds Fleet Assignment Information
        /// </summary>
        public List<FleetAssignment> FleetAssignments { get; set; }

    }
}