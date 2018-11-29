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
using System.Data;
using System.Linq;
using System.Transactions;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class EmployeeManagement : IEmployeeManagement
    {
        /// <summary>
        /// Create an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="skillItems"></param>
        /// <param name="crewItems"></param>
        /// <returns></returns>
        public int CreateEmployee(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var userId = 0;
                    if (!string.IsNullOrEmpty(employee.EmailID) && !string.IsNullOrEmpty(employee.Password))
                    {
                        var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                        predicateGroup.Predicates.Add(Predicates.Field<User>(s => s.Email, Operator.Eq, employee.EmailID));
                        IList<User> users = conn.GetList<User>(predicateGroup).ToList();

                        var user = users.Any() ? users[0] : new User();
                        user.UserName = employee.EmailID.Substring(0, (employee.EmailID.Length > 20 ? 20 : employee.EmailID.Length));
                        user.Email = employee.EmailID;
                        user.Password = employee.Password;
                        user.IsDeleted = false;
                        user.DOB = null;
                        user.IsAdmin = employee.IsAdmin;
                        if (!users.Any())
                        {
                            userId = conn.Insert(user);
                        }
                        else
                        {
                            userId = user.ID;
                            conn.Update(user);
                        }
                    }
                    employee.Password = null;
                    employee.UserID = userId;
                    var id = conn.Insert(employee);
                    if (skillItems != null)
                    {
                        foreach (var skillItem in skillItems)
                        {
                            skillItem.EmpID = id;
                            conn.Insert(skillItem);
                        }
                    }
                    if (crewItems == null)
                    {
                        crewItems = new List<EmployeeCrewDetails>();
                    }
                    UpdateCrewDetail(conn, id, crewItems);

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
        public Employee EditEmployee(Employee employee, List<tbl_EmpSkillDetails> skillItems, List<EmployeeCrewDetails> crewItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = employee.EmpId;
                    var userId = 0;
                    var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    if (!string.IsNullOrEmpty(employee.EmailID) && !string.IsNullOrEmpty(employee.Password))
                    {
                        predicateGroup.Predicates.Add(Predicates.Field<User>(s => s.Email, Operator.Eq, employee.EmailID));
                        IList<User> users = conn.GetList<User>(predicateGroup).ToList();

                        var user = users.Any() ? users[0] : new User();
                        user.UserName = employee.EmailID.Substring(0, (employee.EmailID.Length > 20 ? 20 : employee.EmailID.Length));
                        user.Email = employee.EmailID;
                        user.Password = employee.Password;
                        user.IsDeleted = false;
                        user.DOB = null;
                        user.IsAdmin = employee.IsAdmin;
                        if (!users.Any())
                        {
                            userId = conn.Insert(user);
                        }
                        else
                        {
                            userId = user.ID;
                            conn.Update(user);
                        }
                    }
                    employee.Password = null;
                    employee.UserID = userId;
                    
                    predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<tbl_EmpSkillDetails>(s => s.EmpID, Operator.Eq, id));
                    IList<tbl_EmpSkillDetails> skillDetails = conn.GetList<tbl_EmpSkillDetails>(predicateGroup).ToList();
                    foreach(var skill in skillDetails)
                    {
                        conn.Delete(skill);
                    }
                    if (skillItems != null)
                    {
                        foreach (var skill in skillItems)
                        {
                            skill.EmpID = id;
                            conn.Insert(skill);
                        }
                    }
                    if (crewItems == null)
                    {
                        crewItems = new List<EmployeeCrewDetails>();
                    }
                    UpdateCrewDetail(conn, id, crewItems);

                    conn.Update(employee);
                    transactionScope.Complete();
                    return employee;
                }
            }
        }
        /// <summary>
        /// Update Crew Detail
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="id"></param>
        /// <param name="crewItems"></param>
        private void UpdateCrewDetail(IDbConnection conn, int id, List<EmployeeCrewDetails> crewItems)
        {
            var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.EmpId, Operator.Eq, id));
            var allCrewDetails = conn.GetList<CrewDetail>(predicateGroup).ToList();
            foreach (var crewDetailToBeDelete in allCrewDetails)
            {
                if (crewItems.Find(c => c.CrewID == crewDetailToBeDelete.CrewID) == null)
                {
                    conn.Delete(crewDetailToBeDelete);
                }
            }
            foreach (var crewItem in crewItems)
            {
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, crewItem.CrewID));
                var crewDetails = conn.GetList<CrewDetail>(predicateGroup).ToList();

                if (crewDetails.Any())
                {
                    if (crewItem.IsLead)
                    {
                        foreach (var crewDetail in crewDetails)
                        {
                            crewDetail.IsLead = false;
                            conn.Update(crewDetail);
                        }
                    }
                    predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                    predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq, crewItem.CrewID));
                    predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.EmpId, Operator.Eq, id));
                    var filteredDetails = conn.GetList<CrewDetail>(predicateGroup).ToList();
                    if (filteredDetails.Count > 0)
                    {
                        filteredDetails[0].IsLead = crewItem.IsLead;
                        conn.Update(filteredDetails[0]);
                    }
                    else
                    {
                        var crewDetailInsert = new CrewDetail
                        {
                            CrewID = crewItem.CrewID, EmpId = id, IsLead = crewItem.IsLead, Active = true
                        };
                        conn.Insert(crewDetailInsert);
                    }
                }
                else
                {
                    var crewDetailInsert = new CrewDetail
                    {
                        CrewID = crewItem.CrewID, EmpId = id, IsLead = crewItem.IsLead, Active = true
                    };
                    conn.Insert(crewDetailInsert);
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.EmpId, Operator.Eq, empId));
                var employee = conn.Get<Employee>(empId);
                employee.Status = tobeEnabled;
                employee.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(employee);
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.EmpId, Operator.Eq, empId));
                var employee = conn.Get<Employee>(empId);
                if (employee != null && (employee.UserID ?? 0) > 0)
                {
                    var user = conn.Get<User>(employee.UserID);
                    employee.Password = Common.EncryptDecrypt.DecryptString(user.Password);
                }
                IList<Employee> p = new List<Employee>();
                p.Add(employee);
                IList<tbl_EmpTerritory> territories = conn.GetList<tbl_EmpTerritory>().ToList();
                IList<Role> roles = conn.GetList<Role>().ToList();
                IList<tbl_SkillLevel> skillLevels = conn.GetList<tbl_SkillLevel>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<tbl_EmpSkillDetails>(s => s.EmpID, Operator.Eq, empId));
                IList<tbl_EmpSkillDetails> skillItems = conn.GetList<tbl_EmpSkillDetails>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = conn.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = conn.GetList<State>(predicateGroup).ToList();

                return new EmployeeDTO
                {
                    Cities = cities.OrderBy(t => t.ID).ToList(),
                    States = states.OrderBy(t => t.Description).ToList(),
                    Employees = p,
                    Territories = territories.Where(t => t.Active).OrderBy(q => q.TerritoryDesc).ToList(),
                    Roles = roles.OrderBy(q => q.RoleName).ToList(),
                    SkillLevels = skillLevels.OrderBy(q => q.SkillLevelDesc).ToList(),
                    SkillItems = skillItems.OrderBy(q => q.SkillDesc).ToList()
                };
            }
        }

        /// <summary>
        /// Get Employee By UserID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public EmployeeDTO GetEmployeeByUserID(int userId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.UserID, Operator.Eq, userId));
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.Status, Operator.Eq, true));
                IList<Employee> employees = conn.GetList<Employee>(predicateGroup).ToList();
                IList<Employee> p = new List<Employee>();
                p.Add(employees[0]);
                IList<Role> roles = conn.GetList<Role>().ToList();
                IList<tbl_EmpTerritory> territories = conn.GetList<tbl_EmpTerritory>().ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = conn.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = conn.GetList<State>(predicateGroup).ToList();
                return new EmployeeDTO
                {
                    Cities = cities.OrderBy(t => t.ID).ToList(),
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
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.EmpId, Operator.Eq, empId));
                IList<tbl_EmpTerritory> territories = conn.GetList<tbl_EmpTerritory>().ToList();
                IList<Role> roles = conn.GetList<Role>().ToList();
                IList<tbl_SkillLevel> skillLevels = conn.GetList<tbl_SkillLevel>().ToList();
                IList<Employee> employees = conn.GetList<Employee>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<City>(s => s.Active, Operator.Eq, true));
                IList<City> cities = conn.GetList<City>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = conn.GetList<State>(predicateGroup).ToList();
                return new EmployeeDTO
                {
                    Cities = cities.OrderBy(p => p.ID).ToList(),
                    States = states.OrderBy(p => p.Description).ToList(),
                    Employees = employees,
                    Territories = territories.Where(t => t.Active).OrderBy(q => q.TerritoryDesc).ToList(),
                    Roles = roles.OrderBy(q => q.RoleName).ToList(),
                    SkillLevels = skillLevels.OrderBy(q => q.SkillLevelDesc).ToList()
                };
            }
        }
    }
}
