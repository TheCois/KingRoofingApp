namespace KRF.Core.Entities.Product
{
    public class Assembly
    {
        /// <summary>
        /// Assembly unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID/Code  of the assembly.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name of the assembly.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Brief description about the assembly.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Details about the work-order text.
        /// TODO - Need to understand what this means?
        /// </summary>
        public string WorkOrderText { get; set; }

        /// <summary>
        /// Item composition within the assembly.
        /// </summary>
        //public IList<ItemComposition> ItemComposition { get; set; }

        /// <summary>
        /// Mark up percentage by division details.
        /// TODO - Need to get the formula from Bill and understand the business intent.
        /// </summary>
        public decimal MarkUpPercentageByDivision { get; set; }

        /// <summary>
        /// Holds Labor hour details.
        /// </summary>
        public int LaborHour { get; set; }

        /// <summary>
        /// Holds Labor cost details.
        /// </summary>
        public decimal LaborCost { get; set; }

        /// <summary>
        /// Holds the total cost details.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Holds the total cost details.
        /// </summary>
        public decimal TotalRetailCost { get; set; }
        /// <summary>
        /// Holds the material cost details.
        /// </summary>
        public decimal MaterialCost { get; set; }
        /// <summary>
        /// Holds if assembly if Item type
        /// </summary>
        public bool IsItemAssembly { get; set; }
        public string ProposalText { get; set; }

        /// <summary>
        /// Unit of Measure of the assembly.
        /// </summary>
        public int UnitOfMeasureId { get; set; }
    }
}
