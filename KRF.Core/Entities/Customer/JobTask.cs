using System;

namespace KRF.Core.Entities.Customer
{
    public class JobTask
    {
        public int TaskID { get; set; }
        public int JobID { get; set; }
        public string TaskName { get; set; }
        public DateTime? InspectionScheduledDate { get; set; }
        public DateTime? InspectionCompletedDate { get; set; }
        public string InspectionScheduledConfirmationNo { get; set; }
        public string InspectionCompletedConfirmationNo { get; set; }
        public DateTime? TaskStartDate { get; set; }
        public DateTime? TaskCompletedDate { get; set; }
        public string InspectionNotes { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }    
    }
}
