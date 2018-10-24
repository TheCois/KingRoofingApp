using KRF.Core.Entities.Product;
using KRF.Core.Entities.Sales;
using KRF.Core.Entities.ValueList;
using System.Collections.Generic;

namespace KRF.Core.DTO.Sales
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class EstimateDTO
    {
        /// <summary>
        /// Holds the esitmate List
        /// </summary>
        public IList<Estimate> Estimates { get; set; }

        /// <summary>
        /// Holds estimate items List
        /// </summary>
        public IList<EstimateItem> EstimateItems { get; set; }

        /// <summary>
        /// Required for Estimate lead lookup
        /// </summary>
        public IList<Lead> Leads { get; set; }

        /// <summary>
        /// Required for Estimate customer lookup
        /// </summary>
        public IList<Lead> Customers { get; set; }

        /// <summary>
        /// Required for Estimate customer address lookup
        /// </summary>
        public IList<CustomerAddress> CustomerAddress { get; set; }


        /// <summary>
        /// Required for Estimate status lookup
        /// </summary>
        public IList<Status> Status { get; set; }

        /// <summary>
        /// Holds RoofType List
        /// </summary>
        public IList<RoofType> RoofTypes { get; set; }

        /// <summary>
        /// Required for EstimateItem for Item lookuup
        /// </summary>
        public IList<Item> Items { get; set; }

        /// <summary>
        /// Required for EstimateItem for Assebly lookyup
        /// </summary>
        public IList<Assembly> Assemblies { get; set; }

        /// <summary>
        /// Unit of measure 
        /// </summary>
        public IList<UnitOfMeasure> UnitOfMeasure { get; set; }

        public IList<EstimateDocument> EstimateDocuments { get; set; }
        public IList<AssemblyItem> AssemblyItems { get; set; }
    }
}
