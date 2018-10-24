using KRF.Core.DTO.Employee;
using KRF.Core.Entities.Employee;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class EmployeeManagementRepository : IEmployeeManagementRepository
    {
        private readonly IEmployeeManagement _EmployeeManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmployeeManagementRepository()
        {
            _EmployeeManagement = ObjectFactory.GetInstance<IEmployeeManagement>();
        }
        /// <summary>
        /// Create an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        public int Create(Employee Employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            return _EmployeeManagement.CreateEmployee(Employee, skillItems, crewItems);
        }
        /// <summary>
        /// Edit employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        public Employee Edit(Employee Employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            return _EmployeeManagement.EditEmployee(Employee, skillItems, crewItems);
        }

        /// <summary>
        /// Active/Inactive employee status
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleEmployeeStatus(int empId, bool tobeEnabled)
        {
            return _EmployeeManagement.ToggleEmployeeStatus(empId, tobeEnabled);
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns></returns>
        public EmployeeDTO GetEmployes()
        {
            return _EmployeeManagement.ListAllEmployees();
        }

        /// <summary>
        /// Get employee by EmpID
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public EmployeeDTO GetEmploye(int empId)
        {
            return _EmployeeManagement.GetEmployeeDetails(empId);
        }
        /// <summary>
        /// Get Employee By UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public EmployeeDTO GetEmployeByUserID(int userID)
        {
            return _EmployeeManagement.GetEmployeByUserID(userID);
        }
    }
}
