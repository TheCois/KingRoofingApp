using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Linq;
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
            var administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            var masterRecords = administrationRepo.GetMasterRecordsByType(type).Where(x => x.ID < 2000).ToList();
            return Json(new
            {
                param.sEcho,
                iTotalRecords = masterRecords.Count,
                iTotalDisplayRecords = masterRecords.Count,
                aaData = (from p in masterRecords
                          select new[] {
                              "<span class='edit-administration' data-val=" + p.ID + " data-desc="+ Uri.EscapeDataString(p.Description.Trim()) + " data-extraField1="+ (string.IsNullOrEmpty(p.ExtraField1) ? "" : Uri.EscapeDataString(p.ExtraField1.Trim())) +"><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.Description,
                              p.ExtraField1,
                              p.Active ? "Active" : "In-Active",
                              "<span class='delete-administration' data-val=" + p.ID + " data-active="+ (!p.Active) +"><ul><li class='"+ (p.Active ? "delete" : "update") +"'><a href='#non'>"+ (p.Active ? "De-Activate" : "Activate") +"</a></li></ul></span>"
                          }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAdministrationType()
        {
            var administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            var administrationTypes = administrationRepo.GetAdministrationType();
            return Json(new
            {
                types = administrationTypes
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMasterRecord(int type, int id, bool activate)
        {
            var administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            string message;
            bool success;
            try
            {
                success = administrationRepo.SetActiveInactive(type, id, activate);
                message = activate ? "Record successfully activated." : "Record successfully de-activated.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                message = "Some error occured while deactivating the record. Please try again later.";
                success = false;
            }
            return Json(new {success, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveMasterRecord(int type, int id, string description, string extraField1)
        {
            var administrationRepo = ObjectFactory.GetInstance<IAdministrationRepository>();
            string message;
            var success = false;
            try
            {
                var masterRecords = administrationRepo.GetMasterRecordsByType(type);
                if (masterRecords.Find(p=>p.Description.ToLower().Trim() == description.ToLower().Trim() && p.ID != id)==null)
                {
                    if (id > 0)
                    {
                        var masterRecord = masterRecords.FirstOrDefault(p => p.ID == id);
                        success = administrationRepo.Edit(type, id, description, masterRecord.Active, extraField1);
                        message = success ? "Record successfully updated." : "Record could not be updated.";
                    }
                    else
                    {
                        var recordId = administrationRepo.Create(type, description, extraField1);
                        if (recordId > 0)
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
                Console.WriteLine(ex);
                message = "Some error occured while inserting the record. Please try again later.";
                success = false;
            }
            return Json(new {success, message }, JsonRequestBehavior.AllowGet);
        }
    }
}
