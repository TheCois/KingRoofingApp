
namespace KRF.Core.Entities.ValueList
{
    public class ContactMethod : ValueList
    {
        /// <summary>
        /// Holds the Status ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the Status Name information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
