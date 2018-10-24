using System;

namespace KRF.Core.Entities.Employee
{
    public class Employee
    {
        public int EmpId { get; set; }

        /// <summary>
        /// Holds the First Name information of the Contact
        /// </summary>
        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        /// <summary>
        /// Holds the Last Name information of the Contact
        /// </summary>
        public string LastName { get; set; }
        public string Title { get; set; }
        public string EmpType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int City { get; set; }
        public int State { get; set; }
        public string ZipCode { get; set; }
        public string EmpPhNo1 { get; set; }
        public string EmpPhNo2 { get; set; }
        public string EmpPhNo3 { get; set; }
        public int TerritoryID { get; set; }
        public string EmergencyCName { get; set; }
        public string EmergencyCPhNo { get; set; }
        public string EmergencyCPhNo2 { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailID { get; set; }
        public string DLNo { get; set; }
        public string HourlyRate { get; set; }
        public int RoleId { get; set; }
        public System.Nullable<DateTime> DateCreated { get; set; }
        public System.Nullable<DateTime> DateUpdated { get; set; }
        public int? UserID { get; set; }
        public bool? AppAccess { get; set; }
        public bool? Status { get; set; }
        public bool IsAdmin { get; set; }

    }

    public class tbl_EmpSkillDetails
    {
        public int SkillID { get; set; }
        public string SkillDesc { get; set; }
        public int SkillLevelID { get; set; }
        public int EmpID { get; set; }

    }

    public class EmployeeCrewDetails
    {
        public int CrewID { get; set; }
        public int EmpId { get; set; }
        public bool IsLead { get; set; }
    }
}
