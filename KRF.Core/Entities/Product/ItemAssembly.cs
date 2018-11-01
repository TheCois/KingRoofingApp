using System.Collections.Generic;

namespace KRF.Core.Entities.Product
{
    public class ItemAssembly
    {
        /// <summary>
        /// Assembly unique identifier
        /// </summary>
        public int Id { get; set; }

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
        public IList<ItemComposition> ItemComposition { get; set; }

        /// <summary>
        /// Mark up percentage by division details.
        /// TODO - Need to get the formula from Bill and understand the business intent.
        /// </summary>
        public decimal MarkUpPercentageByDivision { get; set; }

        /// <summary>
        /// Holds Labor cost details.
        /// </summary>
        public decimal LaborCost { get; set; }

        /// <summary>
        /// Holds the total cost details.
        /// </summary>
        public decimal TotalCost { get; set; }

    }
}
