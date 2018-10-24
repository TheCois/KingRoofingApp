using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Sales;
using KRF.Core.Entities.ValueList;
using System.Collections.Generic;
namespace KRF.Core.DTO.Sales
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class LeadDTO
    {
        /// <summary>
        /// Holds the Item List
        /// </summary>
        public IList<Lead> Leads { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<State> States { get; set; }

        /// <summary>
        /// Holds city List
        /// </summary>
        public IList<City> Cities { get; set; }

        /// <summary>
        /// Holds country List
        /// </summary>
        public IList<Country> Countries { get; set; }

        /// <summary>
        /// Holds Status List
        /// </summary>
        public IList<Status> Statuses { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<ContactMethod> ContactMethod { get; set; }

        /// <summary>
        /// Holds Status List
        /// </summary>
        public IList<HearAboutUs> HearAboutUsList { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<PropertyRelationship> PropertyRelationship { get; set; }

        /// <summary>
        /// Holds Status List
        /// </summary>
        public IList<ProjectStartTimeline> ProjectStartTimelines { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<ProjectType> ProjectTypes { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<RoofAge> RoofAgeList { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<NumberOfStories> BuildingStoriesList { get; set; }

        /// <summary>
        /// Holds RoofType List
        /// </summary>
        public IList<RoofType> RoofTypes { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<KRF.Core.Entities.Employee.Employee> Employees { get; set; }
        /// <summary>
        /// Holds ExistingRoof List
        /// </summary>
        public IList<ExistingRoof> ExistingRoofs { get; set; }

        /// <summary>
        /// Holds Customer Address List
        /// </summary>
        public IList<CustomerAddress> CustomerAddress { get; set; }
    }
}
