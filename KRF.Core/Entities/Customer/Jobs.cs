using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.Customer
{
    public class Job
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int LeadID { get; set; }
        public int JobAddressID { get; set; }
        public string JobAddress1 { get; set; }
        public string JobAddress2 { get; set; }
        public int? JobState { get; set; }
        public int? JobCity { get; set; }
        public string JobZipCode { get; set; }
        public int EstimateID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }
        public decimal? EstimatedLabourHours { get; set; }
        public decimal? AverageWorkingHours { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? JobStatusID { get; set; }
        public bool Status { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
