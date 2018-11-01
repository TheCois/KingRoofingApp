namespace KRF.Core.Entities.ValueList
{
    public class City : ValueList
    {
        /// <summary>
        /// Holds the Color ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the Color Name information.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Holds the Active information.
        /// </summary>
        public string Active { get; set; }
    }
}
