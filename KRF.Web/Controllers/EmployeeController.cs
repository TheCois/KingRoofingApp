using KRF.Core.DTO.Employee;
using KRF.Core.Entities.MISC;
using KRF.Core.Repository;
using KRF.Web.Models;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var employes = employeeRepo.GetEmployes();
            TempData["Employes"] = employes;
            return View();
        }
        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Add(int ID = 0)
        {
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            //var employes = employeeRepo.GetEmployes();
            ViewBag.ID = ID;
            return View();
        }

        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult MyAccount(int ID = 0)
        {
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            //var employes = employeeRepo.GetEmployes();
            ViewBag.ID = ID;
            return View("MyAccount");
        }

        /// <summary>
        /// Get Employee Detail
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetEmployes(jQueryDataTableParamModel param)
        {
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();

            EmployeeDTO employeeDTO = (EmployeeDTO)TempData["Employes"];
            if (employeeDTO == null)
                employeeDTO = employeeRepo.GetEmployes();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in employeeDTO.Employees.Where(p => p.Status == true)
                          select new string[] {
                              "<span class='edit-customer' data-val=" + p.EmpId.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.EmpId.ToString(),
                              p.FirstName + " " + p.LastName,
                              p.EmpType,
                              (p.EmpPhNo1 ?? ""),
                              (p.EmpPhNo2 ?? ""),
                              p.EmergencyCPhNo,
                              p.HourlyRate,
                              (employeeDTO.Roles.Where(r => r.RoleId == p.RoleId).FirstOrDefault() != null ? employeeDTO.Roles.Where(r => r.RoleId == p.RoleId).FirstOrDefault().RoleName : ""),
                              (p.EmailID != null && p.EmailID.ToLower() == "adminmanager" ? "" : "<span class='delete-customer' data-val=" + p.EmpId.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>")
                          }).ToArray(),
                keyValue = new
                {
                    city = employeeDTO.Cities,
                    state = employeeDTO.States,
                    territories = employeeDTO.Territories.Select(k => new { ID = k.TerritoryID, Description = k.TerritoryDesc }),
                    roles = employeeDTO.Roles.Where(p => p.Active == true).Select(k => new { ID = k.RoleId, Description = k.RoleName }),
                    skilllevels = employeeDTO.SkillLevels.Where(p => p.Active == true).Select(k => new { ID = k.SkillLevelID, Description = k.SkillLevelDesc })
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
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var employeeDTO = employeeRepo.GetEmploye(empId);
            var crews = crewRepo.GetCrews();
            return Json(new
            {
                employee = employeeDTO.Employees.First(),
                keyValue = new
                {
                    city = employeeDTO.Cities,
                    state = employeeDTO.States,
                    territories = employeeDTO.Territories.Select(k => new { ID = k.TerritoryID, Description = k.TerritoryDesc }),
                    roles = employeeDTO.Roles.Where(p => p.Active == true).Select(k => new { ID = k.RoleId, Description = k.RoleName }),
                    skilllevels = employeeDTO.SkillLevels.Where(p => p.Active == true).Select(k => new { ID = k.SkillLevelID, Description = k.SkillLevelDesc }),
                    crews = crews.Crews.Select(k => new { ID = k.CrewID, Description = k.CrewName }).OrderBy(k=>k.Description)
                },
                skillItems = employeeDTO.SkillItems.Select(k => new { SkillID = k.SkillID, SkillDesc = (k.SkillDesc ?? ""), SkillLevelID = k.SkillLevelID, SkillLevel = employeeDTO.SkillLevels.Where(s => s.SkillLevelID == k.SkillLevelID).FirstOrDefault().SkillLevelDesc, EmpId = k.EmpID }),
                crewDetails = (from p in crews.Crews
                               join q in crews.CrewDetails on p.CrewID equals q.CrewID
                               where q.EmpId == empId && p.Active == true && q.Active == true
                               select new { p.CrewID, p.CrewName, EmpId = empId, q.IsLead }
                                )
            });
        }

        /// <summary>
        /// Get Employee by UserID
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public ActionResult GetEmployeeByUserID(int userID)
        {
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var employeeDTO = employeeRepo.GetEmployeByUserID(userID);
            return Json(new
            {
                employee = employeeDTO.Employees.First(),
                keyValue = new
                {
                    city = employeeDTO.Cities,
                    state = employeeDTO.States,
                    territories = employeeDTO.Territories.Select(k => new { ID = k.TerritoryID, Description = k.TerritoryDesc }),
                    roles = employeeDTO.Roles.Where(p => p.Active == true).Select(k => new { ID = k.RoleId, Description = k.RoleName })
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
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            int ID = 0;
            try
            {
                var employee = employeeData.Employee;
                ID = employee.EmpId;
                EmployeeDTO employeeDTO = employeeRepo.GetEmployes();
                if(!string.IsNullOrEmpty(employee.Password))
                    employee.Password = KRF.Common.EncryptDecrypt.EncryptString(employee.Password);

                if (employee.EmpId == 0)
                {
                    if (!string.IsNullOrEmpty(employee.EmailID) && employeeDTO.Employees.Where(p => p.Status == true && p.EmailID == employee.EmailID).Any())
                    {
                        hasSaved = false;
                        message = "Email ID already registered. Please enter different email.";
                    }
                    else
                    {
                        employee.DateCreated = DateTime.Now;
                        employee.DateUpdated = null;
                        employee.Status = true;
                        ID = employeeRepo.Create(employee, employeeData.SkillItems, employeeData.CrewItems);
                        message = "Record successfully inserted!";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(employee.EmailID) && employeeDTO.Employees.Where(p => p.Status == true && p.EmpId != employee.EmpId && p.EmailID == employee.EmailID).Any())
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


            return Json(new { hasSaved = hasSaved, message = message, ID = ID });
        }
        /// <summary>
        /// Enable/Disbale Employee status
        /// </summary>
        /// <param name="empId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleEmployeeStatus(int empId, bool tobeEnabled)
        {

            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();

            employeeRepo.ToggleEmployeeStatus(empId, tobeEnabled);

            return Json(new { hasDeleted = true });
        }

    }
}
