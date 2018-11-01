using System;

namespace KRF.Core.Entities.Customer
{
    public class JobAssignment
    {
        public int JobAssignmentID { get; set; }
        public int JobID { get; set; }
        /// <summary>
        /// Hold EmployeeID/CrewID based on Type field
        /// </summary>
        public int ObjectPKID { get; set; }
        public string Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
