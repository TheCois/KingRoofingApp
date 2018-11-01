namespace KRF.Core.Entities.Customer
{
    public class BuildingInformation
    {
        public int ID { get; set; }

        public string RoofType { get; set; }

        public string Category { get; set; }

        public string RoofPitch { get; set; }

        public string Style { get; set; }

        public string Color { get; set; }

        public int Stories { get; set; }

        public int RoofingAge { get; set; }

        public decimal SquareFeet { get; set; }

        public string Notes { get; set; }

        public int LeadId { get; set; }
    }
}
