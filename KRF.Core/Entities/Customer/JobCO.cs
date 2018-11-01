using System;

namespace KRF.Core.Entities.Customer
{
    public class JobCO
    {
        public int COID { get; set; }
        public int JobID { get; set; }
        public string COCode { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int? SalesRepID { get; set; }
        public string SalesRepEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
    public class COEstimateItem
    {
        public int ID { get; set; }
        public int COID { get; set; }
        public int ItemAssemblyID { get; set; }
        public int ItemAssemblyType { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
    }
}
