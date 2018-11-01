using KRF.Core.Entities.ValueList;
using System.Collections.Generic;
namespace KRF.Core.DTO.Employee
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class EmployeeDTO
    {
        public IList<KRF.Core.Entities.Employee.Employee> Employees { get; set; }
        /// <summary>
        /// Holds the Item List
        /// </summary>
        public IList<KRF.Core.Entities.AccessControl.Role> Roles { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<State> States { get; set; }

        /// <summary>
        /// Holds State List
        /// </summary>
        public IList<City> Cities { get; set; }

        /// <summary>
        /// Holds the Territory List
        /// </summary>
        public IList<tbl_EmpTerritory> Territories { get; set; }
        public IList<tbl_SkillLevel> SkillLevels { get; set; }

        public IList<KRF.Core.Entities.Employee.tbl_EmpSkillDetails> SkillItems { get; set; }

    }
}
