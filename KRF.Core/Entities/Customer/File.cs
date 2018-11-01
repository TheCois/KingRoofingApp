using System;

namespace KRF.Core.Entities.Customer
{
    public class File
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public DateTime UploadedOn { get; set; }

        public string Type { get; set; }

        public int CustomerID { get; set; }
    }

}
