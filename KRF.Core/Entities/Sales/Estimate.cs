using System;

namespace KRF.Core.Entities.Sales
{
    public class Estimate
    {
        /// <summary>
        /// Estimate unique identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Name of the estimate
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Status of the estimate
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Lead or Customer associated with estimate
        /// </summary>
        public int LeadID { get; set; }

        /// <summary>
        /// Address associated with estimate
        /// </summary>
        public int JobAddressID { get; set; }

        /// <summary>
        /// Estimate total cost
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Is Estimate signed (holds True / False)
        /// </summary>
        public decimal PriceAdj { get; set; }

        /// <summary>
        /// If signed, holds date and time.
        /// </summary>
        public string ReasonForAdj { get; set; }

        /// <summary>
        /// Contract price agreed by the customer
        /// </summary>
        public decimal ContractPrice { get; set; }

        /// <summary>
        /// Date when Estimate was created
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Estimate total material cost
        /// </summary>
        public decimal TotalMaterialCost { get; set; }
        /// <summary>
        /// Estimate total labor cost
        /// </summary>
        public decimal TotalLaborCost { get; set; }

        public int RoofType { get; set; }

        /// <summary>
        /// Holds Note
        /// </summary>
        public string Note { get; set; }
    }

    public class EstimateItem
    {
        /// <summary>
        /// EstimateItem ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Estimate ID
        /// </summary>
        public int EstimateID { get; set; }
        
        /// <summary>
        /// ItemID or Assembly ID
        /// </summary>
        public int ItemAssemblyID { get; set; }
        
        /// <summary>
        /// Whether Item or Assembly
        /// </summary>
        public int ItemAssemblyType { get; set; }
        
        /// <summary>
        /// Price of Item Or Assembly set in Product
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Quantity entered by the user
        /// </summary>
        public decimal Quantity { get; set; }
        
        /// <summary>
        /// Cost = Price * Quantity
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// Assebly total labour cost
        /// </summary>
        public decimal LaborCost { get; set; }
        /// <summary>
        /// Assebly total material cost
        /// </summary>
        public decimal MaterialCost { get; set; }
        /// <summary>
        /// Assebly UOMs
        /// </summary>
        public string UOMs { get; set; }
    }

    public class EstimateDocument
    {
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Estimate ID from Estimate class
        /// </summary>
        public int EstimateID { get; set; }

        /// <summary>
        /// Documet Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Document type (e.g pdf, jpg etc)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Description of the document
        /// </summary>
        public string Description { get; set; }

        
        /// <summary>
        /// File content in byte array
        /// </summary>
        public byte[] Text { get; set; }

        /// <summary>
        /// Upload DateTime of the document
        /// </summary>
        public DateTime? UploadDateTime { get; set; }
    }
}
