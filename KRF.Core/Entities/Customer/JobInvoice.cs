using System;

namespace KRF.Core.Entities.Customer
{
    public class JobInvoice
    {
        public int InvoiceID { get; set; }
        public int JobID { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Notes { get; set; }
        public int? EstimateID { get; set; }
        public string COIDs { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
    public class InvoiceItems
    {
        public int ID { get; set; }
        public int InvoiceID { get; set; }
        public int ItemAssemblyID { get; set; }
        public int ItemAssemblyType { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public int COID { get; set; }
    }
}
