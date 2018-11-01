using System;

namespace KRF.Core.Entities.Customer
{
    public class JobInspection
    {
        public int InspID { get; set; }
        public int JobID { get; set; }
        public string JobAddress { get; set; }
        public int EmployeeID { get; set; }
        public int PermitID { get; set; }
        public string Phone { get; set; }
        public DateTime CalledDate { get; set; }
        public DateTime? ResultDate { get; set; }
        public int InspectionID { get; set; }
        public int? StatusID { get; set; }
        public int InspectorID { get; set; }
        public string Notes { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
