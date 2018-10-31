using System;

namespace KRF.Core.Entities.Master
{
    public class Equipment
    {
        public int ID { get; set; }
        public string SNNo { get; set; }
        public string ModelNumber { get; set; }
        public string EquipmentName { get; set; }
        public int EquipmentID { get; set; }
        //public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public string PurchaseLocation { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? EquipmentStatusID { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Vendor { get; set; }
    }
}
