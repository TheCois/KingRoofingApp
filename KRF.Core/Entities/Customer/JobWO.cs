using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.Customer
{
    public class JobWO
    {
        public int WOID { get; set; }
        public int JobID { get; set; }
        public string WOCode { get; set; }
        public string WODesc { get; set; }
        public DateTime WorkWeekEndingDate { get; set; }
        public int? CrewID { get; set; }
        public int? LeadID { get; set; }
        public int? EstimateID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalJobBalanceAmount { get; set; }
        public string COIDs { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
    public class WOEstimateItem
    {
        public int ID { get; set; }
        public int WOID { get; set; }
        public int ItemAssemblyID { get; set; }
        public int ItemAssemblyType { get; set; }
        public decimal Budget { get; set; }
        public decimal Used { get; set; }
        public decimal Rate { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public string ItemNames { get; set; }
        public int COID { get; set; }
        public int ItemID { get; set; }
    }
    public class vw_JobAssignmentCrewLeaders
    {
        public int JobID { get; set; }
        public int EmpId { get; set; }
        public string LeadName { get; set; }
    }
}
