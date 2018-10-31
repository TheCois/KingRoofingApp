using KRF.Core.DTO.Employee;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class EmployeeController : BaseController
    {
        //
        // GET: /Employee/

        public ActionResult Index()
        {
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var employees = employeeRepo.GetEmployees();
            TempData["Employes"] = employees;
            return View();
        }
        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Add(int id = 0)
        {
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            ViewBag.ID = id;
            return View();
        }

        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult MyAccount(int ID = 0)
        {
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            //var employes = employeeRepo.GetEmployes();
            ViewBag.ID = ID;
            return View("MyAccount");
        }

        /// <summary>
        /// Get Employee Detail
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetEmployees(jQueryDataTableParamModel param)
        {
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();

            var employeeDto = (EmployeeDTO)TempData["Employes"] ?? employeeRepo.GetEmployees();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in employeeDto.Employees.Where(p => p.Status == true)
                    let firstEmployeeWithTheRole = employeeDto.Roles.FirstOrDefault(r => r.RoleId == p.RoleId)
                    select new string[] {
                              "<span class='edit-customer' data-val=" + p.EmpId.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.EmpId.ToString(),
                              p.FirstName + " " + p.LastName,
                              p.EmpType,
                              (p.EmpPhNo1 ?? ""),
                              (p.EmpPhNo2 ?? ""),
                              p.EmergencyCPhNo,
                              p.HourlyRate,
                              (firstEmployeeWithTheRole != null ? firstEmployeeWithTheRole.RoleName : ""),
                              (p.EmailID != null && p.EmailID.ToLower() == "adminmanager" ? "" : "<span class='delete-customer' data-val=" + p.EmpId.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>")
                          }).ToArray(),
                keyValue = new
                {
                    city = employeeDto.Cities,
                    state = employeeDto.States,
                    territories = employeeDto.Territories.Select(k => new { ID = k.TerritoryID, Description = k.TerritoryDesc }),
                    roles = employeeDto.Roles.Where(p => p.Active).Select(k => new { ID = k.RoleId, Description = k.RoleName }),
                    skilllevels = employeeDto.SkillLevels.Where(p => p.Active).Select(k => new { ID = k.SkillLevelID, Description = k.SkillLevelDesc })
                }
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Employee by EmpID
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public ActionResult GetEmployee(int empId)
        {
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var employeeDto = employeeRepo.GetEmployee(empId);
            var crews = crewRepo.GetCrews();
            return Json(new
            {
                employee = employeeDto.Employees.First(),
                keyValue = new
                {
                    city = employeeDto.Cities,
                    state = employeeDto.States,
                    territories =
                        employeeDto.Territories.Select(k => new {ID = k.TerritoryID, Description = k.TerritoryDesc}),
                    roles = employeeDto.Roles.Where(p => p.Active)
                        .Select(k => new {ID = k.RoleId, Description = k.RoleName}),
                    skilllevels = employeeDto.SkillLevels.Where(p => p.Active)
                        .Select(k => new {ID = k.SkillLevelID, Description = k.SkillLevelDesc}),
                    crews = crews.Crews.Select(k => new {ID = k.CrewID, Description = k.CrewName})
                        .OrderBy(k => k.Description)
                },
                skillItems = employeeDto.SkillItems.Select(k =>
                {
                    var relevantSkillLevel =
                        employeeDto.SkillLevels.FirstOrDefault(s => s.SkillLevelID == k.SkillLevelID);
                    return new
                    {
                        k.SkillID,
                        SkillDesc = (k.SkillDesc ?? ""), k.SkillLevelID,
                        SkillLevel = relevantSkillLevel.SkillLevelDesc,
                        EmpId = k.EmpID
                    };
                }),
                crewDetails = (from p in crews.Crews
                        join q in crews.CrewDetails on p.CrewID equals q.CrewID
                        where q.EmpId == empId && p.Active && q.Active
                        select new {p.CrewID, p.CrewName, EmpId = empId, q.IsLead}
                    )
            });
        }

        /// <summary>
        /// Get Employee by UserID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetEmployeeByUserId(int userId)
        {
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var employeeDto = employeeRepo.GetEmployeeByUserID(userId);
            return Json(new
            {
                employee = employeeDto.Employees.First(),
                keyValue = new
                {
                    city = employeeDto.Cities,
                    state = employeeDto.States,
                    territories = employeeDto.Territories.Select(k => new { ID = k.TerritoryID, Description = k.TerritoryDesc }),
                    roles = employeeDto.Roles.Where(p => p.Active).Select(k => new { ID = k.RoleId, Description = k.RoleName })
                }
            });
        }

        /// <summary>
        /// Save Employee
        /// </summary>
        /// <param name="employeeData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(EmployeeData employeeData)
        {
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var hasSaved = true;
            string message;
            var id = 0;
            try
            {
                var employee = employeeData.Employee;
                id = employee.EmpId;
                var employeeDto = employeeRepo.GetEmployees();
                if(!string.IsNullOrEmpty(employee.Password))
                    employee.Password = KRF.Common.EncryptDecrypt.EncryptString(employee.Password);

                if (employee.EmpId == 0)
                {
                    if (!string.IsNullOrEmpty(employee.EmailID) && employeeDto.Employees.Where(p => p.Status == true && p.EmailID == employee.EmailID).Any())
                    {
                        hasSaved = false;
                        message = "Email ID already registered. Please enter different email.";
                    }
                    else
                    {
                        employee.DateCreated = DateTime.Now;
                        employee.DateUpdated = null;
                        employee.Status = true;
                        id = employeeRepo.Create(employee, employeeData.SkillItems, employeeData.CrewItems);
                        message = "Record successfully inserted!";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(employee.EmailID) && employeeDto.Employees.Where(p => p.Status == true && p.EmpId != employee.EmpId && p.EmailID == employee.EmailID).Any())
                    {
                        hasSaved = false;
                        message = "Email ID already registered. Please enter different email.";
                    }
                    else
                    {
                        employee.DateUpdated = DateTime.Now;
                        employeeRepo.Edit(employee, employeeData.SkillItems, employeeData.CrewItems);
                        message = "Record successfully updated!";
                    }
                }
                hasSaved = true;
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Employee data could not be updated.";
            }


            return Json(new { hasSaved = hasSaved, message = message, ID = id });
        }
        /// <summary>
        /// Enable/Disable Employee status
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleEmployeeStatus(int empId, bool tobeEnabled)
        {

            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();

            employeeRepo.ToggleEmployeeStatus(empId, tobeEnabled);

            return Json(new { hasDeleted = true });
        }

    }
}
