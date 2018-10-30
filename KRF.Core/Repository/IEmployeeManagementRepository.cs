using KRF.Core.DTO.Employee;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Sales;
using System.Collections.Generic;

namespace KRF.Core.Repository
{
    public interface IEmployeeManagementRepository
    {
        /// <summary>
        /// Create an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        int Create(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems);

        /// <summary>
        /// Edit employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        Employee Edit(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems);

        /// <summary>
        /// Active/Inactive employee status
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        bool ToggleEmployeeStatus(int empId, bool tobeEnabled);

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns></returns>
        EmployeeDTO GetEmployees();

        /// <summary>
        /// Get employee by EmpID
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        EmployeeDTO GetEmployee(int empId);
        /// <summary>
        /// Get Employee By UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        EmployeeDTO GetEmployeeByUserID(int userID);
    
    }
}
