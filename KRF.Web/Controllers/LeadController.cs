using KRF.Web.Models;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using KRF.Core.Repository;
using KRF.Core.Entities.Sales;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.MISC;
using System;
using KRF.Core.Enums;

namespace KRF.Web.Controllers
{

    [CustomActionFilter.CustomActionFilter]
    public class LeadController : BaseController
    {
        public ActionResult Index()
        {
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var leads = leadRepo.GetLeads(GetNonConvertedLeadsPredicate());
            TempData["Leads"] = leads;
            return View();
        }

        public ActionResult Getleads(jQueryDataTableParamModel param)
        {
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();

            LeadDTO leadDTO = (LeadDTO)TempData["Leads"];
            if (leadDTO == null)
                leadDTO = leadRepo.GetLeads(GetNonConvertedLeadsPredicate());

            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            EstimateDTO estimateDTO = (EstimateDTO)TempData["Estimates"];
            if (estimateDTO == null)
                estimateDTO = estimateRepo.ListAll();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in leadDTO.Leads
                          select new string[] {
                              "<span class='edit-lead' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.ID.ToString(),
                              p.FirstName,
                              p.LastName,
                              "<a href=\"mailto:" + p.Email + "\" >" + p.Email + "</a>",
                              p.Telephone,
                              p.Cell,
                              p.Office,
                              p.Status == 0 ? "": leadDTO.Statuses.Where(k => k.ID == p.Status).First().Description,
                              (estimateDTO.Estimates.Where(e => e.LeadID == p.ID).FirstOrDefault() != null ? estimateDTO.Estimates.Where(e => e.LeadID == p.ID).Count().ToString() : "0"),
                              "<span class='delete-lead' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray(),
                keyValue = new
                {
                    state = leadDTO.States,
                    city = leadDTO.Cities,
                    country = leadDTO.Countries,
                    status = leadDTO.Statuses,
                    contactMethod = leadDTO.ContactMethod,
                    hearAboutUsList = leadDTO.HearAboutUsList,
                    propertyRelationship = leadDTO.PropertyRelationship,
                    projectStartTimelines = leadDTO.ProjectStartTimelines,
                    projectTypes = leadDTO.ProjectTypes,
                    roofAgeList = leadDTO.RoofAgeList,
                    buildingStoriesList = leadDTO.BuildingStoriesList,
                    roofTypes = leadDTO.RoofTypes,
                    existingRoofs = leadDTO.ExistingRoofs,
                    employees = (from e in leadDTO.Employees select new {ID= e.EmpId, Description= e.LastName+", "+ e.FirstName}).ToList()
                }
            }, JsonRequestBehavior.AllowGet);
        }

        private Func<Lead, bool> GetNonConvertedLeadsPredicate()
        {
            return x => x.LeadStage == (int)LeadStageType.Lead && x.Status != (int)Status.ConvertToCustomer && x.Status != (int)Status.ConvertToEstimate;
        }

        public ActionResult GetLead(int id)
        {
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var leadtDTO = leadRepo.GetLead(id);
            return Json(new { lead = leadtDTO.Leads.First(), appointment = leadtDTO.Leads.First() .AppointmentDateTime.ToString()});
        }

        [ValidateAntiForgeryToken]
        public JsonResult Save(Lead lead)
        {
            int ID = lead.ID;
            string message = string.Empty;
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            if (lead.ID == 0)
            {
                message = "Record successfully inserted!";
                lead.LeadStage = (int)LeadStageType.Lead;
                ID = leadRepo.Create(lead);
            }
            else
            {
                // TODO this is a terrible hack to fix a bad situation.
                if (lead.LeadStage < 1)
                {
                    var oldLead = leadRepo.GetLead(ID);
                    lead.LeadStage = oldLead.Leads.First().LeadStage;
                }

                // ----------------------------------------------------
                message = "Record successfully updated!";
                leadRepo.Edit(lead);
            }

            return Json(new { hasSaved = true, id = ID, message = message });
        }

        public JsonResult DeleteLead(int id)
        {
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            bool hasSaved = leadRepo.Delete(id);

            return Json(new { hasSaved = hasSaved });
        }

        [HttpGet]
        public JsonResult GetItems(jQueryDataTableParamModel param, int ID)
        {
            Item item = new Item { ItemCode = "FHR-1001", Name = "Coil Nails", Category = "Category 1", Manufacturer = "Manufacturer1", Price = "$100" };
            IList<Item> items = new List<Item>();
            items.Add(item);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = new List<string[]>() {
                    new string[] {"Microsoft", "Redmond", "22-122-1101", "a.com", "1"},
                    new string[] {"Google", "Mountain View", "22-122-1101", "a.com", "2"},
                    new string[] {"Gowi", "Pancevo", "22-122-1101", "a.com", "3"},
                    new string[] {"Microsoft", "Redmond", "22-122-1101", "a.com", "1"},
                    new string[] {"Google", "Mountain View", "22-122-1101", "a.com", "2"},
                    new string[] {"Gowi", "Pancevo", "22-122-1101", "a.com", "3"},
                    new string[] {"Microsoft", "Redmond", "22-122-1101", "a.com", "1"},
                    new string[] {"Google", "Mountain View", "22-122-1101", "a.com", "2"},
                    new string[] {"Gowi", "Pancevo", "22-122-1101", "a.com", "3"},
                    new string[] {"Microsoft", "Redmond", "22-122-1101", "a.com", "1"},
                    new string[] {"Google", "Mountain View", "22-122-1101", "a.com", "2"},
                    new string[] {"Gowi", "Pancevo", "22-122-1101", "a.com", "3"},
                    new string[] {"Microsoft", "Redmond", "22-122-1101", "a.com", "1"},
                    new string[] {"Google", "Mountain View", "22-122-1101", "a.com", "2"},
                    new string[] {"Gowi", "Pancevo", "22-122-1101", "a.com", "3"},
                    new string[] {"Microsoft", "Redmond", "22-122-1101", "a.com", "1"},
                    new string[] {"Google", "Mountain View", "22-122-1101", "a.com", "2"},
                    new string[] {"Gowi", "Pancevo", "22-122-1101", "a.com", "<span class='link'> </span>"}
                }
            },
          JsonRequestBehavior.AllowGet);
        }


    }
}
