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
    public class VendorController : BaseController
    {
        //
        // GET: /Vendor/

        public ActionResult Index()
        {
            IVendorManagementRepository vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            var vendors = vendorRepo.ListAllVendors();
            TempData["Vendors"] = vendors;
            return View();
        }

        /// <summary>
        /// Get Add Vendor
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Add(int ID = 0)
        {
            ViewBag.ID = ID;
            ViewBag.EditMode = true;
            return View();
        }
        /// <summary>
        /// Get Vendor Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult View(int ID)
        {
            ViewBag.ID = ID;
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
            IVendorManagementRepository vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();

            VendorDTO vendorDTO = (VendorDTO)TempData["Vendors"];
            if (vendorDTO == null)
                vendorDTO = vendorRepo.ListAllVendors();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in vendorDTO.Vendors.Where(p => p.Active == true)
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
        /// <param name="crewID"></param>
        /// <returns></returns>
        public ActionResult GetVendor(int vendorID)
        {
            IVendorManagementRepository vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            var vendorDTO = vendorRepo.GetVendor(vendorID);

            return Json(new
            {
                vendor = vendorDTO.Vendors.First()
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
            IVendorManagementRepository vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            bool hasSaved = true;
            VendorDTO vendorDTO = new VendorDTO();
            bool recordExist = false;
            string message = string.Empty;
            int ID = vendor.ID;
            try
            {
                VendorDTO vendorRecords = vendorRepo.ListAllVendors();
                if (!string.IsNullOrEmpty(vendor.Email))
                {
                    if (vendorRecords != null)
                    {
                        if (vendorRecords.Vendors.Count() > 0)
                        {
                            if (vendor.ID == 0)
                            {
                                if (vendorRecords.Vendors.Where(c => c.Email.ToLower() == vendor.Email.ToLower() && c.Active == true).FirstOrDefault() != null)
                                {
                                    recordExist = true;
                                }
                            }
                            else
                            {
                                if (vendorRecords.Vendors.Where(c => (c.Email != null && c.Email.ToLower() == vendor.Email.ToLower()) && c.ID != vendor.ID && c.Active == true).FirstOrDefault() != null)
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
                        ID = vendorRepo.CreateVendor(vendor);
                        message = "Vendor detail inserted successfully.";
                    }
                    else
                    {
                        Vendor vendorRecord = vendorRecords.Vendors.Where(c => c.ID == vendor.ID).FirstOrDefault();
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


            return Json(new { hasSaved = hasSaved, message = message, ID = ID });
        }
        /// <summary>
        /// Active/Inactive Vendor status
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult SetActiveInactiveVendor(int vendorID, bool active)
        {
            IVendorManagementRepository vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();

            vendorRepo.SetActiveInactiveVendor(vendorID, active);

            return Json(new { hasDeleted = true });
        }
    }
}
