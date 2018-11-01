namespace KRF.Core.Entities.ValueList
{
    public class ExistingRoof : ValueList
    {
        /// <summary>
        /// Holds the ExistingRoof ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the ExistingRoof Name information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
