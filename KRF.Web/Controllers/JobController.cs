using KRF.Core.DTO.Job;
using KRF.Core.DTO.Sales;
using KRF.Core.Entities.Customer;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Master;
using KRF.Core.Enums;
using KRF.Core.Entities.ValueList;
using KRF.Core.Repository;
using KRF.Web.Models;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KRF.Web.Controllers
{
    [CustomActionFilter.CustomActionFilter]
    public class JobController : BaseController
    {
        #region "Job Listing"
        //
        // GET: /Fleet/
        public ActionResult Index()
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var jobs = jobRepo.GetJobs();
            TempData["Jobs"] = jobs;
            return View();
        }
        /// <summary>
        /// Get Job List
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetJobs(jQueryDataTableParamModel param)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool isAccessible = Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId, (int)PageType.Job,(int)PermissionType.Edit);
            JobDTO jobDTO = (JobDTO)TempData["Jobs"];
            if (jobDTO == null)
                jobDTO = jobRepo.GetJobs();

            var leads = jobDTO.Leads;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in jobDTO.Jobs
                          select new string[] {
                              (p.Status == false ? "<span class='edit-job' data-val=" + p.Id.ToString() + "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>" : ""),
                              p.JobCode.ToString(),
                              p.Title,
                              leads.FirstOrDefault(c => c.ID == p.LeadID).FirstName + " " + leads.FirstOrDefault(c => c.ID == p.LeadID).LastName,
                              p.LeadID.ToString(),
                              (p.StartDate != null ? Convert.ToDateTime(p.StartDate).ToShortDateString() : ""),
                              (p.EndDate != null ? Convert.ToDateTime(p.EndDate).ToShortDateString() : ""),
                              "<span>" + jobDTO.JobStatus.Where(s => s.ID == p.JobStatusID).FirstOrDefault().Description +"</span>",
                              //"<input type='checkbox' class='complete-job' data-val=" + p.Id.ToString() + " "+ (p.Status == true ? "checked disabled": (isAccessible ? "" : " disabled")) +"></input>",
                              //,"<span class='delete-fleet' data-val=" + p.Id.ToString() + "><ul><li class='delete'><a href='#non'>Delete</a></li></ul></span>"
                          }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Job Add"
        /// <summary>
        /// Get Add View
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Add(int ID = 0)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            ViewBag.ID = ID;
            return View();
        }
        #endregion

        #region "Job Information/Job Summary"
        /// <summary>
        /// Get Job Detail by JobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobInformation(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var jobDTO = jobRepo.GetJobDetail(jobID);
            var customers = jobDTO.Leads.Where(k => k.LeadStage == (int)LeadStageType.Customer).Select(k => new { ID = k.ID, Description = (k.FirstName + " " + k.LastName), Contact = k.LeadName }).OrderBy(k => k.Description).ToList();
            var leads = jobDTO.Leads.Where(k => k.LeadStage == (int)LeadStageType.Lead).Where(p => !string.IsNullOrEmpty(p.LeadName)).Select(k => new { ID = k.ID, Description = (k.LeadName ?? "") }).ToList();
            var job = jobDTO.Jobs[0];
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            KRF.Core.DTO.Sales.LeadDTO leadData = leadRepo.GetLead(job != null ? job.LeadID : 0);
            var customerAddress = jobDTO.CustomerAddress.FirstOrDefault(c => c.LeadID == (job != null ? job.LeadID : 0) && c.ID == (job != null ? job.JobAddressID : 0));
            IEmployeeManagementRepository employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            KRF.Core.DTO.Employee.EmployeeDTO empDTO = employeeRepo.GetEmployes();
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            KRF.Core.DTO.Master.CrewDTO crewDTO = crewRepo.GetCrews();
            List<CrewEmpDetails> lstCrewEmp = new List<CrewEmpDetails>();

            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            EstimateDTO estimateDTO = estimateRepo.ListAll();
            foreach (var crew in crewDTO.Crews.Where(c => c.Active == true))
            {
                lstCrewEmp.Add(new CrewEmpDetails() { Id = crew.CrewID, Name = crew.CrewName, Type = "C" });
            }
            foreach (var emp in empDTO.Employees.Where(e => e.Status == true))
            {
                lstCrewEmp.Add(new CrewEmpDetails() { Id = emp.EmpId, Name = emp.FirstName + " " + emp.LastName, Type = "E" });
            }

            List<ItemAssemblies> lstItemAssemblies = new List<ItemAssemblies>();
            //26-Aug-2015 - Need to show only Assembly in items
            //foreach (var item in jobDTO.Items)
            //{
            //    lstItemAssemblies.Add(new ItemAssemblies() { Id = item.Id, Name = item.Name, Code = item.Code, Type = "0" });
            //}
            foreach (var assembly in jobDTO.Assemblies)
            {
                lstItemAssemblies.Add(new ItemAssemblies() { Id = assembly.Id, Name = assembly.AssemblyName, Code = assembly.Code, Type = "1" });
            }

            var unitOfMeasure = jobDTO.UnitOfMeasures.ToDictionary(k => k.Id);

            var addrList = jobDTO.CustomerAddress.Select(k => new { ID = k.ID, CustomerID = k.LeadID, Description = k.Address1 + " " + k.Address2 }).Where(k => k.Description.Trim().Length > 0).OrderBy(k => k.Description).ToList();

            return Json(new
            {
                job = job,
                keyValue = new
                {
                    customers = customers.OrderBy(c => c.Description),
                    leads = leads.OrderBy(l => l.Description),
                    addresses = addrList,
                    custoAddresses = jobDTO.CustomerAddress.ToList(),
                    cities = jobDTO.Cities,
                    states = jobDTO.States,
                    crewEmps = lstCrewEmp.Select(k => new { ID = k.Id + "~" + k.Type, Description = k.Name, Type = k.Type }).OrderBy(p => p.Description),
                    jobAssigments = MapJobAssignments(jobDTO.JobAssignments, empDTO.Employees, crewDTO.Crews),
                    //tasks = jobDTO.JobTasks.OrderByDescending(t => t.TaskID),
                    estimates = estimateDTO.Estimates.Where(e => e.Status == (int)EstimateStatusType.Complete).Select(k => new { ID = k.ID, JobAddressID = k.JobAddressID, Description = k.Name }).OrderBy(k => k.Description),
                    //POs = jobDTO.JobPOs.OrderBy(p=>p.POID),
                    //COs = jobDTO.JobCOs.OrderBy(p=>p.COID),
                    itemAssemblies = lstItemAssemblies.Select(k => new { ID = k.Id.ToString() + "," + k.Type, Description = k.Code + " - " + k.Name, Type = k.Type }).OrderBy(p => p.Description),
                    items = jobDTO.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        Code = k.Code,
                        Name = k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId) ? unitOfMeasure[k.UnitOfMeasureId].Name : string.Empty,
                        Price = k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = jobDTO.Assemblies.Select(k => new { ID = k.Id, Description = k.Code + " " + k.AssemblyName, Code = k.Code, Name = k.AssemblyName, Price = k.TotalRetailCost, MaterialCost = k.MaterialCost, LaborCost = k.LaborCost }).OrderBy(k => k.Description).ToList()
                }
            });
        }
        /// <summary>
        /// Get Job Summary by JobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobSummary(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var jobDTO = jobRepo.GetJobDetail(jobID);
            var customers = jobDTO.Leads.Where(k => k.LeadStage == (int)LeadStageType.Customer).Select(k => new { ID = k.ID, Description = (k.FirstName + " " + k.LastName), Contact = k.LeadName }).OrderBy(k => k.Description).ToList();
            var job = jobDTO.Jobs[0];
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            KRF.Core.DTO.Sales.LeadDTO leadData = leadRepo.GetLead(job != null ? job.LeadID : 0);
            var lead = leadData.Leads[0];
            var jobAddress = jobDTO.CustomerAddress.FirstOrDefault(c => c.LeadID == job.LeadID && c.ID == job.JobAddressID);
            var city = jobDTO.Cities.FirstOrDefault(c => c.ID == jobAddress.City);
            var state = jobDTO.States.FirstOrDefault(c => c.ID == jobAddress.State);
            IEmployeeManagementRepository empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var employDTO = empRepo.GetEmployes();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            EstimateDTO estimateDTO = estimateRepo.ListAll();
            KRF.Core.Entities.Sales.Estimate estimate = estimateDTO.Estimates.FirstOrDefault(p => p.ID == job.EstimateID);
            List<JobCO> jobCOs = jobRepo.GetJobCOs(job.Id);
            List<JobInvoice> jobINVs = jobRepo.GetJobInvoices(job.Id);
            decimal totalPrice= (estimate != null ? estimate.TotalCost : 0) + jobCOs.Where(p=>p.Active == true).Sum(p => p.TotalAmount);
            return Json(new
            {
                job = job,
                keyValue = new
                {
                    statuses = estimateDTO.Status.Where(k => k.ID == 1 || k.ID == 2 || k.ID == 6),
                    customerName = lead.FirstName + " " + lead.LastName,
                    jobSiteAddress = jobAddress.Address1.Trim() + ", " + jobAddress.Address2.Trim() + ", " + (city != null ? city.Description.Trim() : "") + " " + state.Abbreviation.Trim() + ", " + jobAddress.ZipCode.Trim(),
                    contactPerson = lead.LeadName,
                    salesPerson = (employDTO.Employees.Where(p => p.EmpId == lead.AssignedTo).FirstOrDefault() != null ? employDTO.Employees.Where(p => p.EmpId == lead.AssignedTo).FirstOrDefault().FirstName + " " + employDTO.Employees.Where(p => p.EmpId == lead.AssignedTo).FirstOrDefault().LastName : ""),
                    contractPrice = Convert.ToString(estimate != null ? estimate.ContractPrice : 0),
                    totalChangeOrder = jobCOs.Where(p => p.Active == true).Sum(p => p.TotalAmount),
                    totalPrice = totalPrice,
                    totalPaymentReceived = jobINVs.Where(p => p.Active == true).Sum(p => p.TotalAmount),
                    totalOutstanding = (jobINVs.Where(p => p.Active == true).Sum(p => p.TotalAmount) - totalPrice)
                }
            });
        }
        /// <summary>
        /// Get Building Information by leadID
        /// </summary>
        /// <param name="leadID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBuildingInformation(int leadID)
        {
            ILeadManagementRepository leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            KRF.Core.DTO.Sales.LeadDTO leadData = leadRepo.GetLeads(x => true);
            KRF.Core.DTO.Sales.LeadDTO leads = leadRepo.GetLead(leadID);
            KRF.Core.Entities.Sales.Lead lead = leads.Leads[0];
            return Json(new
            {
                buildingInformation = new
                {
                    ProjectType = lead != null ? (leadData.ProjectTypes.FirstOrDefault(p => p.ID == lead.ProjectType) != null ? leadData.ProjectTypes.FirstOrDefault(p => p.ID == lead.ProjectType).Description : "") : "",
                    RoofType = lead != null ? (leadData.RoofTypes.FirstOrDefault(p => p.ID == lead.RoofType) != null ? leadData.RoofTypes.FirstOrDefault(p => p.ID == lead.RoofType).Description : "") : "",
                    AgeOfRoof = lead != null ? (leadData.RoofAgeList.FirstOrDefault(p => p.ID == lead.RoofAge) != null ? leadData.RoofAgeList.FirstOrDefault(p => p.ID == lead.RoofAge).Description : "") : "",
                    BuildingStories = lead != null ? (leadData.BuildingStoriesList.FirstOrDefault(p => p.ID == lead.NumberOfStories) != null ? leadData.BuildingStoriesList.FirstOrDefault(p => p.ID == lead.NumberOfStories).Description : "") : "",
                    ProjectExpectedToBegin = lead != null ? (leadData.ProjectStartTimelines.FirstOrDefault(p => p.ID == lead.ProjectStartTimeline) != null ? leadData.ProjectStartTimelines.FirstOrDefault(p => p.ID == lead.ProjectStartTimeline).Description : "") : "",
                    AdditionalInformation = lead != null ? (lead.AdditionalInfo ?? "") : ""
                }
            });
        }
        /// <summary>
        /// Save Job Information
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobSummary(JobData jobData)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                var job = jobData.Job;

                if (job.Id > 0)
                {
                    if (job.JobStatusID == (int)KRF.Core.Entities.MISC.Status.Complete)
                    {
                        job.Status = true;
                    }
                    jobRepo.EditJobSummary(job);
                    message = "Job Summary updated successfully!.";
                }
                hasSaved = true;
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Job Summary could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message });
        }
        /// <summary>
        /// Save Job Information
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobInformation(JobData jobData)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                var job = jobData.Job;

                if (job.Id == 0)
                {
                    job.CreatedOn = DateTime.Now;
                    job.DateUpdated = null;
                    job.Status = false;
                    jobRepo.CreateJobInformation(job);
                }
                else
                {
                    job.DateUpdated = DateTime.Now;
                    jobRepo.EditJobInformation(job);
                    message = "Job Information data updated successfully!.";
                }
                hasSaved = true;
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Job Information could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message });
        }
        #endregion

        #region "Job Assignments"
        public List<JobAssignmentDetails> MapJobAssignments(IList<JobAssignment> jobAssignments, IList<Employee> employees, IList<Crew> crews)
        {
            List<JobAssignmentDetails> jobAssignmentDetails = new List<JobAssignmentDetails>();
            foreach (JobAssignment jobAsgmt in jobAssignments)
            {
                string Name = string.Empty;
                switch (jobAsgmt.Type)
                {
                    case "E":
                        Employee emp = employees.FirstOrDefault(e => e.EmpId == jobAsgmt.ObjectPKID);
                        if (emp != null)
                        {
                            Name = emp.FirstName + " " + emp.LastName;
                        }
                        break;
                    case "C":
                        Crew crew = crews.FirstOrDefault(c => c.CrewID == jobAsgmt.ObjectPKID);
                        if (crew != null)
                        {
                            Name = crew.CrewName;
                        }
                        break;
                }
                jobAssignmentDetails.Add(new JobAssignmentDetails()
                {
                    JobID = jobAsgmt.JobID,
                    JobAssignmentID = jobAsgmt.JobAssignmentID,
                    ObjectPKID = jobAsgmt.ObjectPKID,
                    Type = jobAsgmt.Type,
                    Name = Name,
                    FromDate = jobAsgmt.FromDate.Value.ToShortDateString(),
                    ToDate = jobAsgmt.ToDate.Value.ToShortDateString()
                });
            }
            return jobAssignmentDetails;
        }
        /// <summary>
        /// Get Building Information
        /// </summary>
        /// <param name="leadID"></param>
        /// <returns></returns>
        public JsonResult SaveJobAssignment(JobData jobData)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                if (jobRepo.EditJobAssignment(jobData.Job, jobData.JobAssignments))
                {
                    hasSaved = true;
                    message = "Job Assignment data updated successfully!.";
                }
                else
                {
                    hasSaved = false;
                    message = "Job Assignment could not be updated.";
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Job Assignment could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message });
        }
        /// <summary>
        /// Get Job Milestone list by jobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobTaskList(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var taskList = jobRepo.GetJobTasks(jobID);
            return Json(new
            {
                tasks = taskList.OrderByDescending(t => t.TaskID)
            });
        }
        #endregion

        #region "Job Milestone"
        /// <summary>
        /// Save Job Milestone
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobMilestone(JobData jobData)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                if (jobData.JobTask.TaskID > 0)
                {
                    if (jobRepo.EditJobTask(jobData.JobTask))
                    {
                        hasSaved = true;
                        message = "Task updated successfully!.";
                    }
                    else
                    {
                        hasSaved = false;
                        message = "Task could not be updated.";
                    }
                }
                else
                {
                    if (jobRepo.CreateJobTask(jobData.JobTask) > 0)
                    {
                        hasSaved = true;
                        message = "Task created successfully!.";
                    }
                    else
                    {
                        hasSaved = false;
                        message = "Task could not be created.";
                    }
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Task could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message, tasks = jobRepo.GetJobTasks(jobData.JobTask.JobID).OrderByDescending(t => t.TaskID) });
        }
        /// <summary>
        /// Update Task status
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        public JsonResult UpdateTaskStatus(int taskID, bool isCompleted)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                if (taskID > 0)
                {
                    if (jobRepo.UpdateTaskStatus(taskID, isCompleted))
                    {
                        hasSaved = true;
                        message = "Task status updated successfully!.";
                    }
                    else
                    {
                        hasSaved = false;
                        message = "Task could not be updated.";
                    }
                }
                else
                {
                    hasSaved = false;
                    message = "Task status could not be updated.";
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Task status could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message });
        }
        /// <summary>
        /// Calculate estimated labour hours and job end date
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="jobStartDate"></param>
        /// <param name="averageWorkingHour"></param>
        /// <param name="jobAssigments"></param>
        /// <returns></returns>
        public JsonResult CalculateEstimatedLaborHoursAndJobEndDate(int jobID, DateTime jobStartDate, int averageWorkingHour, List<JobAssignment> jobAssigments)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            decimal estimatedLaborHours = 0;
            bool result = true;
            string jobEndDate = string.Empty;
            try
            {
                estimatedLaborHours = jobRepo.CalculateEstimatedLaborCost(jobID, jobStartDate, jobAssigments);
                DateTime endDate = jobRepo.CalculateJobEndDate(jobStartDate, estimatedLaborHours, averageWorkingHour);
                jobEndDate = endDate.ToShortDateString();
            }
            catch (Exception ex)
            {
                result = false;
            }
            return Json(new { result = result, estimatedHour = Math.Round(estimatedLaborHours, 2), jobEndDate = jobEndDate });
        }
        /// <summary>
        /// Mark job status as complete
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleJobStatus(int jobID, bool tobeEnabled)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasDeleted = jobRepo.ToggleJobStatus(jobID, tobeEnabled);

            return Json(new { hasDeleted = hasDeleted });
        }
        /// <summary>
        /// Get estimate items
        /// </summary>
        /// <param name="estimateID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEstimateItems(int estimateID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            IItemManagementRepository itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<KRF.Core.Entities.Sales.EstimateItem> estimateItems = new List<KRF.Core.Entities.Sales.EstimateItem>();
            var estimateDTO = estimateRepo.Select(estimateID);
            if (estimateID != 0)
            {
                estimateItems = estimateDTO.EstimateItems;
            }
            var unitOfMeasure = estimateDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            var productDTO = itemRepo.GetInventory();
            List<KRF.Core.Entities.Product.AssemblyItem> assemblyItems = estimateDTO.AssemblyItems.ToList();

            var estimates =
            (from p in estimateItems
             join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
             join i in estimateDTO.Items on q.ItemId equals i.Id
             where i.ItemTypeId == (int)Core.Enums.ItemType.Material
             select new { p.ID, p.EstimateID, p.ItemAssemblyID, p.ItemAssemblyType, p.MaterialCost, p.LaborCost, q.RetailCost, ItemNames = i.Name, ItemID = i.Id, p.Quantity }).Select(k => new
                {
                    ID = k.ID,
                    EstimateID = k.EstimateID,
                    ItemAssemblyID = k.ItemAssemblyID,
                    ItemAssemblyType = k.ItemAssemblyType,
                    Price = k.RetailCost, //Price = k.Price,
                    //Quantity = productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault() != null ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty < k.Quantity) ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty - k.Quantity) * -1) : 0) : k.Quantity, //2-Sep-2015 - As discussed currently disabling this logic and showing estimate quantity in PO. We may need to enable this logic at later point of time
                    Quantity = k.Quantity,
                    //Cost = k.RetailCost * (productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault() != null ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty < k.Quantity) ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty - k.Quantity) * -1) : 0) : k.Quantity),
                    Cost = k.RetailCost * k.Quantity,
                    LaborCost = k.RetailCost,
                    MaterialCost = k.RetailCost,
                    //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name)),
                    ItemNames = k.ItemNames,
                    ItemID = k.ItemID,
                    COID = 0
                });

            return Json(new
            {
                estimateItems = estimates.Where(k => string.IsNullOrEmpty(k.ItemNames) == false),
                originalEstimateItems = estimateItems,
                keyValue = new
                {
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
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estimateDTO.Assemblies
                                  join q in assemblyItems on p.Id equals q.AssemblyId
                                  join i in estimateDTO.Items on q.ItemId equals i.Id
                                  where i.ItemTypeId == (int)Core.Enums.ItemType.Material
                                  select new { p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, q.RetailCost, ItemName = i.Name, ItemID = i.Id })
                   .Select(k => new
                   {
                       ID = k.Id,
                       Description = k.Code + " " + k.AssemblyName,
                       Code = k.Code,
                       Name = k.AssemblyName,
                       Price = k.MaterialCost,
                       MaterialCost = k.RetailCost,
                       LaborCost = k.RetailCost,
                       //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                       ItemNames = k.ItemName,
                       ItemID = k.ItemID
                   }).OrderBy(k => k.Description).ToList()
                }
            });
        }
        [HttpPost]
        public JsonResult GetCOItems(int COID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            IItemManagementRepository itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<KRF.Core.Entities.Sales.EstimateItem> estimateItems = new List<KRF.Core.Entities.Sales.EstimateItem>();
            var estimateDTO = estimateRepo.Select(0);
            List<KRF.Core.Entities.Product.AssemblyItem> assemblyItems = estimateDTO.AssemblyItems.ToList();
            JobDTO jobDTO = jobRepo.GetJobCO(COID);
            var productDTO = itemRepo.GetInventory();
            var unitOfMeasure = estimateDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            List<POCOEstimateItems> pocoItems = (from p in jobDTO.COEstimateItems
                                                 join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
                                                 join i in estimateDTO.Items on q.ItemId equals i.Id
                                                 where i.ItemTypeId == (int)Core.Enums.ItemType.Material
                                                 select new { p.ID, p.COID, p.ItemAssemblyID, p.ItemAssemblyType, q.RetailCost, ItemNames = i.Name, ItemID = i.Id, p.Quantity })
                                                 .Where(p => p.ItemAssemblyType == 1).Select(k => new POCOEstimateItems
                {
                    ID = k.ID,
                    EstimateID = 0,
                    ItemAssemblyID = k.ItemAssemblyID,
                    ItemAssemblyType = k.ItemAssemblyType,
                    Price = k.RetailCost,
                    //Quantity = productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault() != null ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty < k.Quantity) ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty - k.Quantity) * -1) : 0) : k.Quantity, //2-Sep-2015 - As discussed currently disabling this logic and showing estimate quantity in PO. We may need to enable this logic at later point of time
                    Quantity = k.Quantity,
                    //Cost = k.RetailCost * (productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault() != null ? (((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty < k.Quantity) ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty - k.Quantity) * -1) : 0)) : k.Quantity),
                    Cost = k.RetailCost * k.Quantity,
                    LaborCost = 0,
                    MaterialCost = 0,
                    ItemNames = k.ItemNames,
                    COID = k.COID,
                    ItemID = k.ItemID
                }).ToList();

            var items = from p in jobDTO.COEstimateItems.Where(p => p.ItemAssemblyType == 0)
                        join q in estimateDTO.Items on p.ItemAssemblyID equals q.Id
                        where q.ItemTypeId == (int)Core.Enums.ItemType.Material
                        select new POCOEstimateItems
                                                {
                                                    ID = p.ID,
                                                    EstimateID = 0,
                                                    ItemAssemblyID = p.ItemAssemblyID,
                                                    ItemAssemblyType = p.ItemAssemblyType,
                                                    Price = p.Price,
                                                    Quantity = p.Quantity,
                                                    Cost = p.Price * p.Quantity,
                                                    LaborCost = 0,
                                                    MaterialCost = 0,
                                                    ItemNames = "",
                                                    COID = p.COID
                                                };
            pocoItems = pocoItems.Where(k => string.IsNullOrEmpty(k.ItemNames) == false).ToList();
            pocoItems.AddRange(items);

            return Json(new
            {
                COItems = pocoItems,
                keyValue = new
                {
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
                    }).OrderBy(p => p.Description).ToList(),
                    assemblies = (from p in estimateDTO.Assemblies
                                  join q in assemblyItems on p.Id equals q.AssemblyId
                                  join i in estimateDTO.Items on q.ItemId equals i.Id
                                  where i.ItemTypeId == (int)Core.Enums.ItemType.Material
                                  select new { p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name, ItemID = i.Id })
                    .Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.MaterialCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost,
                        //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                        ItemNames = k.ItemName,
                        ItemID = k.ItemID
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }
        #endregion

        #region "Job PO"
        /// <summary>
        /// Get Job Purchase order list by jobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobPOList(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            IVendorManagementRepository vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            var poList = jobRepo.GetJobPOs(jobID);
            var estimateDTO = estRepo.ListAll();
            var vendorDTO = vendorRepo.ListAllVendors();
            return Json(new
            {
                keyValue = new
                {
                    POs = poList.OrderByDescending(p => p.POID),
                    estimates = estimateDTO.Estimates.Where(e => e.Status == (int)EstimateStatusType.Complete).Select(k => new { ID = k.ID, JobAddressID = k.JobAddressID, Description = k.Name }).OrderBy(k => k.Description),
                    cos = jobRepo.GetJobCOs(jobID).Where(e => e.Active == true).Select(k => new { ID = k.COID, Description = k.Description }).OrderBy(k => k.Description),
                    vendors = vendorDTO.Vendors.Select(k => new { ID = k.ID, Description = k.VendorName, Address = k.VendorAddress, Phone = k.Phone, Fax = k.Fax, Cell = k.Cell, Email = k.Email, Active = k.Active }).OrderBy(p => p.Description)
                }
            });
        }
        public JsonResult ToggleJobPOStatus(int poID, bool active)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasDeleted = jobRepo.ToggleJobPOStatus(poID, active);
            return Json(new { hasDeleted = hasDeleted });
        }
        /// <summary>
        /// Save Job PO
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobPO(JobData jobData)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                jobData.JobPO.COIDs = jobData.JobPO.COIDs.Replace("~", ",");
                List<JobPO> lstJobPO = jobRepo.GetJobPOs(jobData.JobPO.JobID);
                if (jobData.JobPO.POID > 0)
                {
                    if (lstJobPO.Any() && lstJobPO.Find(p => p.POCode == jobData.JobPO.POCode && p.POID != jobData.JobPO.POID) != null)
                    {
                        hasSaved = false;
                        message = "PO Code already exists.";
                    }
                    else
                    {
                        if (jobRepo.EditJobPO(jobData.JobPO, jobData.POEstimateItems))
                        {
                            hasSaved = true;
                            message = "PO updated successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "PO could not be updated.";
                        }
                    }
                }
                else
                {
                    if (lstJobPO.Any() && lstJobPO.Find(p => p.POCode == jobData.JobPO.POCode) != null)
                    {
                        hasSaved = false;
                        message = "PO Code already exists.";
                    }
                    else
                    {
                        if (jobRepo.CreateJobPO(jobData.JobPO, jobData.POEstimateItems) > 0)
                        {
                            hasSaved = true;
                            message = "PO created successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "PO could not be created.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "PO could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message, POs = jobRepo.GetJobPOs(jobData.JobPO.JobID).OrderByDescending(t => t.POID) });
        }
        /// <summary>
        /// Get Job PO Detail
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        public JsonResult GetJobPODetail(int poID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            EstimateDTO estimateDTO = estimateRepo.ListAll();
            JobDTO jobDTO = jobRepo.GetJobPO(poID);
            var estDTO = estimateRepo.Select(jobDTO.JobPO.EstimateID ?? 0);
            var unitOfMeasure = estDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            List<Core.Entities.Product.AssemblyItem> assemblyItems = estDTO.AssemblyItems.ToList();
            return Json(new
            {
                JobPO = jobDTO.JobPO,
                keyValue = new
                {
                    originalEstimateItems = estDTO.EstimateItems,
                    estimates = estimateDTO.Estimates.Where(e => e.Status == (int)EstimateStatusType.Complete).Select(k => new { ID = k.ID, JobAddressID = k.JobAddressID, Description = k.Name }).OrderBy(k => k.Description),
                    items = estDTO.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        Code = k.Code,
                        Name = k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId) ? unitOfMeasure[k.UnitOfMeasureId].Name : string.Empty,
                        Price = k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estDTO.Assemblies
                                  join q in assemblyItems on p.Id equals q.AssemblyId
                                  join i in estDTO.Items on q.ItemId equals i.Id
                                  where i.ItemTypeId == (int)Core.Enums.ItemType.Material
                                  select new { p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name, ItemID = i.Id })
                    .Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.MaterialCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost,
                        //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                        ItemNames = k.ItemName,
                        ItemID = k.ItemID
                    }).OrderBy(k => k.Description).ToList()
                },
                estimateItems = (from p in jobDTO.POEstimateItems
                                 join q in estDTO.Items on p.ItemID equals q.Id
                                 join a in estDTO.Assemblies on p.ItemAssemblyID equals a.Id
                                 select p).Select(k => new
                {
                    ID = k.ID,
                    POID = k.POID,
                    ItemAssemblyID = k.ItemAssemblyID,
                    ItemAssemblyType = k.ItemAssemblyType,
                    Price = k.Price, //Price = k.Price,
                    Quantity = k.Quantity,
                    Cost = k.Cost,
                    LaborCost = 0,
                    MaterialCost = 0,
                    ItemNames = (k.ItemNames ?? ""),
                    COID = k.COID,
                    ItemID = k.ItemID
                })
            });
        }
        public FileContentResult CreatePODocument(int poID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreatePODocument(poID);
            var filename = "Purchase_Order.docx";
            string contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        #endregion

        #region "Job CO"
        /// <summary>
        /// Get Job Change order list by jobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobCOList(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEmployeeManagementRepository empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var coList = jobRepo.GetJobCOs(jobID);
            var empList = empRepo.GetEmployes();
            return Json(new
            {
                keyValue = new
                {
                    COs = coList.OrderByDescending(p => p.COID),
                    reps = empList.Employees.Where(e => e.Status == true).Select(e => new { ID = e.EmpId, Description = e.FirstName + " " + e.LastName, Email = e.EmailID }).OrderBy(e => e.Description)
                }
            });
        }
        /// <summary>
        /// Togggle Job Changer Order Status
        /// </summary>
        /// <param name="coID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult ToggleJobCOStatus(int coID, bool active)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasDeleted = jobRepo.ToggleJobCOStatus(coID, active);
            return Json(new { hasDeleted = hasDeleted });
        }
        /// <summary>
        /// Save Job CO
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobCO(JobData jobData)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                List<JobCO> lstJobCO = jobRepo.GetJobCOs(jobData.JobCO.JobID);
                if (jobData.JobCO.COID > 0)
                {
                    if (lstJobCO.Any() && lstJobCO.Find(p => p.COCode == jobData.JobCO.COCode && p.COID != jobData.JobCO.COID) != null)
                    {
                        hasSaved = false;
                        message = "CO Code already exists.";
                    }
                    else
                    {
                        if (jobRepo.EditJobCO(jobData.JobCO, jobData.COEstimateItems))
                        {
                            hasSaved = true;
                            message = "CO updated successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "CO could not be updated.";
                        }
                    }
                }
                else
                {
                    if (lstJobCO.Any() && lstJobCO.Find(p => p.COCode == jobData.JobCO.COCode) != null)
                    {
                        hasSaved = false;
                        message = "CO Code already exists.";
                    }
                    else
                    {
                        if (jobRepo.CreateJobCO(jobData.JobCO, jobData.COEstimateItems) > 0)
                        {
                            hasSaved = true;
                            message = "CO created successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "CO could not be created.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "CO could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message, COs = jobRepo.GetJobCOs(jobData.JobCO.JobID).OrderByDescending(t => t.COID) });
        }
        /// <summary>
        /// Get Job CO Detail
        /// </summary>
        /// <param name="coID"></param>
        /// <returns></returns>
        public JsonResult GetJobCODetail(int coID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            EstimateDTO estimateDTO = estimateRepo.ListAll();
            JobDTO jobDTO = jobRepo.GetJobCO(coID);
            var estDTO = estimateRepo.Select(0);
            var unitOfMeasure = estDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            return Json(new
            {
                JobCO = jobDTO.JobCO,
                keyValue = new
                {
                    items = estDTO.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        Code = k.Code,
                        Name = k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId) ? unitOfMeasure[k.UnitOfMeasureId].Name : string.Empty,
                        Price = k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(p => p.Description).ToList(),
                    assemblies = estDTO.Assemblies.Select(k => new { ID = k.Id, Description = k.Code + " " + k.AssemblyName, Code = k.Code, Name = k.AssemblyName, Price = k.TotalRetailCost, MaterialCost = k.MaterialCost, LaborCost = k.LaborCost }).OrderBy(p => p.Description).ToList()
                },
                estimateItems = (from p in jobDTO.COEstimateItems join a in estDTO.Assemblies on p.ItemAssemblyID equals a.Id select p)
            });
        }
        /// <summary>
        /// Create Job Change Order Document
        /// </summary>
        /// <param name="coID"></param>
        /// <returns></returns>
        public FileContentResult CreateCODocument(int coID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreateCODocument(coID);
            var filename = "Change_Order.docx";
            string contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        #endregion

        #region "Job Documents"
        /// <summary>
        /// Get Job Purchase order list by jobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobDocumentList(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var documentList = jobRepo.GetJobDocuments(jobID).OrderByDescending(p => p.ID);
            var documentTypes = jobRepo.GetJobDocumentTypes();
            return Json(new
            {
                keyValue = new
                {
                    documents = documentList.Select(k => new
                    {
                        ID = k.ID,
                        JobID = k.JobID,
                        Name = k.Name,
                        Type = k.Type,
                        TypeName = (documentTypes.Where(p => p.ID.ToString() == k.Type.Trim()).FirstOrDefault() != null ? documentTypes.Where(p => p.ID.ToString() == k.Type.Trim()).FirstOrDefault().DocumentType : ""),
                        Description = k.Description,
                        UploadDateTime = Convert.ToDateTime(k.UploadDateTime).ToString("MM/dd/yyyy HH:mm")
                    }),
                    documentTypes = documentTypes.Select(k => new { ID = k.ID, Description = k.DocumentType })
                }
            });
        }
        /// <summary>
        /// Upload Job Document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="documentTypes"></param>
        /// <param name="jobID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, int documentTypes, int jobID, int id)
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

            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            JobDocument doc = new JobDocument
            {
                ID = id,
                JobID = jobID,
                Name = fileName,
                Description = "",
                Type = documentTypes.ToString(),
                UploadDateTime = DateTime.Now,
                Text = data
            };

            jobRepo.SaveDocument(doc);

            return Json(new
            {
                data = "success"
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Download Job Document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileContentResult Download(int id)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var jobDocument = jobRepo.GetJobDocument(id);
            var b = jobDocument.Text;
            var filename = jobDocument.Name;
            string contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        /// <summary>
        /// Delete job document by jobDocumentID
        /// </summary>
        /// <param name="jobDocumentID"></param>
        /// <returns></returns>
        public ActionResult DeleteJobDocument(int jobDocumentID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var isDeleted = jobRepo.DeleteJobDocument(jobDocumentID);
            return Json(new { isDeleted = isDeleted });
        }
        #endregion

        #region "Job WO"
        /// <summary>
        /// Get Job Work order list by jobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobWOList(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            ICrewManagementRepository crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            IEmployeeManagementRepository empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var woList = jobRepo.GetJobWOs(jobID);
            var estimateDTO = estRepo.ListAll();
            var employDTO = empRepo.GetEmployes();
            List<JobAssignment> jobAssignments = jobRepo.GetJobAssignments(jobID);
            return Json(new
            {
                keyValue = new
                {
                    WOs = woList.Select(k => new
                    {
                        WOID = k.WOID,
                        JobID = k.JobID,
                        WOCode = k.WOCode,
                        WorkWeekEndingDate = k.WorkWeekEndingDate,
                        LeadID = k.LeadID,
                        CrewLeader = (employDTO.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault() != null ? (employDTO.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault().FirstName + " " + employDTO.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault().LastName) : ""),
                        EstimateID = k.EstimateID,
                        TotalAmount = k.TotalAmount,
                        TotalJobBalanceAmount = k.TotalJobBalanceAmount,
                        COIDs = k.COIDs,
                        Active = k.Active
                    }).OrderByDescending(p => p.WOID),
                    estimates = estimateDTO.Estimates.Where(e => e.Status == (int)EstimateStatusType.Complete).Select(k => new { ID = k.ID, JobAddressID = k.JobAddressID, Description = k.Name }).OrderBy(k => k.Description),
                    cos = jobRepo.GetJobCOs(jobID).Where(e => e.Active == true).Select(k => new { ID = k.COID, Description = k.Description }).OrderBy(k => k.Description),
                    crewleads = jobRepo.GetJobAssignmentCrewLeaders(jobID).Select(k => new { ID = k.EmpId, Description = k.LeadName }).OrderBy(k => k.Description)
                }
            });
        }
        public JsonResult ToggleJobWOStatus(int woID, bool active)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasDeleted = jobRepo.ToggleJobWOStatus(woID, active);
            return Json(new { hasDeleted = hasDeleted });
        }
        [HttpPost]
        public JsonResult GetEstimateItemsForWO(int estimateID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            IItemManagementRepository itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<KRF.Core.Entities.Sales.EstimateItem> estimateItems = new List<KRF.Core.Entities.Sales.EstimateItem>();
            var estimateDTO = estimateRepo.Select(estimateID);
            if (estimateID != 0)
            {
                estimateItems = estimateDTO.EstimateItems;
            }
            var unitOfMeasure = estimateDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            var productDTO = itemRepo.GetInventory();
            List<KRF.Core.Entities.Product.AssemblyItem> assemblyItems = estimateDTO.AssemblyItems.ToList();

            var estimates = (from p in estimateItems
                             join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
                             join i in estimateDTO.Items on q.ItemId equals i.Id
                             where i.ItemTypeId == (int)Core.Enums.ItemType.Labor
                             select new { p.ID, p.EstimateID, p.ItemAssemblyID, p.ItemAssemblyType, p.MaterialCost, p.LaborCost, q.RetailCost, ItemNames = i.Name, ItemID = i.Id, p.Quantity })
                             .Select(k => new
                {
                    ID = k.ID,
                    EstimateID = k.EstimateID,
                    ItemAssemblyID = k.ItemAssemblyID,
                    ItemAssemblyType = k.ItemAssemblyType,
                    Budget = k.Quantity,
                    Used = 0,
                    //Rate = ((from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID && q.ItemTypeId == (int)Core.Enums.ItemType.Labor select p.RetailCost).Sum()),
                    Rate = k.RetailCost,
                    Balance = k.Quantity,
                    Amount = 0,
                    LaborCost = k.RetailCost,
                    MaterialCost = k.RetailCost,
                    //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID && q.ItemTypeId == (int)Core.Enums.ItemType.Labor select q.Name)),
                    ItemNames = k.ItemNames,
                    ItemID = k.ItemID,
                    COID = 0
                });

            return Json(new
            {
                estimateItems = estimates.Where(k => string.IsNullOrEmpty(k.ItemNames) == false),
                originalEstimateItems = estimateItems,
                keyValue = new
                {
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
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estimateDTO.Assemblies
                                  join q in assemblyItems on p.Id equals q.AssemblyId
                                  join i in estimateDTO.Items on q.ItemId equals i.Id
                                  where i.ItemTypeId == (int)Core.Enums.ItemType.Labor
                                  select new { p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, q.RetailCost, ItemName = i.Name, ItemID = i.Id })
                    .Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.RetailCost,
                        MaterialCost = k.RetailCost,
                        LaborCost = k.RetailCost,
                        //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                        ItemNames = k.ItemName,
                        ItemID = k.ItemID
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }
        [HttpPost]
        public JsonResult GetCOItemsForWO(int COID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            IItemManagementRepository itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<KRF.Core.Entities.Sales.EstimateItem> estimateItems = new List<KRF.Core.Entities.Sales.EstimateItem>();
            var estimateDTO = estimateRepo.Select(0);
            List<KRF.Core.Entities.Product.AssemblyItem> assemblyItems = estimateDTO.AssemblyItems.ToList();
            JobDTO jobDTO = jobRepo.GetJobCO(COID);
            var productDTO = itemRepo.GetInventory();
            var unitOfMeasure = estimateDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            List<WOCOEstimateItems> wocoItems = (from p in jobDTO.COEstimateItems
                                                 join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
                                                 join i in estimateDTO.Items on q.ItemId equals i.Id
                                                 where i.ItemTypeId == (int)Core.Enums.ItemType.Labor 
                                                 select new { p.ID, p.ItemAssemblyID, p.COID, p.ItemAssemblyType, p.Quantity, q.RetailCost, ItemName = i.Name, ItemID = i.Id }).Select(k => new WOCOEstimateItems
            {
                ID = k.ID,
                EstimateID = 0,
                ItemAssemblyID = k.ItemAssemblyID,
                ItemAssemblyType = k.ItemAssemblyType,
                Budget = k.Quantity,
                Used = 0,
                Rate = k.RetailCost,
                Balance = k.Quantity,
                Amount = 0,
                LaborCost = 0,
                MaterialCost = 0,
                ItemNames = k.ItemName,
                COID = k.COID,
                ItemID = k.ItemID
            }).ToList();

            var items = from p in jobDTO.COEstimateItems.Where(p => p.ItemAssemblyType == 0)
                        join q in estimateDTO.Items on p.ItemAssemblyID equals q.Id
                        where q.ItemTypeId == (int)Core.Enums.ItemType.Labor
                        select new WOCOEstimateItems
                        {
                            ID = p.ID,
                            EstimateID = 0,
                            ItemAssemblyID = p.ItemAssemblyID,
                            ItemAssemblyType = p.ItemAssemblyType,
                            Budget = p.Quantity,
                            Used = 0,
                            Rate = p.Price,
                            Balance = p.Quantity,
                            Amount = 0,
                            LaborCost = 0,
                            MaterialCost = 0,
                            ItemNames = "",
                            COID = p.COID,
                            ItemID = 0
                        };

            wocoItems = wocoItems.Where(k => string.IsNullOrEmpty(k.ItemNames) == false).ToList();
            wocoItems.AddRange(items);

            return Json(new
            {
                COItems = wocoItems,
                keyValue = new
                {
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
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estimateDTO.Assemblies
                                  join q in assemblyItems on p.Id equals q.AssemblyId
                                  join i in estimateDTO.Items on q.ItemId equals i.Id
                                  where i.ItemTypeId == (int)Core.Enums.ItemType.Labor
                                  select new { p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name, ItemID = i.Id })
                    .Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.MaterialCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost,
                        //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                        ItemNames = k.ItemName,
                        ItemID = k.ItemID
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }
        /// <summary>
        /// Save Job WO
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobWO(JobData jobData)
        {
            IEmployeeManagementRepository empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                jobData.JobWO.COIDs = jobData.JobWO.COIDs.Replace("~", ",");
                List<JobWO> lstJobWO = jobRepo.GetJobWOs(jobData.JobWO.JobID);
                if (jobData.JobWO.WOID > 0)
                {
                    if (lstJobWO.Any() && lstJobWO.Find(p => p.WOCode == jobData.JobWO.WOCode && p.WOID != jobData.JobWO.WOID) != null)
                    {
                        hasSaved = false;
                        message = "WO Code already exists.";
                    }
                    else
                    {
                        if (jobRepo.EditJobWO(jobData.JobWO, jobData.WOEstimateItems))
                        {
                            hasSaved = true;
                            message = "WO updated successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "WO could not be updated.";
                        }
                    }
                }
                else
                {
                    if (lstJobWO.Any() && lstJobWO.Find(p => p.WOCode == jobData.JobWO.WOCode) != null)
                    {
                        hasSaved = false;
                        message = "WO Code already exists.";
                    }
                    else
                    {
                        if (jobRepo.CreateJobWO(jobData.JobWO, jobData.WOEstimateItems) > 0)
                        {
                            hasSaved = true;
                            message = "WO created successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "WO could not be created.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "WO could not be updated.";
            }

            var woList = jobRepo.GetJobWOs(jobData.JobWO.JobID);
            var employDTO = empRepo.GetEmployes();

            return Json(new
            {
                hasSaved = hasSaved,
                message = message,
                WOs = woList.Select(k => new
                {
                    WOID = k.WOID,
                    JobID = k.JobID,
                    WOCode = k.WOCode,
                    WorkWeekEndingDate = k.WorkWeekEndingDate,
                    LeadID = k.LeadID,
                    CrewLeader = (employDTO.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault() != null ? (employDTO.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault().FirstName + " " + employDTO.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault().LastName) : ""),
                    EstimateID = k.EstimateID,
                    TotalAmount = k.TotalAmount,
                    TotalJobBalanceAmount = k.TotalJobBalanceAmount,
                    COIDs = k.COIDs,
                    Active = k.Active
                }).OrderByDescending(p => p.WOID)
            });
        }
        /// <summary>
        /// Get Job WO Detail
        /// </summary>
        /// <param name="woID"></param>
        /// <returns></returns>
        public JsonResult GetJobWODetail(int woID, int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            EstimateDTO estimateDTO = estimateRepo.ListAll();
            JobDTO jobDTO = jobRepo.GetJobWO(woID);
            var estDTO = estimateRepo.Select(jobDTO.JobWO.EstimateID ?? 0);
            var unitOfMeasure = estDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            List<Core.Entities.Product.AssemblyItem> assemblyItems = estDTO.AssemblyItems.ToList();
            return Json(new
            {
                JobWO = jobDTO.JobWO,
                keyValue = new
                {
                    originalEstimateItems = estDTO.EstimateItems,
                    estimates = estimateDTO.Estimates.Where(e => e.Status == (int)EstimateStatusType.Complete).Select(k => new { ID = k.ID, JobAddressID = k.JobAddressID, Description = k.Name }).OrderBy(k => k.Description),
                    items = estDTO.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        Code = k.Code,
                        Name = k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId) ? unitOfMeasure[k.UnitOfMeasureId].Name : string.Empty,
                        Price = k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estDTO.Assemblies
                                  join q in assemblyItems on p.Id equals q.AssemblyId
                                  join i in estDTO.Items on q.ItemId equals i.Id
                                  where i.ItemTypeId == (int)Core.Enums.ItemType.Material
                                  select new { p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name, ItemID = i.Id })
                    .Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.MaterialCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost,
                        //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                        ItemNames = k.ItemName,
                        ItemID = k.ItemID
                    }).OrderBy(k => k.Description).ToList(),
                    crewleads = jobRepo.GetJobAssignmentCrewLeaders(jobID).Select(k => new { ID = k.EmpId, Description = k.LeadName }).OrderBy(k => k.Description)
                },
                estimateItems = (from p in jobDTO.WOEstimateItems join q in estDTO.Items on p.ItemID equals q.Id 
                join a in estDTO.Assemblies on p.ItemAssemblyID equals a.Id select p).Select(k => new
                {
                    ID = k.ID,
                    WOID = k.WOID,
                    ItemAssemblyID = k.ItemAssemblyID,
                    ItemAssemblyType = k.ItemAssemblyType,
                    Budget = k.Budget,
                    Used = k.Used,
                    Balance = k.Balance,
                    Rate = k.Rate,
                    Amount = k.Amount,
                    LaborCost = 0,
                    MaterialCost = 0,
                    ItemNames = (k.ItemNames ?? ""),
                    COID = k.COID,
                    ItemID = k.ItemID
                })
            });
        }
        /// <summary>
        /// Create WO Document
        /// </summary>
        /// <param name="woID"></param>
        /// <returns></returns>
        public FileContentResult CreateWODocument(int woID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreateWODocument(woID);
            var filename = b.Name;
            string contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b.Text, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        #endregion

        #region "Job Invoice"
        /// <summary>
        /// Get Job Inovice list by jobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobInvoiceList(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var invoiceList = jobRepo.GetJobInvoices(jobID);
            var estimateDTO = estRepo.ListAll();
            return Json(new
            {
                keyValue = new
                {
                    INVs = invoiceList.OrderByDescending(p => p.InvoiceID),
                    estimates = estimateDTO.Estimates.Where(e => e.Status == (int)EstimateStatusType.Complete).Select(k => new { ID = k.ID, JobAddressID = k.JobAddressID, Description = k.Name }).OrderBy(k => k.Description),
                    cos = jobRepo.GetJobCOs(jobID).Where(e => e.Active == true).Select(k => new { ID = k.COID, Description = k.Description }).OrderBy(k => k.Description),
                }
            });
        }
        [HttpPost]
        public JsonResult GetEstimateItemsForInvoice(int estimateID)
        {
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            IItemManagementRepository itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<KRF.Core.Entities.Sales.EstimateItem> estimateItems = new List<KRF.Core.Entities.Sales.EstimateItem>();
            var estimateDTO = estimateRepo.Select(estimateID);
            if (estimateID != 0)
            {
                estimateItems = estimateDTO.EstimateItems;
            }
            var unitOfMeasure = estimateDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            var productDTO = itemRepo.GetInventory();
            List<KRF.Core.Entities.Product.AssemblyItem> assemblyItems = estimateDTO.AssemblyItems.ToList();

            var estimates = estimateItems.Select(k => new
            {
                ID = k.ID,
                EstimateID = k.EstimateID,
                ItemAssemblyID = k.ItemAssemblyID,
                ItemAssemblyType = k.ItemAssemblyType,
                Price = ((from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID select p.RetailCost).Sum()), //Price = k.Price,
                Quantity = k.Quantity,
                Cost = ((from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID select p.RetailCost).Sum()) * k.Quantity,
                LaborCost = k.LaborCost,
                MaterialCost = k.MaterialCost,
                ItemNames = "",
                COID = 0
            });

            return Json(new
            {
                estimateItems = estimates,
                originalEstimateItems = estimateItems,
                keyValue = new
                {
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
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = estimateDTO.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.TotalRetailCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost,
                        ItemNames = ""
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }
        [HttpPost]
        public JsonResult GetInvoiceCOItems(int COID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            IItemManagementRepository itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<KRF.Core.Entities.Sales.EstimateItem> estimateItems = new List<KRF.Core.Entities.Sales.EstimateItem>();
            var estimateDTO = estimateRepo.Select(0);
            List<KRF.Core.Entities.Product.AssemblyItem> assemblyItems = estimateDTO.AssemblyItems.ToList();
            JobDTO jobDTO = jobRepo.GetJobCO(COID);
            var productDTO = itemRepo.GetInventory();
            var unitOfMeasure = estimateDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            List<POCOEstimateItems> pocoItems = (from p in jobDTO.COEstimateItems join q in estimateDTO.Assemblies on p.ItemAssemblyID equals q.Id select p).Where(p => p.ItemAssemblyType == 1).Select(k => new POCOEstimateItems
            {
                ID = k.ID,
                EstimateID = 0,
                ItemAssemblyID = k.ItemAssemblyID,
                ItemAssemblyType = k.ItemAssemblyType,
                Price = ((from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID select p.RetailCost).Sum()),
                Quantity = k.Quantity,
                Cost = ((from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID select p.RetailCost).Sum()) * k.Quantity,
                LaborCost = 0,
                MaterialCost = 0,
                ItemNames = "",
                COID = k.COID
            }).ToList();

            var items = from p in jobDTO.COEstimateItems.Where(p => p.ItemAssemblyType == 0)
                        join q in estimateDTO.Items on p.ItemAssemblyID equals q.Id
                        select new POCOEstimateItems
                        {
                            ID = p.ID,
                            EstimateID = 0,
                            ItemAssemblyID = p.ItemAssemblyID,
                            ItemAssemblyType = p.ItemAssemblyType,
                            Price = q.Price,
                            Quantity = p.Quantity,
                            Cost = q.Price * p.Quantity,
                            LaborCost = 0,
                            MaterialCost = 0,
                            ItemNames = "",
                            COID = p.COID
                        };
            pocoItems = pocoItems.ToList();
            pocoItems.AddRange(items);

            return Json(new
            {
                COItems = pocoItems,
                keyValue = new
                {
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
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = estimateDTO.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.TotalRetailCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost,
                        ItemNames = string.Join(",", (from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }
        /// <summary>
        /// Toggle Job Invoice status
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult ToggleJobInvoiceStatus(int invoiceID, bool active)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasDeleted = jobRepo.ToggleJobInvoiceStatus(invoiceID, active);
            return Json(new { hasDeleted = hasDeleted });
        }
        ///// <summary>
        ///// Save Job PO
        ///// </summary>
        ///// <param name="jobData"></param>
        ///// <returns></returns>
        public JsonResult SaveJobInvoice(JobData jobData)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                jobData.JobInvoice.COIDs = jobData.JobInvoice.COIDs.Replace("~", ",");
                List<JobInvoice> lstJobInvoice = jobRepo.GetJobInvoices(jobData.JobInvoice.JobID);
                if (jobData.JobInvoice.InvoiceID > 0)
                {
                    if (lstJobInvoice.Any() && lstJobInvoice.Find(p => p.InvoiceCode == jobData.JobInvoice.InvoiceCode && p.InvoiceID != jobData.JobInvoice.InvoiceID) != null)
                    {
                        hasSaved = false;
                        message = "Invoice Id already exists.";
                    }
                    else
                    {
                        if (jobRepo.EditJobInvoice(jobData.JobInvoice, jobData.InvoiceItems))
                        {
                            hasSaved = true;
                            message = "Invoice updated successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "Invoice could not be updated.";
                        }
                    }
                }
                else
                {
                    if (lstJobInvoice.Any() && lstJobInvoice.Find(p => p.InvoiceCode == jobData.JobInvoice.InvoiceCode) != null)
                    {
                        hasSaved = false;
                        message = "Invoice Id already exists.";
                    }
                    else
                    {
                        if (jobRepo.CreateJobInvoice(jobData.JobInvoice, jobData.InvoiceItems) > 0)
                        {
                            hasSaved = true;
                            message = "Invoice created successfully!.";
                        }
                        else
                        {
                            hasSaved = false;
                            message = "Invoice could not be created.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Invoice could not be updated.";
            }

            return Json(new { hasSaved = hasSaved, message = message, INVs = jobRepo.GetJobInvoices(jobData.JobInvoice.JobID).OrderByDescending(t => t.InvoiceID) });
        }
        /// <summary>
        /// Get Job Invoice Detail
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        public JsonResult GetJobInvoiceDetail(int invoiceID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEstimateManagementRepository estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            EstimateDTO estimateDTO = estimateRepo.ListAll();
            JobDTO jobDTO = jobRepo.GetJobInvoice(invoiceID);
            var estDTO = estimateRepo.Select(jobDTO.JobInvoice.EstimateID ?? 0);
            var unitOfMeasure = estDTO.UnitOfMeasure.ToDictionary(k => k.Id);
            List<Core.Entities.Product.AssemblyItem> assemblyItems = estDTO.AssemblyItems.ToList();
            return Json(new
            {
                JobINV = jobDTO.JobInvoice,
                keyValue = new
                {
                    originalEstimateItems = estDTO.EstimateItems,
                    estimates = estimateDTO.Estimates.Where(e => e.Status == (int)EstimateStatusType.Complete).Select(k => new { ID = k.ID, JobAddressID = k.JobAddressID, Description = k.Name }).OrderBy(k => k.Description),
                    items = estDTO.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        Code = k.Code,
                        Name = k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId) ? unitOfMeasure[k.UnitOfMeasureId].Name : string.Empty,
                        Price = k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = estDTO.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        Code = k.Code,
                        Name = k.AssemblyName,
                        Price = k.MaterialCost,
                        MaterialCost = k.MaterialCost,
                        LaborCost = k.LaborCost,
                        ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                    }).OrderBy(k => k.Description).ToList()
                },
                estimateItems = (from p in jobDTO.InvoiceItems join a in estDTO.Assemblies on p.ItemAssemblyID equals a.Id select p).Select(k => new
                {
                    ID = k.ID,
                    InvoiceID = k.InvoiceID,
                    ItemAssemblyID = k.ItemAssemblyID,
                    ItemAssemblyType = k.ItemAssemblyType,
                    Price = k.Price, //Price = k.Price,
                    Quantity = k.Quantity,
                    Cost = k.Cost,
                    LaborCost = 0,
                    MaterialCost = 0,
                    ItemNames = "",
                    COID = k.COID
                })
            });
        }
        /// <summary>
        /// Generate Invoice Document
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        public FileContentResult CreateInvoiceDocument(int invoiceID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreateInvoiceDocument(invoiceID);
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + b.Name + ";");
            return new FileContentResult(b.Text, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        #endregion

        #region "Job Inspection"
        /// <summary>
        /// Get Job Inspection list by jobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public ActionResult GetJobInspectionList(int jobID)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            IEmployeeManagementRepository empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var inspList = jobRepo.GetJobInspections(jobID);
            var employDTO = empRepo.GetEmployes();
            List<Permit> permits = jobRepo.GetPrmitList();
            List<PermitInspection> permitInspection = jobRepo.GetPrmitInspectionList();
            List<PermitStatus> permitStatus = jobRepo.GetPrmitStatusList();
            var jobDTO = jobRepo.GetJobDetail(jobID);
            string jobAddress = jobRepo.GetJobAddress(jobID);
            return Json(new
            {
                keyValue = new
                {
                    Permits = inspList.Select(k => new
                    {
                        InspID = k.InspID,
                        JobID = k.JobID,
                        JobAddress = k.JobAddress,
                        EmployeeID = k.EmployeeID,
                        EmployeeName = (employDTO.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault() != null ? (employDTO.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault().FirstName + " " + employDTO.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault().LastName) : ""),
                        Address = "",
                        PermitID = k.PermitID,
                        Permit = (permits.Where(p => p.ID == k.PermitID).FirstOrDefault() != null ? permits.Where(p => p.ID == k.PermitID).FirstOrDefault().Description : ""),
                        Phone = k.Phone,
                        CalledDate = k.CalledDate,
                        ResultDate = k.ResultDate,
                        InspectionID = k.InspectionID,
                        Inspection = (permitInspection.Where(p => p.ID == k.InspectionID).FirstOrDefault() != null ? permitInspection.Where(p => p.ID == k.InspectionID).FirstOrDefault().Inspections : ""),
                        StatusID = k.StatusID,
                        Status = (permitStatus.Where(p => p.ID == k.StatusID).FirstOrDefault() != null ? permitStatus.Where(p => p.ID == k.StatusID).FirstOrDefault().Description : ""),
                        InspectorID = k.InspectorID,
                        InspectorName = (employDTO.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault() != null ? (employDTO.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault().FirstName + " " + employDTO.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault().LastName) : ""),
                        Notes = k.Notes,
                        Active = k.Active
                    }).OrderByDescending(p => p.InspID),
                    employees = employDTO.Employees.Where(e => e.Status == true).Select(k => new { ID = k.EmpId, Description = k.FirstName + " " + k.LastName }).OrderBy(k => k.Description),
                    permits = permits.Where(e => e.Active == true).OrderBy(e=>e.Description),
                    permitInspections = permitInspection.Where(e => e.Active == true).Select(k => new { ID = k.ID, Description = k.Inspections }).OrderBy(k => k.Description),
                    permitStatus = permitStatus.Where(e => e.Active == true).OrderBy(k => k.Description),
                    job = jobDTO.Jobs[0],
                    jobAddress = jobAddress
                }
            });
        }
        /// <summary>
        /// Toggle Job Inspection status
        /// </summary>
        /// <param name="inspID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult ToggleJobInspectionStatus(int inspID, bool active)
        {
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasDeleted = jobRepo.ToggleJobInspectionStatus(inspID, active);
            return Json(new { hasDeleted = hasDeleted });
        }
        /// <summary>
        /// Save Job Inspection
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobInspection(JobData jobData)
        {
            IEmployeeManagementRepository empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved = true;
            string message = string.Empty;
            try
            {
                if (jobData.JobInspection.InspID > 0)
                {
                    if (jobRepo.EditJobInspection(jobData.JobInspection))
                    {
                        hasSaved = true;
                        message = "Inspection detail updated successfully!.";
                    }
                    else
                    {
                        hasSaved = false;
                        message = "Inspection detail could not be updated.";
                    }
                }
                else
                {
                    if (jobRepo.CreateJobInspection(jobData.JobInspection) > 0)
                    {
                        hasSaved = true;
                        message = "Inspection detail created successfully!.";
                    }
                    else
                    {
                        hasSaved = false;
                        message = "Inspection detail could not be created.";
                    }
                }
            }
            catch (Exception ex)
            {
                hasSaved = false;
                message = "Inspection detail could not be updated.";
            }

            var inspList = jobRepo.GetJobInspections(jobData.JobInspection.JobID);
            var employDTO = empRepo.GetEmployes();
            List<Permit> permits = jobRepo.GetPrmitList();
            List<PermitInspection> permitInspection = jobRepo.GetPrmitInspectionList();
            List<PermitStatus> permitStatus = jobRepo.GetPrmitStatusList();
            return Json(new
            {
                hasSaved = hasSaved,
                message = message,
                Permits = inspList.Select(k => new
                {
                    InspID = k.InspID,
                    JobID = k.JobID,
                    JobAddress = k.JobAddress,
                    EmployeeID = k.EmployeeID,
                    EmployeeName = (employDTO.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault() != null ? (employDTO.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault().FirstName + " " + employDTO.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault().LastName) : ""),
                    Address = "",
                    PermitID = k.PermitID,
                    Permit = (permits.Where(p => p.ID == k.PermitID).FirstOrDefault() != null ? permits.Where(p => p.ID == k.PermitID).FirstOrDefault().Description : ""),
                    Phone = k.Phone,
                    CalledDate = k.CalledDate,
                    ResultDate = k.ResultDate,
                    InspectionID = k.InspectionID,
                    Inspection = (permitInspection.Where(p => p.ID == k.InspectionID).FirstOrDefault() != null ? permitInspection.Where(p => p.ID == k.InspectionID).FirstOrDefault().Inspections : ""),
                    StatusID = k.StatusID,
                    Status = (permitStatus.Where(p => p.ID == k.StatusID).FirstOrDefault() != null ? permitStatus.Where(p => p.ID == k.StatusID).FirstOrDefault().Description : ""),
                    InspectorID = k.InspectorID,
                    InspectorName = (employDTO.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault() != null ? (employDTO.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault().FirstName + " " + employDTO.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault().LastName) : ""),
                    Notes = k.Notes,
                    Active = k.Active
                }).OrderByDescending(p => p.InspID),
            });
        }
        /// <summary>
        /// Get Job Inspection Detail
        /// </summary>
        /// <param name="inspID"></param>
        /// <returns></returns>
        public JsonResult GetJobInspectionDetail(int inspID, int jobID)
        {
            IEmployeeManagementRepository empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            IJobManagementRepository jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            JobDTO jobDTO = jobRepo.GetJobInspection(inspID);
            List<Permit> permits = jobRepo.GetPrmitList();
            List<PermitInspection> permitInspection = jobRepo.GetPrmitInspectionList();
            List<PermitStatus> permitStatus = jobRepo.GetPrmitStatusList();
            var employDTO = empRepo.GetEmployes();
            return Json(new
            {
                JobInspection = jobDTO.JobInspection,
                keyValue = new
                {
                    employees = employDTO.Employees.Where(e => e.Status == true).Select(k => new { ID = k.EmpId, Description = k.FirstName + " " + k.LastName }).OrderBy(k => k.Description),
                    permits = permits.Where(e => e.Active == true),
                    permitInspections = permitInspection.Where(e => e.Active == true).Select(k => new { ID = k.ID, Description = k.Inspections }).OrderBy(k => k.Description),
                    permitStatus = permitStatus.Where(e => e.Active == true)
                }
            });
        }
        #endregion
    }
}
