using System.Collections.Generic;
using KRF.Core.DTO.Employee;
using KRF.Core.Entities.Employee;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;

namespace KRF.Persistence.RepositoryImplementation
{
    public class EmployeeManagementRepository : IEmployeeManagementRepository
    {
        private readonly IEmployeeManagement employeeManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmployeeManagementRepository()
        {
            employeeManagement_ = ObjectFactory.GetInstance<IEmployeeManagement>();
        }
        /// <summary>
        /// Create an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        public int Create(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            return employeeManagement_.CreateEmployee(employee, skillItems, crewItems);
        }
        /// <summary>
        /// Edit employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        public Employee Edit(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            return employeeManagement_.EditEmployee(employee, skillItems, crewItems);
        }

        /// <summary>
        /// Active/Inactive employee status
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleEmployeeStatus(int empId, bool tobeEnabled)
        {
            return employeeManagement_.ToggleEmployeeStatus(empId, tobeEnabled);
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns></returns>
        public EmployeeDTO GetEmployees()
        {
            return employeeManagement_.ListAllEmployees();
        }

        /// <summary>
        /// Get employee by EmpID
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public EmployeeDTO GetEmployee(int empId)
        {
            return employeeManagement_.GetEmployeeDetails(empId);
        }
        /// <summary>
        /// Get Employee By UserID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public EmployeeDTO GetEmployeeByUserID(int userId)
        {
            return employeeManagement_.GetEmployeeByUserID(userId);
        }
    }
}
