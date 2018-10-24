
namespace KRF.Core.Entities.ValueList
{
    public class PropertyRelationship : ValueList
    {
        /// <summary>
        /// Holds the ProjectRelationship ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the ProjectRelationship Name information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
