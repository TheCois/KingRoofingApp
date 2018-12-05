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
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var leads = leadRepo.GetLeads(GetAll());
            TempData["Leads"] = leads;
            return View();
        }

        public ActionResult Getleads(jQueryDataTableParamModel param)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();

            var leadDto = (LeadDTO)TempData["Leads"];
            if (leadDto == null)
                leadDto = leadRepo.GetLeads(GetNonConvertedLeadsPredicate());

            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            var estimateDto = (EstimateDTO)TempData["Estimates"];
            if (estimateDto == null)
                estimateDto = estimateRepo.ListAll();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in leadDto.Leads
                          select new[] {
                              "<span class='edit-lead' data-val=" + p.ID + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.ID.ToString(),
                              p.FirstName,
                              p.LastName,
                              "<a href=\"mailto:" + p.Email + "\" >" + p.Email + "</a>",
                              p.Telephone,
                              p.Cell,
                              p.Office,
                              p.Status == (int)LeadStatus.Inactive ? "": leadDto.Statuses.First(k => k.ID == p.Status).Description,
                              (estimateDto.Estimates.FirstOrDefault(e => e.LeadID == p.ID) != null ? estimateDto.Estimates.Count(e => e.LeadID == p.ID).ToString() : "0"),
                              "<span class='delete-lead' data-val=" + p.ID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray(),
                keyValue = new
                {
                    state = leadDto.States,
                    city = leadDto.Cities,
                    country = leadDto.Countries,
                    status = leadDto.Statuses,
                    contactMethod = leadDto.ContactMethod,
                    hearAboutUsList = leadDto.HearAboutUsList,
                    propertyRelationship = leadDto.PropertyRelationship,
                    projectStartTimelines = leadDto.ProjectStartTimelines,
                    projectTypes = leadDto.ProjectTypes,
                    roofAgeList = leadDto.RoofAgeList,
                    buildingStoriesList = leadDto.BuildingStoriesList,
                    roofTypes = leadDto.RoofTypes,
                    existingRoofs = leadDto.ExistingRoofs,
                    employees = (from e in leadDto.Employees select new {ID= e.EmpId, Description= e.LastName+", "+ e.FirstName}).ToList()
                }
            }, JsonRequestBehavior.AllowGet);
        }

        private Func<Lead, bool> GetNonConvertedLeadsPredicate()
        {
            return x => x.LeadStage == (int)LeadStageType.Lead && x.Status != (int)LeadStatus.ConvertToCustomer && x.Status != (int)LeadStatus.ConvertToEstimate;
        }

        private Func<Lead, bool> GetAll()
        {
            return x => true;
        }

        public ActionResult GetLead(int id)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var leadtDto = leadRepo.GetLead(id);
            return Json(new { lead = leadtDto.Leads.First(), appointment = leadtDto.Leads.First() .AppointmentDateTime.ToString()});
        }

        [ValidateAntiForgeryToken]
        public JsonResult Save(Lead lead)
        {
            var id = lead.ID;
            string message;
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            if (lead.ID == 0)
            {
                message = "Record successfully inserted!";
                lead.LeadStage = (int)LeadStageType.Lead;
                id = leadRepo.Create(lead);
            }
            else
            {
                // TODO this is a terrible hack to fix a bad situation.
                if (lead.LeadStage < 1)
                {
                    var oldLead = leadRepo.GetLead(id);
                    lead.LeadStage = oldLead.Leads.First().LeadStage;
                }

                // ----------------------------------------------------
                message = "Record successfully updated!";
                leadRepo.Edit(lead);
            }

            return Json(new { hasSaved = true, id = id, message = message });
        }

        public JsonResult DeleteLead(int id)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var hasSaved = leadRepo.Delete(id);

            return Json(new { hasSaved = hasSaved });
        }

        [HttpGet]
        public JsonResult GetItems(jQueryDataTableParamModel param, int id)
        {
            var item = new Item { ItemCode = "FHR-1001", Name = "Coil Nails", Category = "Category 1", Manufacturer = "Manufacturer1", Price = "$100" };
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
