
namespace KRF.Core.Entities.ValueList
{
    public class State : ValueList
    {
        /// <summary>
        /// Holds the State ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the State Name information.
        /// </summary>
        public string Description { get; set; }

        public string Abbreviation { get; set; }
        public bool Active { get; set; }
    }
}
