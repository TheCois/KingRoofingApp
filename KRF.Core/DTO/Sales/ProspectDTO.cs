using KRF.Core.Entities.Sales;
using KRF.Core.Entities.ValueList;
using System.Collections.Generic;

namespace KRF.Core.DTO.Sales
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class ProspectDTO
    {
        /// <summary>
        /// Holds the Item List
        /// </summary>
        public IList<Prospect> Propects { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<City> Cities { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<State> States { get; set; }

        // <summary>
        /// Holds Country List
        /// </summary>
        public IList<Country> Countries { get; set; }

        /// <summary>
        /// Holds Status List
        /// </summary>
        public IList<Status> Statuses { get; set; }

    }
}
