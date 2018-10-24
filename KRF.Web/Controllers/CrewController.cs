using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
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
    public class CrewController : BaseController
    {
        //
        // GET: /Crew/

        public ActionResult Index()
        {
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var crews = crewRepo.GetCrews();
            TempData["Crews"] = crews;
            return View();
        }

        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Add(int ID = 0)
        {
            //ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            //var crews = crewRepo.GetCrew();
            ViewBag.ID = ID;
            return View();
        }
        /// <summary>
        /// Get Crew Detail
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetCrews(jQueryDataTableParamModel param)
        {
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            
            CrewDTO crewDTO = (CrewDTO)TempData["Crews"];
            if (crewDTO == null)
                crewDTO = crewRepo.GetCrews();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in crewDTO.Crews.Where(p => p.Active == true)
                          select new string[] {
                              "<span class='edit-customer' data-val=" + p.CrewID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.CrewName,
                              crewDTO.CrewDetails.Where(r => r.CrewID == p.CrewID && r.Active == true).ToList().Count().ToString(),
                              (crewDTO.CrewDetails.Where(r => r.CrewID == p.CrewID && r.IsLead == true).FirstOrDefault() != null ? (crewDTO.Employees.Where(e => e.EmpId == (crewDTO.CrewDetails.Where(r => r.CrewID == p.CrewID && r.IsLead == true).FirstOrDefault().EmpId)).FirstOrDefault().FirstName + " " + crewDTO.Employees.Where(e => e.EmpId == (crewDTO.CrewDetails.Where(r => r.CrewID == p.CrewID && r.IsLead == true).FirstOrDefault().EmpId)).FirstOrDefault().LastName) : ""),
                              "<span class='delete-customer' data-val=" + p.CrewID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray(),
                keyValue = new
                {
                    employees = crewDTO.Employees.Select(k => new { ID = k.EmpId, Description = k.FirstName + " " + k.LastName }).OrderBy(e=>e.Description)
                }
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Crew by CrewID
        /// </summary>
        /// <param name="crewID"></param>
        /// <returns></returns>
        public ActionResult GetCrew(int crewID)
        {
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var crewDTO = crewRepo.GetCrew(crewID);

            return Json(new
            {
                crew = crewDTO.Crews.First(),
                crewDetails = crewDTO.CrewDetails != null ?
                    crewDTO.CrewDetails.Where(r => r.CrewID == crewDTO.Crews.First().CrewID).Select(r => new { ID = r.EmpId, r.CrewDetailID, r.CrewID, r.EmpId, EmployeeName = crewDTO.Employees.Where(e=>e.EmpId == r.EmpId).FirstOrDefault().FirstName + " "+ crewDTO.Employees.Where(e=>e.EmpId == r.EmpId).FirstOrDefault().LastName, r.IsLead, r.Active })
                : null,
                employees = crewDTO.Employees.Where(e => e.Status == true).Select(k => new { ID = k.EmpId, Description = k.FirstName + " " + k.LastName }).OrderBy(e => e.Description)
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
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            bool hasSaved = true;
            var crew = crewData.crew;
            CrewDTO crewDTO = new CrewDTO();
            bool recordExist = false;
            string message = string.Empty;
            int ID = crew.CrewID;
            try
            {

                CrewDTO crewRecords = crewRepo.GetCrews();
                crewDTO.Crews = new List<Crew>();
                crewDTO.CrewDetails = new List<CrewDetail>();
                if (crewRecords != null)
                {
                    if (crewRecords.Crews.Count() > 0)
                    {
                        if (crew.CrewID == 0)
                        {
                            if (crewRecords.Crews.Where(c => c.CrewName.ToLower() == crew.CrewName.ToLower() && c.Active == true).FirstOrDefault() != null)
                            {
                                recordExist = true;
                            }
                        }
                        else
                        {
                            if (crewRecords.Crews.Where(c => c.CrewName.ToLower() == crew.CrewName.ToLower() && c.CrewID != crew.CrewID && c.Active == true).FirstOrDefault() != null)
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
                        crewDTO.Crews.Add(crew);
                        crewDTO.CrewDetails = crewData.crewDetails;
                        ID = crewRepo.Create(crewDTO);
                       message = "Crew data inserted successfully.";
                    }
                    else
                    {
                        Crew crewRecord = crewRecords.Crews.Where(c => c.CrewID == crew.CrewID).FirstOrDefault();
                        crewRecord.CrewName = crew.CrewName;
                        crewRecord.DateUpdated = DateTime.Now;
                        crewDTO.Crews.Add(crewRecord);
                        crewDTO.CrewDetails = crewData.crewDetails;
                        if(crewRepo.Edit(crewDTO))
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
            }
            

            return Json(new { hasSaved = hasSaved, message = message, ID = ID });
        }
        /// <summary>
        /// Active/Inactive Crew status
        /// </summary>
        /// <param name="crewID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult SetActiveInactiveCrew(int crewID, bool tobeEnabled)
        {
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();

            crewRepo.SetActiveInactiveCrew(crewID, tobeEnabled);

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
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();

            crewRepo.SetActiveInactiveCrewDetail(crewDetailId, tobeEnabled);

            return Json(new { hasDeleted = true });
        }
    }
}
