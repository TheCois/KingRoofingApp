using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
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
    public class AdministrationController : BaseController
    {
        //
        // GET: /Administration/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(jQueryDataTableParamModel param, int type)
        {
            IAdministrationRepository administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            List<MasterRecords> masterRecords = administrationRepo.GetMasterRecordsByType(type).Where(x => x.ID < 2000).ToList();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = masterRecords.Count,
                iTotalDisplayRecords = masterRecords.Count,
                aaData = (from p in masterRecords
                          select new string[] {
                              "<span class='edit-administration' data-val=" + p.ID.ToString() + " data-desc="+ Uri.EscapeDataString(p.Description.Trim()) + " data-extraField1="+ (string.IsNullOrEmpty(p.ExtraField1) ? "" : Uri.EscapeDataString(p.ExtraField1.Trim())) +"><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.Description,
                              p.ExtraField1,
                              p.Active ? "Active" : "In-Active",
                              "<span class='delete-administration' data-val=" + p.ID.ToString() + " data-active="+ (!p.Active).ToString() +"><ul><li class='"+ (p.Active ? "delete" : "update") +"'><a href='#non'>"+ (p.Active ? "De-Activate" : "Activate") +"</a></li></ul></span>"
                          }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdministrationType()
        {
            IAdministrationRepository administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            List<Core.Entities.ValueList.AdministrationType> administrationTypes = administrationRepo.GetAdministrationType();
            return Json(new
            {
                types = administrationTypes
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMasterRecord(int type, int id, bool activate)
        {
            IAdministrationRepository administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            string message = string.Empty;
            bool success = false;
            try
            {
                success = administrationRepo.SetActiveInactive(type, id, activate);
                if (!activate)
                    message = "Record successfully de-activated.";
                else
                    message = "Record successfully activated.";
            }
            catch (Exception ex)
            {
                message = "Some error occured while deactivating the record. Please try again later.";
                success = false;
            }
            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveMasterRecord(int type, int id, string description, string extraField1)
        {
            IAdministrationRepository administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            string message = string.Empty;
            bool success = false;
            try
            {
                List<MasterRecords> masterRecords = administrationRepo.GetMasterRecordsByType(type);
                if(masterRecords.Find(p=>p.Description.ToLower().Trim() == description.ToLower().Trim() && p.ID != id)==null)
                {
                    if (id > 0)
                    {
                        MasterRecords masterRecord = masterRecords.Where(p => p.ID == id).FirstOrDefault();
                        success = administrationRepo.Edit(type, id, description, masterRecord.Active, extraField1);
                        if (success)
                        {
                            message = "Record successfully updated.";
                        }
                        else
                        {
                            message = "Record could not be updated.";
                        }
                    }
                    else
                    {
                        var recordid = administrationRepo.Create(type, description, extraField1);
                        if (recordid > 0)
                        {
                            success = true;
                            message = "Record successfully inserted.";
                        }
                        else
                        {
                            message = "Record could not be inserted.";
                        }
                    }
                }
                else
                {
                    message = "Description already exists. Please enter different description.";
                }
            }
            catch (Exception ex)
            {
                message = "Some error occured while inserting the record. Please try again later.";
                success = false;
            }
            return Json(new { success = success, message = message }, JsonRequestBehavior.AllowGet);
        }
    }
}
