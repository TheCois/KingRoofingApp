namespace KRF.Core.Entities.Customer
{
    public class CustomerAddress
    {
        public int ID { get; set; }

        public int CustomerID { get; set; }
        
        public string Type { get; set; }

        public string AddressType { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string BillingAddress { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }



        public string Text { get; set; }

        public string Value { get; set; }

    }
}
