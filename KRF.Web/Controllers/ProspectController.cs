using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
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
    public class ProspectController : BaseController
    {
        //
        // GET: /Opportunity/

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Lead");

            IProspectManagementRepository prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            var prospects = prospectRepo.GetProspects();
            TempData["Prospects"] = prospects;
            return View();
        }

        public ActionResult GetProspects(jQueryDataTableParamModel param)
        {
            ProspectDTO prospectDTO = (ProspectDTO)TempData["Prospects"];
            if (prospectDTO == null)
            {
                IProspectManagementRepository prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
                prospectDTO = prospectRepo.GetProspects();
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in prospectDTO.Propects
                          select new string[] {p.ID.ToString(), p.FirstName, p.LastName, "<a href=\"mailto:"+p.Email+"\" >"+p.Email+"</a>", p.Telephone,
                             p.Status == 0? "":  prospectDTO.Statuses.Where(k=>k.ID==p.Status).First().Description,
                    "<span class='edit-prospect' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>", 
                    "<span class='delete-prospect' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>" }).ToArray(),
                cities = prospectDTO.Cities,
                states = prospectDTO.States,
                countries = prospectDTO.Countries,
                status = prospectDTO.Statuses
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProspect(int id)
        {
            IProspectManagementRepository prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            var prospectDTO = prospectRepo.GetProspect(id);
            return Json(new { prospect = prospectDTO.Propects.First() });
        }

        [ValidateAntiForgeryToken]
        public JsonResult Save(Prospect prospect)
        {
            string message = string.Empty;
            IProspectManagementRepository prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            if (prospect.ID == 0) { return Json(new { id = prospectRepo.Create(prospect), message = "Record successfully inserted!" }); }
            else { return Json(new { id = prospect.ID, prospect = prospectRepo.Edit(prospect), message = "Record successfully updated!" }); }
        }

        public ActionResult DeleteProspect(int id)
        {
            IProspectManagementRepository prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            return Json(new { prospect = prospectRepo.Delete(id) });
        }

        public ActionResult ImportProspect(ProspectModel prospectModel)
        {
            ProspectDTO prospectDTO = (ProspectDTO)TempData["Prospects"];
            IProspectManagementRepository prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>(); ;
            if (prospectDTO == null)
            {
                prospectDTO = prospectRepo.GetProspects();
            }

            var cities = prospectDTO.Cities.ToDictionary(k => k.Description.Trim());
            var states = prospectDTO.States.ToDictionary(k => k.Abbreviation.Trim());
            var existingProspect = prospectDTO.Propects.ToDictionary(k => k.FirstName.Trim() + k.LastName.Trim() + k.Address1.Trim());
            IList<Prospect> prospects = new List<Prospect>();

            foreach (var p in prospectModel.ProspectData)
            {
                var prospectexistsData = p.FirstName.Trim() + p.LastName.Trim() + p.Address1.Trim();
                if (!existingProspect.ContainsKey(prospectexistsData))
                {
                    Prospect prospect = new Prospect
                    {
                        Status = 1,
                        Address1 = p.Address1,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        ZipCode = p.ZipCode,
                        City = cities[p.City.Trim()].ID,
                        State = states[p.State.Trim()].ID,
                    };
                    prospects.Add(prospect);
                }
            }

            prospectRepo.SaveProspects(prospects);

            return null;
        }
    }
}
