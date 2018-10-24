using DapperExtensions;
using KRF.Core.DTO.Employee;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.Actors;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Master;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class EmployeeManagement : IEmployeeManagement
    {
        private string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmployeeManagement()
        {
            _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
        }

        /// <summary>
        /// Create an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        public int CreateEmployee(Employee empoyee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    int userID = 0;
                    if (!string.IsNullOrEmpty(empoyee.EmailID) && !string.IsNullOrEmpty(empoyee.Password))
                    {
                        var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                        predicateGroup.Predicates.Add(Predicates.Field<User>(s => s.Email, Operator.Eq, empoyee.EmailID));
                        IList<User> users = sqlConnection.GetList<User>(predicateGroup).ToList();

                        User user = users.Any() ? users[0] : new User();
                        user.UserName = empoyee.EmailID.Substring(0, (empoyee.EmailID.Length > 20 ? 20 : empoyee.EmailID.Length));
                        user.Email = empoyee.EmailID;
                        user.Password = empoyee.Password;
                        user.IsDeleted = false;
                        user.DOB = null;
                        user.IsAdmin = empoyee.IsAdmin;
                        if (!users.Any())
                        {
                            userID = sqlConnection.Insert<User>(user);
                        }
                        else
                        {
                            userID = user.ID;
                            sqlConnection.Update<User>(user);
                        }
                    }
                    empoyee.Password = null;
                    empoyee.UserID = userID;
                    var id = sqlConnection.Insert<Employee>(empoyee);
                    if (skillItems != null)
                    {
                        foreach (tbl_EmpSkillDetails skillItem in skillItems)
                        {
                            skillItem.EmpID = id;
                            sqlConnection.Insert<tbl_EmpSkillDetails>(skillItem);
                        }
                    }
                    if (crewItems == null)
                    {
                        crewItems = new List<EmployeeCrewDetails>();
                    }
                    UpdateCrewDetail(sqlConnection, id, crewItems);

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        public Employee EditEmployee(Employee empoyee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();
                    int id = empoyee.EmpId;
                    int userID = 0;
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    if (!string.IsNullOrEmpty(empoyee.EmailID) && !string.IsNullOrEmpty(empoyee.Password))
                    {
                        predicateGroup.Predicates.Add(Predicates.Field<User>(s => s.Email, Operator.Eq, empoyee.EmailID));
                        IList<User> users = sqlConnection.GetList<User>(predicateGroup).ToList();

                        User user = users.Any() ? users[0] : new User();
                        user.UserName = empoyee.EmailID.Substring(0, (empoyee.EmailID.Length > 20 ? 20 : empoyee.EmailID.Length));
                        user.Email = empoyee.EmailID;
                        user.Password = empoyee.Password;
                        user.IsDeleted = false;
                        user.DOB = null;
                        user.IsAdmin = empoyee.IsAdmin;
                        if (!users.Any())
                        {
                            userID = sqlConnection.Insert<User>(user);
                        }
                        else
                        {
                            userID = user.ID;
                            sqlConnection.Update<User>(user);
                        }
                    }
                    empoyee.Password = null;
                    empoyee.UserID = userID;
                    
                    predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<tbl_EmpSkillDetails>(s => s.EmpID, Operator.Eq, id));
                    IList<tbl_EmpSkillDetails> skillDetails = sqlConnection.GetList<tbl_EmpSkillDetails>(predicateGroup).ToList();
                    foreach(tbl_EmpSkillDetails skill in skillDetails)
                    {
                        sqlConnection.Delete(skill);
                    }
                    if (skillItems != null)
                    {
                        foreach (tbl_EmpSkillDetails skill in skillItems)
                        {
                            skill.EmpID = id;
                            sqlConnection.Insert<tbl_EmpSkillDetails>(skill);
                        }
                    }
                    if (crewItems == null)
                    {
                        crewItems = new List<EmployeeCrewDetails>();
                    }
                    UpdateCrewDetail(sqlConnection, id, crewItems);

                    var isEdited = sqlConnection.Update<Employee>(empoyee);
                    transactionScope.Complete();
                    return empoyee;
                }
            }
        }
        /// <summary>
        /// Update Crew Detail
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="id"></param>
        /// <param name="crewItems"></param>
        private void UpdateCrewDetail(SqlConnection sqlConnection, int id, List<EmployeeCrewDetails> crewItems)
        {
            var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.EmpId, Operator.Eq, id));
            List<CrewDetail> allCrewDetails = sqlConnection.GetList<CrewDetail>(predicateGroup).ToList();
            foreach (CrewDetail crewDetailToBeDelete in allCrewDetails)
            {
                if (crewItems.Find(c => c.CrewID == crewDetailToBeDelete.CrewID) == null)
                {
                    sqlConnection.Delete<CrewDetail>(crewDetailToBeDelete);
                }
            }
            foreach (EmployeeCrewDetails crewItem in crewItems)
            {
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, crewItem.CrewID));
                List<CrewDetail> crewDetails = sqlConnection.GetList<CrewDetail>(predicateGroup).ToList();

                if (crewDetails.Count() > 0)
                {
                    if (crewItem.IsLead)
                    {
                        foreach (CrewDetail crewDetail in crewDetails)
                        {
                            crewDetail.IsLead = false;
                            sqlConnection.Update<CrewDetail>(crewDetail);
                        }
                    }
                    predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, crewItem.CrewID));
                    predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.EmpId, Operator.Eq, id));
                    List<CrewDetail> crewDetl = sqlConnection.GetList<CrewDetail>(predicateGroup).ToList();
                    if (crewDetl.Count > 0)
                    {
                        crewDetl[0].IsLead = crewItem.IsLead;
                        sqlConnection.Update<CrewDetail>(crewDetl[0]);
                    }
                    else
                    {
                        CrewDetail crewDetailInsert = new CrewDetail();
                        crewDetailInsert.CrewID = crewItem.CrewID;
                        crewDetailInsert.EmpId = id;
                        crewDetailInsert.IsLead = crewItem.IsLead;
                        crewDetailInsert.Active = true;
                        sqlConnection.Insert<CrewDetail>(crewDetailInsert);
                    }
                }
                else
                {
                    CrewDetail crewDetailInsert = new CrewDetail();
                    crewDetailInsert.CrewID = crewItem.CrewID;
                    crewDetailInsert.EmpId = id;
                    crewDetailInsert.IsLead = crewItem.IsLead;
                    crewDetailInsert.Active = true;
                    sqlConnection.Insert<CrewDetail>(crewDetailInsert);
                }
            }
        }

        /// <summary>
        /// Active/Inactive employee status
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleEmployeeStatus(int empId, bool tobeEnabled)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.EmpId, Operator.Eq, empId));
                sqlConnection.Open();
                Employee Employee = sqlConnection.Get<Employee>(empId);
                Employee.Status = tobeEnabled;
                Employee.DateUpdated = DateTime.Now;
                var isUpdated = sqlConnection.Update<Employee>(Employee);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get employee detail by EmpID
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public EmployeeDTO GetEmployeeDetails(int empId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.EmpId, Operator.Eq, empId));
                sqlConnection.Open();
                Employee employee = sqlConnection.Get<Employee>(empId);
                if (employee != null && (employee.UserID ?? 0) > 0)
                {
                    User user = sqlConnection.Get<User>(employee.UserID);
                    employee.Password = Common.EncryptDecrypt.DecryptString(user.Password);
                }
                IList<Employee> p = new List<Employee>();
                p.Add(employee);
                IList<tbl_EmpTerritory> territories = sqlConnection.GetList<tbl_EmpTerritory>().ToList();
                IList<Role> roles = sqlConnection.GetList<Role>().ToList();
                IList<tbl_SkillLevel> skilllevels = sqlConnection.GetList<tbl_SkillLevel>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<tbl_EmpSkillDetails>(s => s.EmpID, Operator.Eq, empId));
                IList<tbl_EmpSkillDetails> skillItems = sqlConnection.GetList<tbl_EmpSkillDetails>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = sqlConnection.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = sqlConnection.GetList<State>(predicateGroup).ToList();

                return new EmployeeDTO
                {
                    Cities = cities.OrderBy(t => t.Description).ToList(),
                    States = states.OrderBy(t => t.Description).ToList(),
                    Employees = p,
                    Territories = territories.Where(t => t.Active).OrderBy(q => q.TerritoryDesc).ToList(),
                    Roles = roles.OrderBy(q => q.RoleName).ToList(),
                    SkillLevels = skilllevels.OrderBy(q => q.SkillLevelDesc).ToList(),
                    SkillItems = skillItems.OrderBy(q => q.SkillDesc).ToList()
                }; ;
            }
        }

        /// <summary>
        /// Get Employee By UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public EmployeeDTO GetEmployeByUserID(int userID)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.UserID, Operator.Eq, userID));
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.Status, Operator.Eq, true));
                sqlConnection.Open();
                IList<Employee> employees = sqlConnection.GetList<Employee>(predicateGroup).ToList();
                IList<Employee> p = new List<Employee>();
                p.Add(employees[0]);
                IList<Role> roles = sqlConnection.GetList<Role>().ToList();
                IList<tbl_EmpTerritory> territories = sqlConnection.GetList<tbl_EmpTerritory>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = sqlConnection.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = sqlConnection.GetList<State>(predicateGroup).ToList();
                return new EmployeeDTO
                {
                    Cities = cities.OrderBy(t => t.Description).ToList(),
                    States = states.OrderBy(t => t.Description).ToList(),
                    Employees = p,
                    Territories = territories.Where(t => t.Active).OrderBy(q => q.TerritoryDesc).ToList(),
                    Roles = roles.OrderBy(q => q.RoleName).ToList()
                };
            }
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns></returns>
        public EmployeeDTO ListAllEmployees()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.EmpId, Operator.Eq, empId));
                sqlConnection.Open();
                IList<tbl_EmpTerritory> territories = sqlConnection.GetList<tbl_EmpTerritory>().ToList();
                IList<Role> roles = sqlConnection.GetList<Role>().ToList();
                IList<tbl_SkillLevel> skilllevels = sqlConnection.GetList<tbl_SkillLevel>().ToList();
                IList<Employee> employees = sqlConnection.GetList<Employee>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = sqlConnection.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = sqlConnection.GetList<State>(predicateGroup).ToList();
                return new EmployeeDTO
                {
                    Cities = cities.OrderBy(p => p.Description).ToList(),
                    States = states.OrderBy(p => p.Description).ToList(),
                    Employees = employees,
                    Territories = territories.Where(t => t.Active).OrderBy(q => q.TerritoryDesc).ToList(),
                    Roles = roles.OrderBy(q => q.RoleName).ToList(),
                    SkillLevels = skilllevels.OrderBy(q => q.SkillLevelDesc).ToList()
                };
            }
        }
    }
}
