using System;

namespace KRF.Core.Entities.Master
{
    public class Vendor
    {
        public int ID { get; set; }
        public string VendorName { get; set; }
        public string ContactName { get; set; }
        public string VendorAddress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Cell { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string WebSite { get; set; }
        public string Manager { get; set; }
        public string SalesRep { get; set; }
        public string ManagerCell { get; set; }
        public string ManagerEmail { get; set; }
        public string SalesRepCell { get; set; }
        public string SalesRepEmail { get; set; }
        public string Note { get; set; }
    }
}
