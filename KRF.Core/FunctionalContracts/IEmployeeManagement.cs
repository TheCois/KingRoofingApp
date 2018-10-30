using System.Collections.Generic;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.MISC;
using KRF.Core.Entities.Employee;
using KRF.Core.DTO.Employee;

namespace KRF.Core.FunctionalContracts
{
    public interface IEmployeeManagement
    {
        /// <summary>
        /// Create employee based on filled in details
        /// </summary>
        /// <param name="employee">holds user object details</param>
        /// <returns>True - if creation successful; False - if failure</returns>
        int CreateEmployee(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems);

        /// <summary>
        /// Edit employee information based on data updated by user.
        /// </summary>
        /// <param name="employee">holds updated user object details</param>
        /// <returns>Update user object details.</returns>
        Employee EditEmployee(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems);

        /// <summary>
        /// Enables / Disabled Employee's status based on passed in employee id.
        /// </summary>
        /// <param name="empId">User identifier</param>
        /// <param name="tobeEnabled">True - If user id has to be enabled else false</param>
        /// <returns>True - if creation successful; False - if failure</returns>
        bool ToggleEmployeeStatus(int empId, bool tobeEnabled);

        /// <summary>
        /// Get details of employee based on empid
        /// </summary>
        /// <param name="empId">empid</param>
        /// <returns>employee details.</returns>
        EmployeeDTO GetEmployeeDetails(int empId);

        /// <summary>
        /// List all registered employees within the system.
        /// </summary>
        /// <returns>Employee's list</returns>
        EmployeeDTO ListAllEmployees();
        /// <summary>
        /// Get Employee By UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        EmployeeDTO GetEmployeeByUserID(int userID);
    }
}
