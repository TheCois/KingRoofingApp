using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
using KRF.Core.Repository;
using KRF.Web.Models;
using System.Collections.Generic;
using System.Linq;
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

            var prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            var prospects = prospectRepo.GetProspects();
            TempData["Prospects"] = prospects;
            return View();
        }

        public ActionResult GetProspects(jQueryDataTableParamModel param)
        {
            var prospectDto = (ProspectDTO)TempData["Prospects"];
            if (prospectDto == null)
            {
                var prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
                prospectDto = prospectRepo.GetProspects();
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in prospectDto.Propects
                          select new[] {p.ID.ToString(), p.FirstName, p.LastName, "<a href=\"mailto:"+p.Email+"\" >"+p.Email+"</a>", p.Telephone,
                             p.Status == 0? "":  prospectDto.Statuses.First(k => k.ID==p.Status).Description,
                    "<span class='edit-prospect' data-val=" + p.ID + "><ul><li class='edit'><a href='#non'>Edit</a></li></ul></span>", 
                    "<span class='delete-prospect' data-val=" + p.ID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>" }).ToArray(),
                cities = prospectDto.Cities,
                states = prospectDto.States,
                countries = prospectDto.Countries,
                status = prospectDto.Statuses
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProspect(int id)
        {
            var prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            var prospectDto = prospectRepo.GetProspect(id);
            return Json(new { prospect = prospectDto.Propects.First() });
        }

        [ValidateAntiForgeryToken]
        public JsonResult Save(Prospect prospect)
        {
            var prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            if (prospect.ID == 0) { return Json(new { id = prospectRepo.Create(prospect), message = "Record successfully inserted!" }); }

            return Json(new { id = prospect.ID, prospect = prospectRepo.Edit(prospect), message = "Record successfully updated!" });
        }

        public ActionResult DeleteProspect(int id)
        {
            var prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            return Json(new { prospect = prospectRepo.Delete(id) });
        }

        public ActionResult ImportProspect(ProspectModel prospectModel)
        {
            var prospectDto = (ProspectDTO)TempData["Prospects"];
            var prospectRepo = ObjectFactory.GetInstance<IProspectManagementRepository>();
            if (prospectDto == null)
            {
                prospectDto = prospectRepo.GetProspects();
            }

            var cities = prospectDto.Cities.ToDictionary(k => k.Description.Trim());
            var states = prospectDto.States.ToDictionary(k => k.Abbreviation.Trim());
            var existingProspect = prospectDto.Propects.ToDictionary(k => k.FirstName.Trim() + k.LastName.Trim() + k.Address1.Trim());
            IList<Prospect> prospects = new List<Prospect>();

            foreach (var p in prospectModel.ProspectData)
            {
                var prospectexistsData = p.FirstName.Trim() + p.LastName.Trim() + p.Address1.Trim();
                if (!existingProspect.ContainsKey(prospectexistsData))
                {
                    var prospect = new Prospect
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
