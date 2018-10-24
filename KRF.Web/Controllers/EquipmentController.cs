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
    public class EquipmentController : BaseController
    {
        //
        // GET: /Equipment/

        public ActionResult Index()
        {
            IEquipmentManagementRepository equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            var equipments = equipmentRepo.GetEquipments();
            TempData["Equipments"] = equipments;
            return View();
        }
        /// <summary>
        /// Get Equipment Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Add(int ID = 0)
        {
            IEquipmentManagementRepository equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            var employes = equipmentRepo.GetEquipment(ID);
            ViewBag.ID = ID;
            ViewBag.EditMode = true;
            return View();
        }
        /// <summary>
        /// Get Equipment Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult View(int ID)
        {
            IEquipmentManagementRepository equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            var employes = equipmentRepo.GetEquipment(ID);
            ViewBag.ID = ID;
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
            IEquipmentManagementRepository equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();

            EquipmentDTO equipmentDTO = (EquipmentDTO)TempData["Equipments"];
            if (equipmentDTO == null)
                equipmentDTO = equipmentRepo.GetEquipments();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in equipmentDTO.Equipments.Where(p => p.Active == true)
                          select new string[] {
                              "<span class='edit-equipment' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.ID.ToString(),
                              "<span><ul><li><a class='noicon' href='" +@Url.Action("View", "Equipment", new { ID = p.ID})+ "'>"+p.EquipmentName+"</a></li></ul></span>", p.EquipmentID == 0 ? string.Empty : p.EquipmentID.ToString(), p.ModelNumber, p.SNNo, (p.PurchaseDate != null ? Convert.ToDateTime(p.PurchaseDate).ToShortDateString() : ""),
                              p.PurchasePrice.ToString("0.00"),
                              Convert.ToString(p.Vendor),
                              (equipmentDTO.EquipmentStatus.Where(s=>s.EquipmentStatusID == p.EquipmentStatusID).FirstOrDefault().StatusName),
                              "<span class='delete-equipment' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>" }).ToArray(),
                keyValue = new
                {
                    equipmentStatus = equipmentDTO.EquipmentStatus.Where(p=>p.Active == true).Select(k => new { ID = k.EquipmentStatusID, Description = k.StatusName})
                }
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Equpment by EquipmentID
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <returns></returns>
        public ActionResult GetEquipment(int equipmentID)
        {
            IEquipmentManagementRepository equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            var equipmentDTO = equipmentRepo.GetEquipment(equipmentID);
            return Json(new
            {
                equipment = equipmentDTO.Equipments.First(),
                keyValue = new
                {
                    equipmentStatus = equipmentDTO.EquipmentStatus.Where(p => p.Active == true).Select(k => new { ID = k.EquipmentStatusID, Description = k.StatusName })
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
            IEquipmentManagementRepository equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            var equipemnts = equipmentRepo.GetEquipments();
            bool isExist = false;
            int ID = equipment.ID;
            try
            {
                
                if (equipment.ID == 0)
                {
                    if (equipemnts.Equipments.Where(e => e.SNNo.ToLower().Trim() == equipment.SNNo.ToLower().Trim() && e.Active == true).FirstOrDefault() != null)
                    {
                        isExist = true;
                        message = "S/N No. already exists.";
                    }
                    if (!isExist)
                    {
                        equipment.DateCreated = DateTime.Now;
                        equipment.DateUpdated = null;
                        equipment.Active = true;
                        ID = equipmentRepo.Create(equipment);
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
                    if (equipemnts.Equipments.Where(e => e.ID != equipment.ID && e.SNNo.ToLower().Trim() == equipment.SNNo.ToLower().Trim() && e.Active == true).FirstOrDefault() != null)
                    {
                        isExist = true;
                        message = "S/N No. already exists.";
                    }
                    if (!isExist)
                    {
                        equipment.DateCreated = equipemnts.Equipments.Where(e => e.ID == equipment.ID).FirstOrDefault().DateCreated;
                        equipment.Active = equipemnts.Equipments.Where(e => e.ID == equipment.ID).FirstOrDefault().Active;
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
                hasSaved = false;
                message = "Equipment data could not be updated.";
            }


            return Json(new { hasSaved = hasSaved, message = message, ID = ID });
        }
        /// <summary>
        /// Active/Inactive Equipement status
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleEquipmentStatus(int equipmentID, bool tobeEnabled)
        {
            IEquipmentManagementRepository equipmentRepo = ObjectFactory.GetInstance<IEquipmentManagementRepository>();
            equipmentRepo.ToggleEquipmentStatus(equipmentID, tobeEnabled);

            return Json(new { hasDeleted = true });
        }
    }
}
