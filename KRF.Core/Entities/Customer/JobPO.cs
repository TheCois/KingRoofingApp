using System;

namespace KRF.Core.Entities.Customer
{
    public class JobPO
    {
        public int POID { get; set; }
        public int JobID { get; set; }
        public string POCode { get; set; }
        public int VendorID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string VendorRep { get; set; }
        public int? EstimateID { get; set; }
        public decimal TotalAmount { get; set; }
        public string COIDs { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
    public class POEstimateItem
    {
        public int ID { get; set; }
        public int POID { get; set; }
        public int ItemAssemblyID { get; set; }
        public int ItemAssemblyType { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public string ItemNames { get; set; }
        public int COID { get; set; }
        public int ItemID { get; set; }
    }
}
