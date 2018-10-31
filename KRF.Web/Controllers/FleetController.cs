using KRF.Core.DTO.Master;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Linq;
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
            var fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();
            var fleets = fleetRepo.GetFleetDetails();
            TempData["Fleets"] = fleets;
            return View();
        }

        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Add(int id = 0)
        {
            ObjectFactory.GetInstance<IFleetManagementRepository>();
            //var employes = fleetRepo.GetEmployes();
            ViewBag.ID = id;
            return View();
        }

        /// <summary>
        /// Get Fleet Details
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetFleets(jQueryDataTableParamModel param)
        {
            var fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();

            var fleetDto = (FleetDTO)TempData["Fleets"] ?? fleetRepo.GetFleetDetails();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in fleetDto.Fleets.Where(p => p.Active)
                    let relevantFleetAssignment = fleetDto.FleetAssignments.FirstOrDefault(a => a.IsCurrent && a.FleetID == p.FleetID)
                    select new[] {
                              "<span class='edit-fleet' data-val=" + p.FleetID + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.FleetID.ToString(),
                              p.Make,
                              p.Model,
                              p.Year,
                              (relevantFleetAssignment != null ? fleetDto.Employes.FirstOrDefault(e => e.EmpId == (relevantFleetAssignment.EmployeeID)).FirstName + " " +fleetDto.Employes.FirstOrDefault(e => e.EmpId == (fleetDto.FleetAssignments.FirstOrDefault(a => a.IsCurrent && a.FleetID == p.FleetID).EmployeeID))?.LastName : ""),
                              (fleetDto.FleetStatus.FirstOrDefault(s => s.FleetStatusID == p.FleetStatusID)?.StatusName),
                              "<span class='delete-fleet' data-val=" + p.FleetID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Fleet Detail by FleetID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public ActionResult GetFleet(int fleetId)
        {
            var fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();
            var fleetDto = fleetRepo.GetFleetDetail(fleetId);
            var employees = fleetDto.Employes.Where(e => e.Status == true).Select(k => new { ID = k.EmpId, Description = (k.FirstName + " " + k.LastName) }).ToList();
            return Json(new
            {
                fleet = fleetDto.Fleets[0],
                keyValue = new
                {
                    fleetstatus = fleetDto.FleetStatus.Where(s=>s.Active).Select(k => new { ID = k.FleetStatusID, Description = k.StatusName }),
                    employees = employees
                },
                serviceDetails = fleetDto.FleetServices.Select(k => new { k.FleetServiceID, ServiceDate = k.ServiceDate.ToString("MM/dd/yyyy"), Notes = k.Notes, k.FleetID }),
                assignmentDetails = fleetDto.FleetAssignments.Select(k => new { k.FleetAssignmentID, k.EmployeeID, EmployeeName = (fleetDto.Employes.Where(e => e.EmpId == k.EmployeeID).FirstOrDefault().FirstName + " " + fleetDto.Employes.Where(e => e.EmpId == k.EmployeeID).FirstOrDefault().LastName), AssignmentDate = k.AssignmentDate.ToString("MM/dd/yyyy"), IsCurrent = k.IsCurrent, k.FleetID })
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
            var fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();
            bool hasSaved;
            string message;
            var id = 0;
            try
            {
                var fleet = fleetData.Fleet;
                id = fleet.FleetID;
                if (fleet.FleetID == 0)
                {
                    fleet.DateCreated = DateTime.Now;
                    fleet.DateUpdated = null;
                    fleet.Active = true;
                    id = fleetRepo.Create(fleet, fleetData.FleetServices, fleetData.FleetAssignments);
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
                Console.WriteLine(ex);
                hasSaved = false;
                message = "Fleet data could not be updated.";
            }


            return Json(new { hasSaved = hasSaved, message = message, ID = id });
        }

        /// <summary>
        /// Active/Inactive Fleet record
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleFleetStatus(int fleetId, bool tobeEnabled)
        {
            var fleetRepo = ObjectFactory.GetInstance<IFleetManagementRepository>();

            fleetRepo.ToggleFleetStatus(fleetId, tobeEnabled);

            return Json(new { hasDeleted = true });
        }
    }
}
