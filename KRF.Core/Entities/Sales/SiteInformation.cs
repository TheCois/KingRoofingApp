using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.MISC;
using KRF.Core.Entities.ValueList;

namespace KRF.Core.Entities.Sales
{
    public class SiteInformation
    {
        /// <summary>
        /// Holds Site information ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Holds Site's Sub division details.
        /// </summary>
        public Division Division { get; set; }

        /// <summary>
        /// Holds site contact information.
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// Holds the Job type information.
        /// </summary>
        public JobType JobType { get; set; }

        /// <summary>
        /// Roof type information.
        /// </summary>
        public RoofType RoofType { get; set; }

        /// <summary>
        /// Roof age information
        /// </summary>
        public RoofAge RoofAge { get; set; }

        /// <summary>
        /// Holds the total number of stories of the property.
        /// </summary>
        public int NumberOfStories { get; set; }

        /// <summary>
        /// Holds Project start timeline details.
        /// </summary>
        public ProjectStartTimeline ProjectStartTimeline { get; set; }

        /// <summary>
        /// Holds any additional notes about the property.
        /// </summary>
        public string Notes { get; set; }
    }
}
