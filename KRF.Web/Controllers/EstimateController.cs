using Dapper;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KRF.Persistence;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class EstimateController : BaseController
    {
        //
        // GET: /Estimate/

        public ActionResult Index(int? customerId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimates = estimateRepo.ListAll();

            if (customerId > 0)
                estimates.Estimates = estimates.Estimates.Where(k => k.LeadID == customerId).ToList();

            TempData["Estimates"] = estimates;
            return PartialView();
        }

        public ActionResult EstimateIndex(int? customerId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimates = estimateRepo.ListAll();

            if (customerId > 0)
                estimates.Estimates = estimates.Estimates.Where(k => k.LeadID == customerId).ToList();

            TempData["Estimates"] = estimates;
            return View("EstimateIndex");
        }

        private string GetRoofType(int roofTypeId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                const string query = "SELECT Description FROM RoofType WHERE Active = 1 AND ID = @ID";
                var roofType = conn.Query<string>(query, new {ID = roofTypeId}).SingleOrDefault();

                return roofType;
            }
        }

        public ActionResult GetEstimates(jQueryDataTableParamModel param, int? customerId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            var estimateDto = (EstimateDTO) TempData["Estimates"] ?? estimateRepo.ListAll();

            var customerKeyValue = estimateDto.Customers.Union(estimateDto.Leads).OrderBy(k => k.FirstName)
                .ToDictionary(k => k.ID);
            var statusKeyValue = estimateDto.Status.ToDictionary(k => k.ID);

            string[][] aaData;

            if (customerId != null && customerId > 0)
            {
                aaData = (from p in estimateDto.Estimates.Where(p => p.LeadID == customerId)
                    let roofType = p.RoofType > 0 ? GetRoofType(p.RoofType) : ""
                    let status = statusKeyValue.ContainsKey(p.Status) ? statusKeyValue[p.Status].Description : ""
                    let createdDate = (p.CreatedDate == null || p.CreatedDate == DateTime.MinValue)
                        ? string.Empty
                        : p.CreatedDate.ToShortDateString()
                    select new[]
                    {
                        "<span class='edit-customer edit-estimate' data-val=" + p.ID + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                        p.ID.ToString(),
                        p.Name,
                        roofType,
                        createdDate,
                        p.ContractPrice.ToString("N", new CultureInfo("en-US")),
                        status,
                        "<span class='delete-customer delete-estimate' data-val=" + p.ID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                    }).ToArray();
            }
            else
            {
                aaData = (from p in estimateDto.Estimates
                    let roofType = p.RoofType > 0 ? GetRoofType(p.RoofType) : string.Empty
                    let customer = customerKeyValue.ContainsKey(p.LeadID)
                        ? customerKeyValue[p.LeadID].FirstName + " " + customerKeyValue[p.LeadID].LastName
                        : string.Empty
                    let status = statusKeyValue.ContainsKey(p.Status)
                        ? statusKeyValue[p.Status].Description
                        : string.Empty
                    let createdDate = (p.CreatedDate == null || p.CreatedDate == DateTime.MinValue)
                        ? string.Empty
                        : p.CreatedDate.ToShortDateString()
                    select new[]
                    {
                        "<span class='edit-customer edit-estimate' data-val=" + p.ID + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                        p.ID.ToString(),
                        p.Name,
                        roofType,
                        customer,
                        createdDate,
                        p.ContractPrice.ToString("N", new CultureInfo("en-US")),
                        status,
                        "<span class='delete-customer delete-estimate' data-val=" + p.ID + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                    }).ToArray();
            }

            return Json(new
            {
                sEcho = param.sEcho,
                aaData,
                keyValue = new
                {
                    customers = estimateDto.Customers.OrderBy(p => p.FirstName).ToList(),
                    status = estimateDto.Status,
                    roofTypes = estimateDto.RoofTypes
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Estimate(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        public ActionResult GetEstimate(int id)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimate = new Estimate();
            IList<EstimateItem> estimateItems = new List<EstimateItem>();

            var estimateDto = estimateRepo.Select(id);

            if (id != 0)
            {
                estimate = estimateDto.Estimates.First();
                estimateItems = (from p in estimateDto.EstimateItems
                    join a in estimateDto.Assemblies on p.ItemAssemblyID equals a.Id
                    select p).ToList();
            }


            var unitOfMeasure = estimateDto.UnitOfMeasure.ToDictionary(k => k.Id);

            return Json(new
            {
                estimate = estimate,
                estimateItems = estimateItems,
                keyValue = new
                {
                    leads = estimateDto.Leads.OrderBy(p => p.FirstName)
                        .Select(k => new {ID = k.ID, Description = k.FirstName + " " + k.LastName}).ToList(),
                    customers = estimateDto.Customers.OrderBy(p => p.FirstName)
                        .Select(k => new {ID = k.ID, Description = k.FirstName + " " + k.LastName}).ToList(),
                    estimateForLead = estimateDto.Leads.Any(l => l.ID == estimate.LeadID),
                    address = estimateDto.CustomerAddress.Select(k => new
                        {ID = k.ID, CustomerID = k.LeadID, Description = k.Address1 + " " + k.Address2}).ToList(),
                    status = estimateDto.Status.Where(k => k.ID == 1 || k.ID == 2 || k.ID == 3 || k.ID == 6),
                    items = estimateDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        Code = k.Code,
                        Name = k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        Price = k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).ToList(),
                    assemblyCategories = estimateDto.Assemblies.Select(k => new {ID = k.Code, Description = k.Code})
                        .Distinct().OrderBy(p => p.Description).ToList(),
                    assemblies = estimateDto.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        Price = k.TotalRetailCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost
                    }).ToList(),
                    roofTypes = estimateDto.RoofTypes.Select(k => new {ID = k.ID, Description = k.Description})
                        .OrderBy(p => p.Description).ToList()
                }
            });
        }

        [HttpPost]
        public ActionResult Save(EstimateData estimateData, bool updateClicked)
        {
            string message;
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimate = estimateData.Estimate;
            var estimateItem = estimateData.EstimateItem;
            var id = estimate.ID;
            foreach (var estItem in estimateItem)
            {
                if (string.IsNullOrEmpty(estItem.UOMs))
                {
                    estItem.UOMs = GetAssemblyUoMs(estItem.ItemAssemblyID);
                }
            }

            if (estimate.ID == 0)
            {
                id = estimateRepo.Create(estimate, estimateItem);
                message = "Record successfully inserted!";
            }
            else
            {
                estimateRepo.Edit(estimate, estimateItem);
                message = "Record successfully updated!";
            }

            if (updateClicked)
            {
                return Json(new
                {
                    ID = id,
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("Index");
        }

        public ActionResult DeleteEstimate(int id)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            estimateRepo.Delete(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, string description, string uploaddatetime, int estimateId,
            int id)
        {
            byte[] data = null;
            var fileName = string.Empty;

            if (file != null)
            {
                var target = new MemoryStream();
                file.InputStream.CopyTo(target);
                data = target.ToArray();
                fileName = file.FileName;
            }

            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var e = new EstimateDocument
            {
                ID = id,
                EstimateID = estimateId,
                Name = fileName,
                Description = description,
                UploadDateTime = DateTime.Now,
                Text = data
            };

            estimateRepo.SaveDocument(e);

            return Json(new
            {
                data = "success"
            }, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult Download(int id)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            var estimateDocument = estimateRepo.GetEstimateDocument(id);
            var b = estimateDocument.Text;
            var filename = estimateDocument.Name;
            var contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");


            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        public ActionResult EstimateDocument(int estimateId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDocuments = estimateRepo.GetEstimateDocuments(estimateId);
            if (estimateId == 0)
                estimateDocuments.Clear();
            return Json(new
            {
                aaData = (from p in estimateDocuments
                    select new[]
                    {
                        "<span class='open-estimate-doc' data-val=" + p.ID + ">" + p.Name + "</span>",
                        p.Description,
                        (p.UploadDateTime == null
                            ? ""
                            : Convert.ToDateTime(p.UploadDateTime).ToString("MM/dd/yyyy HH:mm")),
                        //"<span class='link edit-estimate-doc' data-val=" + p.ID.ToString() + ">Edit</span>", 
                        "<span class='delete-estimate-doc' data-val=" + p.ID +
                        "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                    }).ToArray()

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteEstimateDocument(int estimateDocumentId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var isDeleted = estimateRepo.DeleteEstimateDocument(estimateDocumentId);
            return Json(new {isDeleted = isDeleted});
        }

        public string GetAssemblyUoMs(int assemblyId)
        {
            var assemblyRepo = ObjectFactory.GetInstance<IAssemblyManagementRepository>();
            var assembly = assemblyRepo.GetAssembly(assemblyId).assembly;
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            var unitOfMeasure = itemRepo.GetUnitOfMeasures().ToDictionary(k => k.Id, k => k.Name);
            return assembly.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(assembly.UnitOfMeasureId)
                ? unitOfMeasure[assembly.UnitOfMeasureId]
                : string.Empty;
            ;
        }

        public ActionResult GetAssemblyUom(int assemblyId)
        {
            return Json(new {UOMs = GetAssemblyUoMs(assemblyId)});
        }

        public FileContentResult CreateProposalDocument(int estimateId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            //return Json(new { documentCreate = true});

            var b = estimateRepo.CreateProposalDocument(estimateId);
            const string filename = "Proposal.docx";
            var contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
    }
}
