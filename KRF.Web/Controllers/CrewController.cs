using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class CrewController : BaseController
    {
        //
        // GET: /Crew/

        public ActionResult Index()
        {
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var crews = crewRepo.GetCrews();
            TempData["Crews"] = crews;
            return View();
        }

        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Add(int id = 0)
        {
            //ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            //var crews = crewRepo.GetCrew();
            ViewBag.ID = id;
            return View();
        }
        /// <summary>
        /// Get Crew Detail
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetCrews(jQueryDataTableParamModel param)
        {
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            
            var crewDto = (CrewDTO)TempData["Crews"] ?? crewRepo.GetCrews();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in crewDto.Crews.Where(p => p.Active)
                    select new[]
                    {
                        "<span class='edit-customer' data-val=" + p.CrewID +
                        "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                        p.CrewName,
                        crewDto.CrewDetails.Where(r => r.CrewID == p.CrewID && r.Active).ToList().Count.ToString(),
                        (crewDto.CrewDetails.FirstOrDefault(r => r.CrewID == p.CrewID && r.IsLead) != null
                            ? (crewDto.Employees.FirstOrDefault(e =>
                                   e.EmpId == (crewDto.CrewDetails.FirstOrDefault(r => r.CrewID == p.CrewID && r.IsLead)
                                       .EmpId)).FirstName + " " + crewDto.Employees.FirstOrDefault(e =>
                                   e.EmpId == (crewDto.CrewDetails.FirstOrDefault(r => r.CrewID == p.CrewID && r.IsLead)
                                       .EmpId)).LastName)
                            : ""),
                        "<span class='delete-customer' data-val=" + p.CrewID +
                        "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                    }).ToArray(),
                keyValue = new
                {
                    employees = crewDto.Employees
                        .Select(k => new {ID = k.EmpId, Description = k.FirstName + " " + k.LastName})
                        .OrderBy(e => e.Description)
                }
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Crew by CrewID
        /// </summary>
        /// <param name="crewId"></param>
        /// <returns></returns>
        public ActionResult GetCrew(int crewId)
        {
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var crewDto = crewRepo.GetCrew(crewId);

            return Json(new
            {
                crew = crewDto.Crews.First(),
                crewDetails = crewDto.CrewDetails?.Where(r => r.CrewID == crewDto.Crews.First().CrewID).Select(r => new
                {
                    ID = r.EmpId, r.CrewDetailID, r.CrewID, r.EmpId,
                    EmployeeName = crewDto.Employees.FirstOrDefault(e => e.EmpId == r.EmpId)?.FirstName + " " +
                                   crewDto.Employees.FirstOrDefault(e => e.EmpId == r.EmpId)?.LastName,
                    r.IsLead, r.Active
                }),
                employees = crewDto.Employees.Where(e => e.Status == true)
                    .Select(k => new {ID = k.EmpId, Description = k.FirstName + " " + k.LastName})
                    .OrderBy(e => e.Description)
            });
        }

        /// <summary>
        /// Save Crew
        /// </summary>
        /// <param name="crewData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(CrewData crewData)
        {
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            bool hasSaved;
            var crew = crewData.crew;
            var crewDto = new CrewDTO();
            var recordExist = false;
            var message = string.Empty;
            var id = crew.CrewID;
            try
            {

                var crewRecords = crewRepo.GetCrews();
                crewDto.Crews = new List<Crew>();
                crewDto.CrewDetails = new List<CrewDetail>();
                if (crewRecords != null)
                {
                    if (crewRecords.Crews.Any())
                    {
                        if (crew.CrewID == 0)
                        {
                            if (crewRecords.Crews.FirstOrDefault(c => string.Equals(c.CrewName, crew.CrewName, StringComparison.CurrentCultureIgnoreCase) && c.Active) != null)
                            {
                                recordExist = true;
                            }
                        }
                        else
                        {
                            if (crewRecords.Crews.FirstOrDefault(c => string.Equals(c.CrewName, crew.CrewName, StringComparison.CurrentCultureIgnoreCase) && c.CrewID != crew.CrewID && c.Active) != null)
                            {
                                recordExist = true;
                            }
                        }
                    }
                }

                if (!recordExist)
                {
                    if (crew.CrewID == 0)
                    {
                        crew.DateCreated = DateTime.Now;
                        crew.DateUpdated = null;
                        crew.Active = true;
                        crewDto.Crews.Add(crew);
                        crewDto.CrewDetails = crewData.crewDetails;
                        id = crewRepo.Create(crewDto);
                       message = "Crew data inserted successfully.";
                    }
                    else
                    {
                        var crewRecord = crewRecords.Crews.FirstOrDefault(c => c.CrewID == crew.CrewID);
                        crewRecord.CrewName = crew.CrewName;
                        crewRecord.DateUpdated = DateTime.Now;
                        crewDto.Crews.Add(crewRecord);
                        crewDto.CrewDetails = crewData.crewDetails;
                        if(crewRepo.Edit(crewDto))
                        {
                            message = "Crew data updated successfully.";
                        }
                    }
                    hasSaved = true;
                }
                else
                {
                    hasSaved = false;
                    message = "Crew Name already exists.";
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Crew data could not be updated.";
                Console.WriteLine(ex);
            }
            

            return Json(new {hasSaved, message, ID = id });
        }
        /// <summary>
        /// Active/Inactive Crew status
        /// </summary>
        /// <param name="crewId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult SetActiveInactiveCrew(int crewId, bool tobeEnabled)
        {
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();

            crewRepo.SetActiveInactiveCrew(crewId, tobeEnabled);

            return Json(new { hasDeleted = true });
        }
        /// <summary>
        /// Active/Inactive Crew Detail Status
        /// </summary>
        /// <param name="crewDetailId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult SetActiveInactiveCrewDetail(int crewDetailId, bool tobeEnabled)
        {
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();

            crewRepo.SetActiveInactiveCrewDetail(crewDetailId, tobeEnabled);

            return Json(new { hasDeleted = true });
        }
    }
}
