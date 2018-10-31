using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class EquipmentController : BaseController
    {
        //
        // GET: /Equipment/

        public ActionResult Index()
        {
            var equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            var equipments = equipmentRepo.GetEquipments();
            TempData["Equipments"] = equipments;
            return View();
        }
        /// <summary>
        /// Get Equipment Add View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Add(int id = 0)
        {
            ViewBag.ID = id;
            ViewBag.EditMode = true;
            return View();
        }
        /// <summary>
        /// Get Equipment Add View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult View(int id)
        {
            ViewBag.ID = id;
            ViewBag.EditMode = false;
            return View("Add");
        }
        /// <summary>
        /// Get Equipment Detail
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetEquipments(jQueryDataTableParamModel param)
        {
            var equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();

            var equipmentDto = (EquipmentDTO)TempData["Equipments"] ?? equipmentRepo.GetEquipments();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in equipmentDto.Equipments.Where(p => p.Active)
                          select new[] {
                              "<span class='edit-equipment' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.ID.ToString(),
                              "<span><ul><li><a class='noicon' href='" +@Url.Action("View", "Equipment", new {p.ID})+ "'>"+p.EquipmentName+"</a></li></ul></span>", p.EquipmentID == 0 ? string.Empty : p.EquipmentID.ToString(), p.ModelNumber, p.SNNo, (p.PurchaseDate != null ? Convert.ToDateTime(p.PurchaseDate).ToShortDateString() : ""),
                              p.PurchasePrice.ToString("0.00"),
                              Convert.ToString(p.Vendor),
                              equipmentDto.EquipmentStatus.FirstOrDefault(s => s.EquipmentStatusID == p.EquipmentStatusID)?.StatusName,
                              "<span class='delete-equipment' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>" }).ToArray(),
                keyValue = new
                {
                    equipmentStatus = equipmentDto.EquipmentStatus.Where(p=>p.Active).Select(k => new { ID = k.EquipmentStatusID, Description = k.StatusName})
                }
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Equipment by EquipmentID
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <returns></returns>
        public ActionResult GetEquipment(int equipmentId)
        {
            var equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            var equipmentDto = equipmentRepo.GetEquipment(equipmentId);
            return Json(new
            {
                equipment = equipmentDto.Equipments.First(),
                keyValue = new
                {
                    equipmentStatus = equipmentDto.EquipmentStatus.Where(p => p.Active).Select(k => new { ID = k.EquipmentStatusID, Description = k.StatusName })
                }
            });
        }
        /// <summary>
        /// Save Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(Equipment equipment)
        {
            var equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            bool hasSaved;
            var message = string.Empty;
            var equipments = equipmentRepo.GetEquipments();
            var isExist = false;
            var id = equipment.ID;
            try
            {
                
                if (equipment.ID == 0)
                {
                    if (equipments.Equipments.FirstOrDefault(e => e.SNNo.ToLower().Trim() == equipment.SNNo.ToLower().Trim() && e.Active) != null)
                    {
                        isExist = true;
                        message = "S/N No. already exists.";
                    }
                    if (!isExist)
                    {
                        equipment.DateCreated = DateTime.Now;
                        equipment.DateUpdated = null;
                        equipment.Active = true;
                        id = equipmentRepo.Create(equipment);
                        message = "Equipment data inserted successfully!!!";
                        hasSaved = true;
                    }
                    else
                    {
                        hasSaved = false;
                    }
                }
                else
                {
                    if (equipments.Equipments.FirstOrDefault(e => e.ID != equipment.ID && e.SNNo.ToLower().Trim() == equipment.SNNo.ToLower().Trim() && e.Active) != null)
                    {
                        isExist = true;
                        message = "S/N No. already exists.";
                    }
                    if (!isExist)
                    {
                        var relevantEquipment = equipments.Equipments.FirstOrDefault(e => e.ID == equipment.ID);
                        equipment.DateCreated = relevantEquipment.DateCreated;
                        equipment.Active = relevantEquipment.Active;
                        equipment.DateUpdated = DateTime.Now;
                        equipmentRepo.Edit(equipment);
                        message = "Equipment data updated successfully!!!";
                        hasSaved = true;
                    }
                    else
                    {
                        hasSaved = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                hasSaved = false;
                message = "Equipment data could not be updated.";
            }


            return Json(new {hasSaved, message, ID = id });
        }
        /// <summary>
        /// Active/Inactive Equipment status
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleEquipmentStatus(int equipmentId, bool tobeEnabled)
        {
            var equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            equipmentRepo.ToggleEquipmentStatus(equipmentId, tobeEnabled);

            return Json(new { hasDeleted = true });
        }
    }
}
