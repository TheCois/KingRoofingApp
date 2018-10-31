using System;
using System.Collections.Generic;

namespace KRF.Core.Entities.Master
{
    public class Fleet
    {
        public int FleetID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public int FleetStatusID { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    public class FleetService
    {
        public int FleetServiceID { get; set; }
        public int FleetID { get; set; }
        public DateTime ServiceDate { get; set; }
        public string Notes { get; set; }
        public bool Active { get; set; }
    }

    public class FleetAssignment
    {
        public int FleetAssignmentID { get; set; }
        public int FleetID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime AssignmentDate { get; set; }
        public bool IsCurrent { get; set; }
        public bool Active { get; set; }
    }
}
