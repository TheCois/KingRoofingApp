namespace KRF.Core.Entities.Product
{
    public class ItemComposition
    {
        /// <summary>
        /// Holds item detail
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Item quantity to be included in assembly.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// % of item to be included in assembly.
        /// </summary>
        public decimal PercentageOfItem { get; set; }
    }
}
