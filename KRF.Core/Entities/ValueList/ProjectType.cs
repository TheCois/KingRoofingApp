
namespace KRF.Core.Entities.ValueList
{
    public class ProjectType : ValueList
    {
        /// <summary>
        /// Holds the ProjectType ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the ProjectType Name information.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
