using KRF.Core.DTO.Master;
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
    public class FleetController : BaseController
    {
        //
        // GET: /Fleet/

        public ActionResult Index()
        {
            IFleetManagementRepository fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();
            var fleets = fleetRepo.GetFleetDetails();
            TempData["Fleets"] = fleets;
            return View();
        }

        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Add(int ID = 0)
        {
            IFleetManagementRepository fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();
            //var employes = fleetRepo.GetEmployes();
            ViewBag.ID = ID;
            return View();
        }

        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetFleets(jQueryDataTableParamModel param)
        {
            IFleetManagementRepository fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();

            FleetDTO fleetDTO = (FleetDTO)TempData["Fleets"];
            if (fleetDTO == null)
                fleetDTO = fleetRepo.GetFleetDetails();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in fleetDTO.Fleets.Where(p => p.Active == true)
                          select new string[] {
                              "<span class='edit-fleet' data-val=" + p.FleetID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.FleetID.ToString(),
                              p.Make,
                              p.Model,
                              p.Year.ToString(),
                              (fleetDTO.FleetAssignments.Where(a=>a.IsCurrent == true && a.FleetID == p.FleetID).FirstOrDefault() != null ? fleetDTO.Employes.Where(e=>e.EmpId == (fleetDTO.FleetAssignments.Where(a=>a.IsCurrent == true && a.FleetID == p.FleetID).FirstOrDefault().EmployeeID)).FirstOrDefault().FirstName + " " +fleetDTO.Employes.Where(e=>e.EmpId == (fleetDTO.FleetAssignments.Where(a=>a.IsCurrent == true && a.FleetID == p.FleetID).FirstOrDefault().EmployeeID)).FirstOrDefault().LastName : ""),
                              (fleetDTO.FleetStatus.Where(s=>s.FleetStatusID == p.FleetStatusID).FirstOrDefault().StatusName),
                              "<span class='delete-fleet' data-val=" + p.FleetID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Fleet Detail by FleetID
        /// </summary>
        /// <param name="fleetID"></param>
        /// <returns></returns>
        public ActionResult GetFleet(int fleetID)
        {
            IFleetManagementRepository fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();
            var fleetDTO = fleetRepo.GetFleetDetail(fleetID);
            var emps = fleetDTO.Employes.Where(e => e.Status == true).Select(k => new { ID = k.EmpId, Description = (k.FirstName + " " + k.LastName) }).ToList();
            return Json(new
            {
                fleet = fleetDTO.Fleets[0],
                keyValue = new
                {
                    fleetstatus = fleetDTO.FleetStatus.Where(s=>s.Active == true).Select(k => new { ID = k.FleetStatusID, Description = k.StatusName }),
                    employees = emps
                },
                serviceDetails = fleetDTO.FleetServices.Select(k => new { k.FleetServiceID, ServiceDate = k.ServiceDate.ToString("MM/dd/yyyy"), Notes = k.Notes, k.FleetID }),
                assignmentDetails = fleetDTO.FleetAssignments.Select(k => new { k.FleetAssignmentID, k.EmployeeID, EmployeeName = (fleetDTO.Employes.Where(e => e.EmpId == k.EmployeeID).FirstOrDefault().FirstName + " " + fleetDTO.Employes.Where(e => e.EmpId == k.EmployeeID).FirstOrDefault().LastName), AssignmentDate = k.AssignmentDate.ToString("MM/dd/yyyy"), IsCurrent = k.IsCurrent, k.FleetID })
            });
        }

        /// <summary>
        /// Save Fleet Detail
        /// </summary>
        /// <param name="fleetData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(FleetData fleetData)
        {
            IFleetManagementRepository fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            int ID = 0;
            try
            {
                var fleet = fleetData.Fleet;
                ID = fleet.FleetID;
                if (fleet.FleetID == 0)
                {
                    fleet.DateCreated = DateTime.Now;
                    fleet.DateUpdated = null;
                    fleet.Active = true;
                    ID = fleetRepo.Create(fleet, fleetData.FleetServices, fleetData.FleetAssignments);
                    message = "Record successfully inserted!";
                }
                else
                {
                    fleet.DateUpdated = DateTime.Now;
                    fleetRepo.Edit(fleet, fleetData.FleetServices, fleetData.FleetAssignments);
                    message = "Record successfully updated!";
                }
                hasSaved = true;
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Fleet data could not be updated.";
            }


            return Json(new { hasSaved = hasSaved, message = message, ID = ID });
        }

        /// <summary>
        /// Active/Inactive Fleet record
        /// </summary>
        /// <param name="fleetID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleFleetStatus(int fleetID, bool tobeEnabled)
        {
            IFleetManagementRepository fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();

            fleetRepo.ToggleFleetStatus(fleetID, tobeEnabled);

            return Json(new { hasDeleted = true });
        }
    }
}
