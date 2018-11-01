namespace KRF.Core.Entities.ValueList
{
    public class RoofType : ValueList
    {
        /// <summary>
        /// Holds the RoofType ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the RoofType Name information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
