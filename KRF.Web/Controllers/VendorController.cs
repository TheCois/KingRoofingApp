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
    public class VendorController : BaseController
    {
        //
        // GET: /Vendor/

        public ActionResult Index()
        {
            var vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            var vendors = vendorRepo.ListAllVendors();
            TempData["Vendors"] = vendors;
            return View();
        }

        /// <summary>
        /// Get Add Vendor
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
        /// Get Vendor Add View
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
        /// Get Vendor Detail
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetVendors(jQueryDataTableParamModel param)
        {
            var vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();

            var vendorDto = (VendorDTO)TempData["Vendors"] ?? vendorRepo.ListAllVendors();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in vendorDto.Vendors.Where(p => p.Active)
                          select new string[] {
                              "<span class='edit-customer' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              "<span><ul><li><a class='noicon' href='" + @Url.Action("View", "Vendor", new { ID = p.ID })+ "'>" + p.VendorName + "</a></li></ul></span>",
                              p.Manager,
                              (p.SalesRep ?? ""),
                              p.Phone,
                              p.Fax,
                              "<a href=\"mailto:" + p.Email + "\" >" + p.Email + "</a>",
                              "<span class='delete-customer' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Vendor by vendorID
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public ActionResult GetVendor(int vendorId)
        {
            var vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            var vendorDto = vendorRepo.GetVendor(vendorId);

            return Json(new
            {
                vendor = vendorDto.Vendors.First()
            });
        }

        /// <summary>
        /// Save Vendor
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(Vendor vendor)
        {
            var vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            var hasSaved = true;
            var vendorDto = new VendorDTO();
            var recordExist = false;
            var message = string.Empty;
            var id = vendor.ID;
            try
            {
                var vendorRecords = vendorRepo.ListAllVendors();
                if (!string.IsNullOrEmpty(vendor.Email))
                {
                    if (vendorRecords != null)
                    {
                        if (vendorRecords.Vendors.Any())
                        {
                            if (vendor.ID == 0)
                            {
                                if (vendorRecords.Vendors.Where(c => c.Email.ToLower() == vendor.Email.ToLower() && c.Active).FirstOrDefault() != null)
                                {
                                    recordExist = true;
                                }
                            }
                            else
                            {
                                if (vendorRecords.Vendors.Where(c => (c.Email != null && c.Email.ToLower() == vendor.Email.ToLower()) && c.ID != vendor.ID && c.Active).FirstOrDefault() != null)
                                {
                                    recordExist = true;
                                }
                            }
                        }
                    }
                }

                if (!recordExist)
                {
                    if (vendor.ID == 0)
                    {
                        vendor.DateCreated = DateTime.Now;
                        vendor.DateUpdated = null;
                        vendor.Active = true;
                        id = vendorRepo.CreateVendor(vendor);
                        message = "Vendor detail inserted successfully.";
                    }
                    else
                    {
                        var vendorRecord = vendorRecords.Vendors.Where(c => c.ID == vendor.ID).FirstOrDefault();
                        vendor.DateUpdated = DateTime.Now;
                        vendor.Active = vendorRecord.Active;
                        vendor.DateCreated = vendorRecord.DateCreated;
                        if (vendorRepo.EditVendor(vendor))
                        {
                            message = "Vendor detail updated successfully.";
                        }
                    }
                    hasSaved = true;
                }
                else
                {
                    hasSaved = false;
                    message = "Vendor email already exists.";
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Vendor data could not be updated.";
            }


            return Json(new { hasSaved = hasSaved, message = message, ID = id });
        }
        /// <summary>
        /// Active/Inactive Vendor status
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult SetActiveInactiveVendor(int vendorId, bool active)
        {
            var vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();

            vendorRepo.SetActiveInactiveVendor(vendorId, active);

            return Json(new { hasDeleted = true });
        }
    }
}
