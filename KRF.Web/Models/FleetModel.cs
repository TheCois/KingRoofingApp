using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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