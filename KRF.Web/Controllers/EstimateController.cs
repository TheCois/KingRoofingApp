using Dapper;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Sales;
using KRF.Core.Repository;
using KRF.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class EstimateController : BaseController
    {
        //
        // GET: /Estimate/

        public ActionResult Index(int? customerID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimates = estimateRepo.ListAll();

            if (customerID > 0)
                estimates.Estimates = estimates.Estimates.Where(k => k.LeadID == customerID).ToList();

            TempData["Estimates"] = estimates;
            return PartialView();
        }

        public ActionResult EstimateIndex(int? customerID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimates = estimateRepo.ListAll();

            if (customerID > 0)
                estimates.Estimates = estimates.Estimates.Where(k => k.LeadID == customerID).ToList();

            TempData["Estimates"] = estimates;
            return View("EstimateIndex");
        }

        private string GetRoofType(int roofTypeId)
        {
            var connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                const string query = "SELECT [Description] FROM RoofType WHERE [Active] = 1 AND ID = @ID";
                string roofType = sqlConnection.Query<string>(query, new { ID = roofTypeId }).SingleOrDefault();

                return roofType;
            }
        }

        public ActionResult GetEstimates(jQueryDataTableParamModel param, int? customerID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            EstimateDTO estimateDTO = (EstimateDTO)TempData["Estimates"];
            if (estimateDTO == null)
                estimateDTO = estimateRepo.ListAll();

            var customerKeyValue = estimateDTO.Customers.Union(estimateDTO.Leads).OrderBy(k => k.FirstName).ToDictionary(k => k.ID);
            var statusKeyValue = estimateDTO.Status.ToDictionary(k => k.ID);

            string[][] aaData = null;

            if (customerID != null && customerID > 0)
            {
                aaData = (from p in estimateDTO.Estimates.Where(p => p.LeadID == customerID)
                          let roofType = p.RoofType > 0 ? GetRoofType(p.RoofType) : ""
                          let status = statusKeyValue.ContainsKey(p.Status) ? statusKeyValue[p.Status].Description : ""
                          let createdDate = (p.CreatedDate == null || p.CreatedDate == DateTime.MinValue) ? string.Empty : p.CreatedDate.ToShortDateString()
                          select new string[] {
                              "<span class='edit-customer edit-estimate' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.ID.ToString(),
                              roofType,
                              p.Name,
                              createdDate,
                              p.ContractPrice.ToString("N",new CultureInfo("en-US")),
                              status,
                              "<span class='delete-customer delete-estimate' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray();
            }
            else
            {
                aaData = (from p in estimateDTO.Estimates
                          let roofType = p.RoofType > 0 ? GetRoofType(p.RoofType) : string.Empty
                          let customer = customerKeyValue.ContainsKey(p.LeadID) ? customerKeyValue[p.LeadID].FirstName + " " + customerKeyValue[p.LeadID].LastName : string.Empty
                          let status = statusKeyValue.ContainsKey(p.Status) ? statusKeyValue[p.Status].Description : string.Empty
                          let createdDate = (p.CreatedDate == null || p.CreatedDate == DateTime.MinValue) ? string.Empty : p.CreatedDate.ToShortDateString()
                          select new string[] {
                              "<span class='edit-customer edit-estimate' data-val=" + p.ID.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>",
                              p.ID.ToString(),
                              p.Name,
                              roofType,
                              customer,
                              createdDate,
                              p.ContractPrice.ToString("N",new CultureInfo("en-US")),
                              status,
                              "<span class='delete-customer delete-estimate' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray();
            }

            return Json(new
            {
                sEcho = param.sEcho,
                aaData,
                keyValue = new
                {
                    customers = estimateDTO.Customers.OrderBy(p => p.FirstName).ToList(),
                    status = estimateDTO.Status,
                    roofTypes = estimateDTO.RoofTypes
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
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            Estimate estimate = new Estimate();
            IList<EstimateItem> estimateItems = new List<EstimateItem>();

            var estimateDTO = estimateRepo.Select(id);

            if (id != 0)
            {
                estimate = estimateDTO.Estimates.First();
                estimateItems = (from p in estimateDTO.EstimateItems join a in estimateDTO.Assemblies on p.ItemAssemblyID equals a.Id select p).ToList();
            }


            var unitOfMeasure = estimateDTO.UnitOfMeasure.ToDictionary(k => k.Id);

            return Json(new
            {
                estimate = estimate,
                estimateItems = estimateItems,
                keyValue = new
                {
                    leads = estimateDTO.Leads.OrderBy(p => p.FirstName).Select(k => new { ID = k.ID, Description = k.FirstName + " " + k.LastName }).ToList(),
                    customers = estimateDTO.Customers.OrderBy(p => p.FirstName).Select(k => new { ID = k.ID, Description = k.FirstName + " " + k.LastName }).ToList(),
                    estimateForLead = estimateDTO.Leads.Any(l => l.ID == estimate.LeadID),
                    address = estimateDTO.CustomerAddress.Select(k => new { ID = k.ID, CustomerID = k.LeadID, Description = k.Address1 + " " + k.Address2 }).ToList(),
                    status = estimateDTO.Status.Where(k => k.ID == 1 || k.ID == 2 || k.ID == 3 || k.ID == 6),
                    items = estimateDTO.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        Code = k.Code,
                        Name = k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId) ? unitOfMeasure[k.UnitOfMeasureId].Name : string.Empty,
                        Price = k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).ToList(),
                    assemblyCategories = estimateDTO.Assemblies.Select(k => new { ID = k.Code, Description = k.Code }).Distinct().OrderBy(p => p.Description).ToList(),
                    assemblies = estimateDTO.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId) ? unitOfMeasure[k.UnitOfMeasureId].Name : string.Empty,
                        Price = k.TotalRetailCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost
                    }).ToList(),
                    roofTypes = estimateDTO.RoofTypes.Select(k => new { ID = k.ID, Description = k.Description }).OrderBy(p => p.Description).ToList()
                }
        });
        }
        [HttpPost]
        public ActionResult Save(EstimateData estimateData, bool updateClicked)
        {
            string message = string.Empty;
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimate = estimateData.Estimate;
            var estimateItem = estimateData.EstimateItem;
            int ID = estimate.ID;
            foreach(EstimateItem estItem in estimateItem)
            {
                if(string.IsNullOrEmpty(estItem.UOMs))
                {
                    estItem.UOMs = GetAssemblyUOMs(estItem.ItemAssemblyID);
                }
            }
            if (estimate.ID == 0)
            {
                ID = estimateRepo.Create(estimate, estimateItem);
                message = "Record successfully inserted!";
            }
            else
            {
                estimateRepo.Edit(estimate, estimateItem);
                message = "Record successfully updated!";
            }
            if(updateClicked)
            {
                return Json(new
                {
                    ID = ID,
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteEstimate(int id)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            estimateRepo.Delete(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, string description, string uploaddatetime, int estimateID, int id)
        {
            byte[] data = null;
            string fileName = string.Empty;

            if (file != null)
            {
                MemoryStream target = new MemoryStream();
                file.InputStream.CopyTo(target);
                data = target.ToArray();
                fileName = file.FileName;
            }

            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            EstimateDocument e = new EstimateDocument
            {
                ID = id,
                EstimateID = estimateID,
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
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            var estimateDocument = estimateRepo.GetEstimateDocument(id);
            var b = estimateDocument.Text;
            var filename = estimateDocument.Name;
            string contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");


            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        public ActionResult EstimateDocument(int estimateID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDocuments = estimateRepo.GetEstimateDocuments(estimateID);
            if (estimateID == 0)
                estimateDocuments.Clear();
            return Json(new
            {
                aaData = (from p in estimateDocuments
                          select new string[] {
                               "<span class='open-estimate-doc' data-val=" + p.ID.ToString() + ">"+p.Name+"</span>", 
                                p.Description,
                                (p.UploadDateTime == null ? "" : Convert.ToDateTime(p.UploadDateTime).ToString("MM/dd/yyyy HH:mm")),
                    //"<span class='link edit-estimate-doc' data-val=" + p.ID.ToString() + ">Edit</span>", 
                    "<span class='delete-estimate-doc' data-val=" + p.ID.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>" }).ToArray()

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteEstimateDocument(int estimateDocumentID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var isDeleted = estimateRepo.DeleteEstimateDocument(estimateDocumentID);
            return Json(new { isDeleted = isDeleted });
        }

        public string GetAssemblyUOMs(int assemblyID)
        {
            IAssemblyManagementRepository assemblyRepo = ObjectFactory.GetInstance<IAssemblyManagementRepository>();
            var assembly = assemblyRepo.GetAssembly(assemblyID).assembly;
            IItemManagementRepository itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            var unitOfMeasure = itemRepo.GetUnitOfMeasures().ToDictionary(k => k.Id, k => k.Name);
            return assembly.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(assembly.UnitOfMeasureId) ? unitOfMeasure[assembly.UnitOfMeasureId] : string.Empty; ;
        }

        public ActionResult GetAssemblyUOM(int assemblyID)
        {
            return Json(new { UOMs = GetAssemblyUOMs(assemblyID) });
        }

        public FileContentResult CreateProposalDocument(int estimateID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();

            //return Json(new { documentCreate = true});

            var b = estimateRepo.CreateProposalDocument(estimateID);
            var filename = "Proposal.docx";
            string contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
    }
}
