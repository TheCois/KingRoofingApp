using KRF.Core.DTO.Job;
using KRF.Core.Entities.Customer;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Master;
using KRF.Core.Enums;
using KRF.Core.Repository;
using KRF.Web.Models;
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
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
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
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            Common.Common.IsUserAuthorizeToPerformThisAction(SessionManager.RoleId, (int) PageType.Job,
                (int) PermissionType.Edit);
            var jobDto = (JobDTO) TempData["Jobs"] ?? jobRepo.GetJobs();

            var leads = jobDto.Leads;

            return Json(new
            {
                param.sEcho,
                iTotalRecords = 97,
                iTotalDisplayRecords = 3,
                aaData = (from p in jobDto.Jobs
                    select new[]
                    {
                        p.Status == false
                            ? "<span class='edit-job' data-val=" + p.Id +
                              "><ul><li class='edit'><a href='#non'>View</a></li></ul></span>"
                            : "",
                        p.JobCode,
                        p.Title,
                        leads.FirstOrDefault(c => c.ID == p.LeadID).FirstName + " " +
                        leads.FirstOrDefault(c => c.ID == p.LeadID).LastName,
                        p.LeadID.ToString(),
                        p.StartDate != null ? Convert.ToDateTime(p.StartDate).ToShortDateString() : "",
                        p.EndDate != null ? Convert.ToDateTime(p.EndDate).ToShortDateString() : "",
                        "<span>" + jobDto.JobStatus.Where(s => s.ID == p.JobStatusID).FirstOrDefault().Description +
                        "</span>",
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
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Add(int id = 0)
        {
            ObjectFactory.GetInstance<IJobManagementRepository>();
            ViewBag.ID = id;
            return View();
        }

        #endregion

        #region "Job Information/Job Summary"

        /// <summary>
        /// Get Job Detail by JobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobInformation(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var jobDto = jobRepo.GetJobDetail(jobId);
            var customers = jobDto.Leads.Where(k => k.LeadStage == (int) LeadStageType.Customer)
                .Select(k => new {k.ID, Description = k.FirstName + " " + k.LastName, Contact = k.LeadName})
                .OrderBy(k => k.Description).ToList();
            var leads = jobDto.Leads.Where(p => !string.IsNullOrEmpty(p.LeadName)).Select(k => new {k.ID, Description = $"{k.LeadName} - {k.FirstName} {k.LastName}" ?? ""})
                .ToList();
            var job = jobDto.Jobs[0];
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            leadRepo.GetLead(job?.LeadID ?? 0);
            var employeeRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var empDto = employeeRepo.GetEmployees();
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var crewDto = crewRepo.GetCrews();

            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDto = estimateRepo.ListAll();
            var lstCrewEmp = crewDto.Crews.Where(c => c.Active).Select(crew => new CrewEmpDetails
                {Id = crew.CrewID, Name = crew.CrewName, Type = "C"}).ToList();
            lstCrewEmp.AddRange(empDto.Employees.Where(e => e.Status == true).Select(emp =>
                new CrewEmpDetails {Id = emp.EmpId, Name = emp.FirstName + " " + emp.LastName, Type = "E"}));

            var lstItemAssemblies = jobDto.Assemblies.Select(assembly => new ItemAssemblies
                {Id = assembly.Id, Name = assembly.AssemblyName, Code = assembly.Code, Type = "1"}).ToList();
            //26-Aug-2015 - Need to show only Assembly in items
            //foreach (var item in jobDTO.Items)
            //{
            //    lstItemAssemblies.Add(new ItemAssemblies() { Id = item.Id, Name = item.Name, Code = item.Code, Type = "0" });
            //}

            var unitOfMeasure = jobDto.UnitOfMeasures.ToDictionary(k => k.Id);

            var addressList = jobDto.CustomerAddress
                .Select(k => new {k.ID, CustomerID = k.LeadID, Description = k.Address1 + " " + k.Address2})
                .Where(k => k.Description.Trim().Length > 0).OrderBy(k => k.Description).ToList();

            return Json(new
            {
                job,
                keyValue = new
                {
                    customers = customers.OrderBy(c => c.Description),
                    leads = leads.OrderBy(l => l.Description),
                    addresses = addressList,
                    custoAddresses = jobDto.CustomerAddress.ToList(),
                    cities = jobDto.Cities,
                    states = jobDto.States,
                    crewEmps = lstCrewEmp.Select(k => new {ID = k.Id + "~" + k.Type, Description = k.Name, k.Type})
                        .OrderBy(p => p.Description),
                    jobAssigments = MapJobAssignments(jobDto.JobAssignments, empDto.Employees, crewDto.Crews),
                    //tasks = jobDTO.JobTasks.OrderByDescending(t => t.TaskID),
                    estimates = estimateDto.Estimates.Where(e => e.Status == (int) EstimateStatusType.Complete)
                        .Select(k => new {k.ID, k.JobAddressID, Description = k.Name}).OrderBy(k => k.Description),
                    //POs = jobDTO.JobPOs.OrderBy(p=>p.POID),
                    //COs = jobDTO.JobCOs.OrderBy(p=>p.COID),
                    itemAssemblies = lstItemAssemblies.Select(k => new
                            {ID = k.Id.ToString() + "," + k.Type, Description = k.Code + " - " + k.Name, k.Type})
                        .OrderBy(p => p.Description),
                    items = jobDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = jobDto.Assemblies.Select(k => new
                    {
                        ID = k.Id, Description = k.Code + " " + k.AssemblyName,
                        k.Code, Name = k.AssemblyName, Price = k.TotalRetailCost,
                        k.MaterialCost,
                        k.LaborCost
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }

        /// <summary>
        /// Get Job Summary by JobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobSummary(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var jobDto = jobRepo.GetJobDetail(jobId);
            var customers = jobDto.Leads.Where(k => k.LeadStage == (int) LeadStageType.Customer)
                .Select(k => new {k.ID, Description = k.FirstName + " " + k.LastName, Contact = k.LeadName})
                .OrderBy(k => k.Description).ToList();
            var job = jobDto.Jobs[0];
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var leadData = leadRepo.GetLead(job?.LeadID ?? 0);
            var lead = leadData.Leads[0];
            var jobAddress =
                jobDto.CustomerAddress.FirstOrDefault(c => c.LeadID == job.LeadID && c.ID == job.JobAddressID);
            var city = jobDto.Cities.FirstOrDefault(c => c.ID == jobAddress.City);
            var state = jobDto.States.FirstOrDefault(c => c.ID == jobAddress.State);
            var empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var employDto = empRepo.GetEmployees();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDto = estimateRepo.ListAll();
            var estimate = estimateDto.Estimates.FirstOrDefault(p => p.ID == job.EstimateID);
            var jobCOs = jobRepo.GetJobCOs(job.Id);
            var jobInVs = jobRepo.GetJobInvoices(job.Id);
            var totalPrice = (estimate?.TotalCost ?? 0) + jobCOs.Where(p => p.Active).Sum(p => p.TotalAmount);
            var relevantEmployee = employDto.Employees.FirstOrDefault(p => p.EmpId == lead.AssignedTo);
            return Json(new
            {
                job,
                keyValue = new
                {
                    statuses = estimateDto.Status.Where(k =>
                        k.ID == (int) EstimateStatusType.New || k.ID == (int) EstimateStatusType.InProgress ||
                        k.ID == (int) EstimateStatusType.Complete),
                    customerName = lead.FirstName + " " + lead.LastName,
                    jobSiteAddress = jobAddress?.Address1.Trim() + ", " + jobAddress?.Address2?.Trim() + ", " +
                                     (city != null ? city.Description?.Trim() : "") + " " + state?.Abbreviation.Trim() +
                                     ", " + jobAddress.ZipCode?.Trim(),
                    contactPerson = lead.LeadName,
                    salesPerson = relevantEmployee != null
                        ? relevantEmployee.FirstName + " " + relevantEmployee.LastName
                        : "",
                    contractPrice = Convert.ToString(estimate?.ContractPrice ?? 0),
                    totalChangeOrder = jobCOs.Where(p => p.Active).Sum(p => p.TotalAmount),
                    totalPrice,
                    totalPaymentReceived = jobInVs.Where(p => p.Active).Sum(p => p.TotalAmount),
                    totalOutstanding = jobInVs.Where(p => p.Active).Sum(p => p.TotalAmount) - totalPrice
                }
            });
        }

        /// <summary>
        /// Get Building Information by leadID
        /// </summary>
        /// <param name="leadId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBuildingInformation(int leadId)
        {
            var leadRepo = ObjectFactory.GetInstance<ILeadManagementRepository>();
            var leadData = leadRepo.GetLeads(x => true);
            var leads = leadRepo.GetLead(leadId);
            var lead = leads.Leads[0];
            return Json(new
            {
                buildingInformation = new
                {
                    ProjectType = lead != null
                        ? leadData.ProjectTypes.FirstOrDefault(p => p.ID == lead.ProjectType) != null
                            ?
                            leadData.ProjectTypes.FirstOrDefault(p => p.ID == lead.ProjectType).Description
                            : ""
                        : "",
                    RoofType = lead != null
                        ? leadData.RoofTypes.FirstOrDefault(p => p.ID == lead.RoofType) != null
                            ?
                            leadData.RoofTypes.FirstOrDefault(p => p.ID == lead.RoofType).Description
                            : ""
                        : "",
                    AgeOfRoof = lead != null
                        ? leadData.RoofAgeList.FirstOrDefault(p => p.ID == lead.RoofAge) != null
                            ?
                            leadData.RoofAgeList.FirstOrDefault(p => p.ID == lead.RoofAge).Description
                            : ""
                        : "",
                    BuildingStories = lead != null
                        ? leadData.BuildingStoriesList.FirstOrDefault(p => p.ID == lead.NumberOfStories) != null
                            ?
                            leadData.BuildingStoriesList.FirstOrDefault(p => p.ID == lead.NumberOfStories).Description
                            : ""
                        : "",
                    ProjectExpectedToBegin = lead != null
                        ? leadData.ProjectStartTimelines.FirstOrDefault(p => p.ID == lead.ProjectStartTimeline) !=
                          null ? leadData.ProjectStartTimelines.FirstOrDefault(p => p.ID == lead.ProjectStartTimeline)
                            .Description : ""
                        : "",
                    AdditionalInformation = lead != null ? lead.AdditionalInfo ?? "" : ""
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
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasSaved = true;
            var message = string.Empty;
            try
            {
                var job = jobData.Job;

                if (job.Id > 0)
                {
                    if (job.JobStatusID == (int) Core.Entities.MISC.Status.Complete)
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

            return Json(new {hasSaved, message});
        }

        /// <summary>
        /// Save Job Information
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobInformation(JobData jobData)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasSaved = true;
            var message = string.Empty;
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

            return Json(new {hasSaved, message});
        }

        #endregion

        #region "Job Assignments"

        public List<JobAssignmentDetails> MapJobAssignments(IList<JobAssignment> jobAssignments,
            IList<Employee> employees, IList<Crew> crews)
        {
            var jobAssignmentDetails = new List<JobAssignmentDetails>();
            foreach (var jobAsgmt in jobAssignments)
            {
                var name = string.Empty;
                switch (jobAsgmt.Type)
                {
                    case "E":
                        var emp = employees.FirstOrDefault(e => e.EmpId == jobAsgmt.ObjectPKID);
                        if (emp != null)
                        {
                            name = emp.FirstName + " " + emp.LastName;
                        }

                        break;
                    case "C":
                        var crew = crews.FirstOrDefault(c => c.CrewID == jobAsgmt.ObjectPKID);
                        if (crew != null)
                        {
                            name = crew.CrewName;
                        }

                        break;
                }

                jobAssignmentDetails.Add(new JobAssignmentDetails
                {
                    JobID = jobAsgmt.JobID,
                    JobAssignmentID = jobAsgmt.JobAssignmentID,
                    ObjectPKID = jobAsgmt.ObjectPKID,
                    Type = jobAsgmt.Type,
                    Name = name,
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
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved;
            string message;
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

            return Json(new {hasSaved, message});
        }

        /// <summary>
        /// Get Job Milestone list by jobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobTaskList(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var taskList = jobRepo.GetJobTasks(jobId);
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
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved;
            string message;
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
                Console.WriteLine(ex);
                hasSaved = false;
                message = "Task could not be updated.";
            }

            return Json(new
            {
                hasSaved, message, tasks = jobRepo.GetJobTasks(jobData.JobTask.JobID).OrderByDescending(t => t.TaskID)
            });
        }

        /// <summary>
        /// Update Task status
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        public JsonResult UpdateTaskStatus(int taskId, bool isCompleted)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            bool hasSaved;
            string message;
            try
            {
                if (taskId > 0)
                {
                    if (jobRepo.UpdateTaskStatus(taskId, isCompleted))
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
                Console.WriteLine(ex);
                hasSaved = false;
                message = "Task status could not be updated.";
            }

            return Json(new {hasSaved, message});
        }

        /// <summary>
        /// Calculate estimated labour hours and job end date
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobStartDate"></param>
        /// <param name="averageWorkingHour"></param>
        /// <param name="jobAssigments"></param>
        /// <returns></returns>
        public JsonResult CalculateEstimatedLaborHoursAndJobEndDate(int jobId, DateTime jobStartDate,
            int averageWorkingHour, List<JobAssignment> jobAssigments)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            decimal estimatedLaborHours = 0;
            var result = true;
            var jobEndDate = string.Empty;
            try
            {
                estimatedLaborHours = jobRepo.CalculateEstimatedLaborCost(jobId, jobStartDate, jobAssigments);
                var endDate = jobRepo.CalculateJobEndDate(jobStartDate, estimatedLaborHours, averageWorkingHour);
                jobEndDate = endDate.ToShortDateString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = false;
            }

            return Json(new {result, estimatedHour = Math.Round(estimatedLaborHours, 2), jobEndDate});
        }

        /// <summary>
        /// Mark job status as complete
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public JsonResult ToggleJobStatus(int jobId, bool tobeEnabled)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasDeleted = jobRepo.ToggleJobStatus(jobId, tobeEnabled);

            return Json(new {hasDeleted});
        }

        /// <summary>
        /// Get estimate items
        /// </summary>
        /// <param name="estimateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEstimateItems(int estimateId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<Core.Entities.Sales.EstimateItem> estimateItems = new List<Core.Entities.Sales.EstimateItem>();
            var estimateDto = estimateRepo.Select(estimateId);
            if (estimateId != 0)
            {
                estimateItems = estimateDto.EstimateItems;
            }

            var unitOfMeasure = estimateDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var productDto = itemRepo.GetInventory();
            var assemblyItems = estimateDto.AssemblyItems.ToList();

            var estimates =
                (from p in estimateItems
                    join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
                    join i in estimateDto.Items on q.ItemId equals i.Id
                    where i.ItemTypeId == (int) Core.Enums.ItemType.Material
                    select new
                    {
                        p.ID, p.EstimateID, p.ItemAssemblyID, p.ItemAssemblyType, p.MaterialCost, p.LaborCost,
                        q.RetailCost, ItemNames = i.Name, ItemID = i.Id, p.Quantity
                    }).Select(k => new
                {
                    k.ID,
                    k.EstimateID,
                    k.ItemAssemblyID,
                    k.ItemAssemblyType,
                    Price = k.RetailCost, //Price = k.Price,
                    //Quantity = productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault() != null ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty < k.Quantity) ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty - k.Quantity) * -1) : 0) : k.Quantity, //2-Sep-2015 - As discussed currently disabling this logic and showing estimate quantity in PO. We may need to enable this logic at later point of time
                    k.Quantity,
                    //Cost = k.RetailCost * (productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault() != null ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty < k.Quantity) ? ((productDTO.Inventories.Where(p => p.ItemID == k.ItemID).FirstOrDefault().Qty - k.Quantity) * -1) : 0) : k.Quantity),
                    Cost = k.RetailCost * k.Quantity,
                    LaborCost = k.RetailCost,
                    MaterialCost = k.RetailCost,
                    //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name)),
                    k.ItemNames,
                    k.ItemID,
                    COID = 0
                });

            return Json(new
            {
                estimateItems = estimates.Where(k => string.IsNullOrEmpty(k.ItemNames) == false),
                originalEstimateItems = estimateItems,
                keyValue = new
                {
                    items = estimateDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estimateDto.Assemblies
                            join q in assemblyItems on p.Id equals q.AssemblyId
                            join i in estimateDto.Items on q.ItemId equals i.Id
                            where i.ItemTypeId == (int) Core.Enums.ItemType.Material
                            select new
                            {
                                p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, q.RetailCost,
                                ItemName = i.Name, ItemID = i.Id
                            })
                        .Select(k => new
                        {
                            ID = k.Id,
                            Description = k.Code + " " + k.AssemblyName,
                            k.Code,
                            Name = k.AssemblyName,
                            Price = k.MaterialCost,
                            MaterialCost = k.RetailCost,
                            LaborCost = k.RetailCost,
                            //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                            ItemNames = k.ItemName,
                            k.ItemID
                        }).OrderBy(k => k.Description).ToList()
                }
            });
        }

        [HttpPost]
        public JsonResult GetCoItems(int coid)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            new List<Core.Entities.Sales.EstimateItem>();
            var estimateDto = estimateRepo.Select(0);
            var assemblyItems = estimateDto.AssemblyItems.ToList();
            var jobDto = jobRepo.GetJobCO(coid);
            var productDto = itemRepo.GetInventory();
            var unitOfMeasure = estimateDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var pocoItems = (from p in jobDto.COEstimateItems
                    join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
                    join i in estimateDto.Items on q.ItemId equals i.Id
                    where i.ItemTypeId == (int) Core.Enums.ItemType.Material
                    select new
                    {
                        p.ID, p.COID, p.ItemAssemblyID, p.ItemAssemblyType, q.RetailCost, ItemNames = i.Name,
                        ItemID = i.Id, p.Quantity
                    })
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

            var items = from p in jobDto.COEstimateItems.Where(p => p.ItemAssemblyType == 0)
                join q in estimateDto.Items on p.ItemAssemblyID equals q.Id
                where q.ItemTypeId == (int) Core.Enums.ItemType.Material
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
                    items = estimateDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(p => p.Description).ToList(),
                    assemblies = (from p in estimateDto.Assemblies
                            join q in assemblyItems on p.Id equals q.AssemblyId
                            join i in estimateDto.Items on q.ItemId equals i.Id
                            where i.ItemTypeId == (int) Core.Enums.ItemType.Material
                            select new
                            {
                                p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name,
                                ItemID = i.Id
                            })
                        .Select(k => new
                        {
                            ID = k.Id,
                            Description = k.Code + " " + k.AssemblyName,
                            k.Code,
                            Name = k.AssemblyName,
                            Price = k.MaterialCost,
                            k.MaterialCost,
                            k.LaborCost,
                            //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                            ItemNames = k.ItemName,
                            k.ItemID
                        }).OrderBy(k => k.Description).ToList()
                }
            });
        }

        #endregion

        #region "Job PO"

        /// <summary>
        /// Get Job Purchase order list by jobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobPoList(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var vendorRepo = ObjectFactory.GetInstance<IVendorManagementRepository>();
            var poList = jobRepo.GetJobPOs(jobId);
            var estimateDto = estRepo.ListAll();
            var vendorDto = vendorRepo.ListAllVendors();
            return Json(new
            {
                keyValue = new
                {
                    POs = poList.OrderByDescending(p => p.POID),
                    estimates = estimateDto.Estimates.Where(e => e.Status == (int) EstimateStatusType.Complete)
                        .Select(k => new {k.ID, k.JobAddressID, Description = k.Name}).OrderBy(k => k.Description),
                    cos = jobRepo.GetJobCOs(jobId).Where(e => e.Active).Select(k => new {ID = k.COID, k.Description})
                        .OrderBy(k => k.Description),
                    vendors = vendorDto.Vendors.Select(k => new
                    {
                        k.ID, Description = k.VendorName, Address = k.VendorAddress,
                        k.Phone,
                        k.Fax,
                        k.Cell,
                        k.Email,
                        k.Active
                    }).OrderBy(p => p.Description)
                }
            });
        }

        public JsonResult ToggleJobPoStatus(int poId, bool active)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasDeleted = jobRepo.ToggleJobPOStatus(poId, active);
            return Json(new {hasDeleted});
        }

        /// <summary>
        /// Save Job PO
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobPo(JobData jobData)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasSaved = true;
            var message = string.Empty;
            try
            {
                jobData.JobPO.COIDs = jobData.JobPO.COIDs.Replace("~", ",");
                var lstJobPo = jobRepo.GetJobPOs(jobData.JobPO.JobID);
                if (jobData.JobPO.POID > 0)
                {
                    if (lstJobPo.Any() &&
                        lstJobPo.Find(p => p.POCode == jobData.JobPO.POCode && p.POID != jobData.JobPO.POID) != null)
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
                    if (lstJobPo.Any() && lstJobPo.Find(p => p.POCode == jobData.JobPO.POCode) != null)
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

            return Json(new
                {hasSaved, message, POs = jobRepo.GetJobPOs(jobData.JobPO.JobID).OrderByDescending(t => t.POID)});
        }

        /// <summary>
        /// Get Job PO Detail
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public JsonResult GetJobPoDetail(int poId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDto = estimateRepo.ListAll();
            var jobDto = jobRepo.GetJobPO(poId);
            var estDto = estimateRepo.Select(jobDto.JobPO.EstimateID ?? 0);
            var unitOfMeasure = estDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var assemblyItems = estDto.AssemblyItems.ToList();
            return Json(new
            {
                jobDto.JobPO,
                keyValue = new
                {
                    originalEstimateItems = estDto.EstimateItems,
                    estimates = estimateDto.Estimates.Where(e => e.Status == (int) EstimateStatusType.Complete)
                        .Select(k => new {k.ID, k.JobAddressID, Description = k.Name}).OrderBy(k => k.Description),
                    items = estDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estDto.Assemblies
                            join q in assemblyItems on p.Id equals q.AssemblyId
                            join i in estDto.Items on q.ItemId equals i.Id
                            where i.ItemTypeId == (int) Core.Enums.ItemType.Material
                            select new
                            {
                                p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name,
                                ItemID = i.Id
                            })
                        .Select(k => new
                        {
                            ID = k.Id,
                            Description = k.Code + " " + k.AssemblyName,
                            k.Code,
                            Name = k.AssemblyName,
                            Price = k.MaterialCost,
                            k.MaterialCost,
                            k.LaborCost,
                            //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                            ItemNames = k.ItemName,
                            k.ItemID
                        }).OrderBy(k => k.Description).ToList()
                },
                estimateItems = (from p in jobDto.POEstimateItems
                    join q in estDto.Items on p.ItemID equals q.Id
                    join a in estDto.Assemblies on p.ItemAssemblyID equals a.Id
                    select p).Select(k => new
                {
                    k.ID,
                    k.POID,
                    k.ItemAssemblyID,
                    k.ItemAssemblyType,
                    k.Price, //Price = k.Price,
                    k.Quantity,
                    k.Cost,
                    LaborCost = 0,
                    MaterialCost = 0,
                    ItemNames = k.ItemNames ?? "",
                    k.COID,
                    k.ItemID
                })
            });
        }

        public FileContentResult CreatePoDocument(int poId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreatePODocument(poId);
            var filename = "Purchase_Order.docx";
            var contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        #endregion

        #region "Job CO"

        /// <summary>
        /// Get Job Change order list by jobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobCoList(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var coList = jobRepo.GetJobCOs(jobId);
            var empList = empRepo.GetEmployees();
            return Json(new
            {
                keyValue = new
                {
                    COs = coList.OrderByDescending(p => p.COID),
                    reps = empList.Employees.Where(e => e.Status == true).Select(e =>
                            new {ID = e.EmpId, Description = e.FirstName + " " + e.LastName, Email = e.EmailID})
                        .OrderBy(e => e.Description)
                }
            });
        }

        /// <summary>
        /// Togggle Job Changer Order Status
        /// </summary>
        /// <param name="coId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult ToggleJobCoStatus(int coId, bool active)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasDeleted = jobRepo.ToggleJobCOStatus(coId, active);
            return Json(new {hasDeleted});
        }

        /// <summary>
        /// Save Job CO
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobCo(JobData jobData)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasSaved = true;
            var message = string.Empty;
            try
            {
                var lstJobCo = jobRepo.GetJobCOs(jobData.JobCO.JobID);
                if (jobData.JobCO.COID > 0)
                {
                    if (lstJobCo.Any() &&
                        lstJobCo.Find(p => p.COCode == jobData.JobCO.COCode && p.COID != jobData.JobCO.COID) != null)
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
                    if (lstJobCo.Any() && lstJobCo.Find(p => p.COCode == jobData.JobCO.COCode) != null)
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

            return Json(new
                {hasSaved, message, COs = jobRepo.GetJobCOs(jobData.JobCO.JobID).OrderByDescending(t => t.COID)});
        }

        /// <summary>
        /// Get Job CO Detail
        /// </summary>
        /// <param name="coId"></param>
        /// <returns></returns>
        public JsonResult GetJobCoDetail(int coId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDto = estimateRepo.ListAll();
            var jobDto = jobRepo.GetJobCO(coId);
            var estDto = estimateRepo.Select(0);
            var unitOfMeasure = estDto.UnitOfMeasure.ToDictionary(k => k.Id);
            return Json(new
            {
                jobDto.JobCO,
                keyValue = new
                {
                    items = estDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(p => p.Description).ToList(),
                    assemblies = estDto.Assemblies.Select(k => new
                    {
                        ID = k.Id, Description = k.Code + " " + k.AssemblyName,
                        k.Code, Name = k.AssemblyName, Price = k.TotalRetailCost,
                        k.MaterialCost,
                        k.LaborCost
                    }).OrderBy(p => p.Description).ToList()
                },
                estimateItems = from p in jobDto.COEstimateItems
                    join a in estDto.Assemblies on p.ItemAssemblyID equals a.Id
                    select p
            });
        }

        /// <summary>
        /// Create Job Change Order Document
        /// </summary>
        /// <param name="coId"></param>
        /// <returns></returns>
        public FileContentResult CreateCoDocument(int coId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreateCODocument(coId);
            var filename = "Change_Order.docx";
            var contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        #endregion

        #region "Job Documents"

        /// <summary>
        /// Get Job Purchase order list by jobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobDocumentList(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var documentList = jobRepo.GetJobDocuments(jobId).OrderByDescending(p => p.ID);
            var documentTypes = jobRepo.GetJobDocumentTypes();
            return Json(new
            {
                keyValue = new
                {
                    documents = documentList.Select(k =>
                    {
                        var relevantJobDocumentType =
                            documentTypes.FirstOrDefault(p => p.ID.ToString() == k.Type.Trim());
                        return new
                        {
                            k.ID,
                            k.JobID,
                            k.Name,
                            k.Type,
                            TypeName = relevantJobDocumentType != null
                                ? relevantJobDocumentType.DocumentType
                                : "",
                            k.Description,
                            UploadDateTime = Convert.ToDateTime(k.UploadDateTime).ToString("MM/dd/yyyy HH:mm")
                        };
                    }),
                    documentTypes = documentTypes.Select(k => new {k.ID, Description = k.DocumentType})
                }
            });
        }

        /// <summary>
        /// Upload Job Document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="documentTypes"></param>
        /// <param name="jobId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, int documentTypes, int jobId, int id)
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

            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var doc = new JobDocument
            {
                ID = id,
                JobID = jobId,
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
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var jobDocument = jobRepo.GetJobDocument(id);
            var b = jobDocument.Text;
            var filename = jobDocument.Name;
            var contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        /// <summary>
        /// Delete job document by jobDocumentID
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        public ActionResult DeleteJobDocument(int jobDocumentId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var isDeleted = jobRepo.DeleteJobDocument(jobDocumentId);
            return Json(new {isDeleted});
        }

        #endregion

        #region "Job WO"

        /// <summary>
        /// Get Job Work order list by jobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobWoList(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var crewRepo = ObjectFactory.GetInstance<ICrewManagementRepository>();
            var empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var woList = jobRepo.GetJobWOs(jobId);
            var estimateDto = estRepo.ListAll();
            var employDto = empRepo.GetEmployees();
            var jobAssignments = jobRepo.GetJobAssignments(jobId);
            return Json(new
            {
                keyValue = new
                {
                    WOs = woList.Select(k =>
                    {
                        var relevantEmployee = employDto.Employees.FirstOrDefault(p => p.EmpId == k.LeadID);
                        return new
                        {
                            k.WOID,
                            k.JobID,
                            k.WOCode,
                            k.WorkWeekEndingDate,
                            k.LeadID,
                            CrewLeader = relevantEmployee != null
                                ? relevantEmployee.FirstName + " " +
                                  relevantEmployee.LastName
                                : "",
                            k.EstimateID,
                            k.TotalAmount,
                            k.TotalJobBalanceAmount,
                            k.COIDs,
                            k.Active
                        };
                    }).OrderByDescending(p => p.WOID),
                    estimates = estimateDto.Estimates.Where(e => e.Status == (int) EstimateStatusType.Complete)
                        .Select(k => new {k.ID, k.JobAddressID, Description = k.Name}).OrderBy(k => k.Description),
                    cos = jobRepo.GetJobCOs(jobId).Where(e => e.Active).Select(k => new {ID = k.COID, k.Description})
                        .OrderBy(k => k.Description),
                    crewleads = jobRepo.GetJobAssignmentCrewLeaders(jobId)
                        .Select(k => new {ID = k.EmpId, Description = k.LeadName}).OrderBy(k => k.Description)
                }
            });
        }

        public JsonResult ToggleJobWoStatus(int woId, bool active)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasDeleted = jobRepo.ToggleJobWOStatus(woId, active);
            return Json(new {hasDeleted});
        }

        [HttpPost]
        public JsonResult GetEstimateItemsForWo(int estimateId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<Core.Entities.Sales.EstimateItem> estimateItems = new List<Core.Entities.Sales.EstimateItem>();
            var estimateDto = estimateRepo.Select(estimateId);
            if (estimateId != 0)
            {
                estimateItems = estimateDto.EstimateItems;
            }

            var unitOfMeasure = estimateDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var productDto = itemRepo.GetInventory();
            var assemblyItems = estimateDto.AssemblyItems.ToList();

            var estimates = (from p in estimateItems
                    join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
                    join i in estimateDto.Items on q.ItemId equals i.Id
                    where i.ItemTypeId == (int) Core.Enums.ItemType.Labor
                    select new
                    {
                        p.ID, p.EstimateID, p.ItemAssemblyID, p.ItemAssemblyType, p.MaterialCost, p.LaborCost,
                        q.RetailCost, ItemNames = i.Name, ItemID = i.Id, p.Quantity
                    })
                .Select(k => new
                {
                    k.ID,
                    k.EstimateID,
                    k.ItemAssemblyID,
                    k.ItemAssemblyType,
                    Budget = k.Quantity,
                    Used = 0,
                    //Rate = ((from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID && q.ItemTypeId == (int)Core.Enums.ItemType.Labor select p.RetailCost).Sum()),
                    Rate = k.RetailCost,
                    Balance = k.Quantity,
                    Amount = 0,
                    LaborCost = k.RetailCost,
                    MaterialCost = k.RetailCost,
                    //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estimateDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.ItemAssemblyID && q.ItemTypeId == (int)Core.Enums.ItemType.Labor select q.Name)),
                    k.ItemNames,
                    k.ItemID,
                    COID = 0
                });

            return Json(new
            {
                estimateItems = estimates.Where(k => string.IsNullOrEmpty(k.ItemNames) == false),
                originalEstimateItems = estimateItems,
                keyValue = new
                {
                    items = estimateDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estimateDto.Assemblies
                            join q in assemblyItems on p.Id equals q.AssemblyId
                            join i in estimateDto.Items on q.ItemId equals i.Id
                            where i.ItemTypeId == (int) Core.Enums.ItemType.Labor
                            select new
                            {
                                p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, q.RetailCost,
                                ItemName = i.Name, ItemID = i.Id
                            })
                        .Select(k => new
                        {
                            ID = k.Id,
                            Description = k.Code + " " + k.AssemblyName,
                            k.Code,
                            Name = k.AssemblyName,
                            Price = k.RetailCost,
                            MaterialCost = k.RetailCost,
                            LaborCost = k.RetailCost,
                            //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                            ItemNames = k.ItemName,
                            k.ItemID
                        }).OrderBy(k => k.Description).ToList()
                }
            });
        }

        [HttpPost]
        public JsonResult GetCoItemsForWo(int coid)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<Core.Entities.Sales.EstimateItem> estimateItems = new List<Core.Entities.Sales.EstimateItem>();
            var estimateDto = estimateRepo.Select(0);
            var assemblyItems = estimateDto.AssemblyItems.ToList();
            var jobDto = jobRepo.GetJobCO(coid);
            var productDto = itemRepo.GetInventory();
            var unitOfMeasure = estimateDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var wocoItems = (from p in jobDto.COEstimateItems
                join q in assemblyItems on p.ItemAssemblyID equals q.AssemblyId
                join i in estimateDto.Items on q.ItemId equals i.Id
                where i.ItemTypeId == (int) Core.Enums.ItemType.Labor
                select new
                {
                    p.ID, p.ItemAssemblyID, p.COID, p.ItemAssemblyType, p.Quantity, q.RetailCost, ItemName = i.Name,
                    ItemID = i.Id
                }).Select(k => new WOCOEstimateItems
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

            var items = from p in jobDto.COEstimateItems.Where(p => p.ItemAssemblyType == 0)
                join q in estimateDto.Items on p.ItemAssemblyID equals q.Id
                where q.ItemTypeId == (int) Core.Enums.ItemType.Labor
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
                    items = estimateDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estimateDto.Assemblies
                            join q in assemblyItems on p.Id equals q.AssemblyId
                            join i in estimateDto.Items on q.ItemId equals i.Id
                            where i.ItemTypeId == (int) Core.Enums.ItemType.Labor
                            select new
                            {
                                p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name,
                                ItemID = i.Id
                            })
                        .Select(k => new
                        {
                            ID = k.Id,
                            Description = k.Code + " " + k.AssemblyName,
                            k.Code,
                            Name = k.AssemblyName,
                            Price = k.MaterialCost,
                            k.MaterialCost,
                            k.LaborCost,
                            //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                            ItemNames = k.ItemName,
                            k.ItemID
                        }).OrderBy(k => k.Description).ToList()
                }
            });
        }

        /// <summary>
        /// Save Job WO
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobWo(JobData jobData)
        {
            var empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasSaved = true;
            var message = string.Empty;
            try
            {
                jobData.JobWO.COIDs = jobData.JobWO.COIDs.Replace("~", ",");
                var lstJobWo = jobRepo.GetJobWOs(jobData.JobWO.JobID);
                if (jobData.JobWO.WOID > 0)
                {
                    if (lstJobWo.Any() &&
                        lstJobWo.Find(p => p.WOCode == jobData.JobWO.WOCode && p.WOID != jobData.JobWO.WOID) != null)
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
                    if (lstJobWo.Any() && lstJobWo.Find(p => p.WOCode == jobData.JobWO.WOCode) != null)
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
            var employDto = empRepo.GetEmployees();

            return Json(new
            {
                hasSaved,
                message,
                WOs = woList.Select(k => new
                {
                    k.WOID,
                    k.JobID,
                    k.WOCode,
                    k.WorkWeekEndingDate,
                    k.LeadID,
                    CrewLeader = employDto.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault() != null
                        ? employDto.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault().FirstName + " " +
                          employDto.Employees.Where(p => p.EmpId == k.LeadID).FirstOrDefault().LastName
                        : "",
                    k.EstimateID,
                    k.TotalAmount,
                    k.TotalJobBalanceAmount,
                    k.COIDs,
                    k.Active
                }).OrderByDescending(p => p.WOID)
            });
        }

        /// <summary>
        /// Get Job WO Detail
        /// </summary>
        /// <param name="woId"></param>
        /// <returns></returns>
        public JsonResult GetJobWoDetail(int woId, int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDto = estimateRepo.ListAll();
            var jobDto = jobRepo.GetJobWO(woId);
            var estDto = estimateRepo.Select(jobDto.JobWO.EstimateID ?? 0);
            var unitOfMeasure = estDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var assemblyItems = estDto.AssemblyItems.ToList();
            return Json(new
            {
                jobDto.JobWO,
                keyValue = new
                {
                    originalEstimateItems = estDto.EstimateItems,
                    estimates = estimateDto.Estimates.Where(e => e.Status == (int) EstimateStatusType.Complete)
                        .Select(k => new {k.ID, k.JobAddressID, Description = k.Name}).OrderBy(k => k.Description),
                    items = estDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = (from p in estDto.Assemblies
                            join q in assemblyItems on p.Id equals q.AssemblyId
                            join i in estDto.Items on q.ItemId equals i.Id
                            where i.ItemTypeId == (int) Core.Enums.ItemType.Material
                            select new
                            {
                                p.Id, p.Code, p.AssemblyName, p.MaterialCost, p.LaborCost, ItemName = i.Name,
                                ItemID = i.Id
                            })
                        .Select(k => new
                        {
                            ID = k.Id,
                            Description = k.Code + " " + k.AssemblyName,
                            k.Code,
                            Name = k.AssemblyName,
                            Price = k.MaterialCost,
                            k.MaterialCost,
                            k.LaborCost,
                            //ItemNames = string.Join("<br/>", (from p in assemblyItems join q in estDTO.Items on p.ItemId equals q.Id where p.AssemblyId == k.Id && q.ItemTypeId == (int)Core.Enums.ItemType.Material select q.Name))
                            ItemNames = k.ItemName,
                            k.ItemID
                        }).OrderBy(k => k.Description).ToList(),
                    crewleads = jobRepo.GetJobAssignmentCrewLeaders(jobId)
                        .Select(k => new {ID = k.EmpId, Description = k.LeadName}).OrderBy(k => k.Description)
                },
                estimateItems = (from p in jobDto.WOEstimateItems
                    join q in estDto.Items on p.ItemID equals q.Id
                    join a in estDto.Assemblies on p.ItemAssemblyID equals a.Id
                    select p).Select(k => new
                {
                    k.ID,
                    k.WOID,
                    k.ItemAssemblyID,
                    k.ItemAssemblyType,
                    k.Budget,
                    k.Used,
                    k.Balance,
                    k.Rate,
                    k.Amount,
                    LaborCost = 0,
                    MaterialCost = 0,
                    ItemNames = k.ItemNames ?? "",
                    k.COID,
                    k.ItemID
                })
            });
        }

        /// <summary>
        /// Create WO Document
        /// </summary>
        /// <param name="woId"></param>
        /// <returns></returns>
        public FileContentResult CreateWoDocument(int woId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreateWODocument(woId);
            var filename = b.Name;
            var contentType = filename.Split('.').LastOrDefault();
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ";");
            return new FileContentResult(b.Text,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        #endregion

        #region "Job Invoice"

        /// <summary>
        /// Get Job Inovice list by jobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobInvoiceList(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var invoiceList = jobRepo.GetJobInvoices(jobId);
            var estimateDto = estRepo.ListAll();
            return Json(new
            {
                keyValue = new
                {
                    INVs = invoiceList.OrderByDescending(p => p.InvoiceID),
                    estimates = estimateDto.Estimates.Where(e => e.Status == (int) EstimateStatusType.Complete)
                        .Select(k => new {k.ID, k.JobAddressID, Description = k.Name}).OrderBy(k => k.Description),
                    cos = jobRepo.GetJobCOs(jobId).Where(e => e.Active).Select(k => new {ID = k.COID, k.Description})
                        .OrderBy(k => k.Description),
                }
            });
        }

        [HttpPost]
        public JsonResult GetEstimateItemsForInvoice(int estimateId)
        {
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<Core.Entities.Sales.EstimateItem> estimateItems = new List<Core.Entities.Sales.EstimateItem>();
            var estimateDto = estimateRepo.Select(estimateId);
            if (estimateId != 0)
            {
                estimateItems = estimateDto.EstimateItems;
            }

            var unitOfMeasure = estimateDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var productDto = itemRepo.GetInventory();
            var assemblyItems = estimateDto.AssemblyItems.ToList();

            var estimates = estimateItems.Select(k => new
            {
                k.ID,
                k.EstimateID,
                k.ItemAssemblyID,
                k.ItemAssemblyType,
                Price = (from p in assemblyItems
                    join q in estimateDto.Items on p.ItemId equals q.Id
                    where p.AssemblyId == k.ItemAssemblyID
                    select p.RetailCost).Sum(), //Price = k.Price,
                k.Quantity,
                Cost = (from p in assemblyItems
                           join q in estimateDto.Items on p.ItemId equals q.Id
                           where p.AssemblyId == k.ItemAssemblyID
                           select p.RetailCost).Sum() * k.Quantity,
                k.LaborCost,
                k.MaterialCost,
                ItemNames = "",
                COID = 0
            });

            return Json(new
            {
                estimateItems = estimates,
                originalEstimateItems = estimateItems,
                keyValue = new
                {
                    items = estimateDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = estimateDto.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        k.Code,
                        Name = k.AssemblyName,
                        Price = k.TotalRetailCost,
                        k.MaterialCost,
                        k.LaborCost,
                        ItemNames = ""
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }

        [HttpPost]
        public JsonResult GetInvoiceCoItems(int coid)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var itemRepo = ObjectFactory.GetInstance<IItemManagementRepository>();
            IList<Core.Entities.Sales.EstimateItem> estimateItems = new List<Core.Entities.Sales.EstimateItem>();
            var estimateDto = estimateRepo.Select(0);
            var assemblyItems = estimateDto.AssemblyItems.ToList();
            var jobDto = jobRepo.GetJobCO(coid);
            var productDto = itemRepo.GetInventory();
            var unitOfMeasure = estimateDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var pocoItems =
                (from p in jobDto.COEstimateItems
                    join q in estimateDto.Assemblies on p.ItemAssemblyID equals q.Id
                    select p).Where(p => p.ItemAssemblyType == 1).Select(k => new POCOEstimateItems
                {
                    ID = k.ID,
                    EstimateID = 0,
                    ItemAssemblyID = k.ItemAssemblyID,
                    ItemAssemblyType = k.ItemAssemblyType,
                    Price = (from p in assemblyItems
                        join q in estimateDto.Items on p.ItemId equals q.Id
                        where p.AssemblyId == k.ItemAssemblyID
                        select p.RetailCost).Sum(),
                    Quantity = k.Quantity,
                    Cost = (from p in assemblyItems
                               join q in estimateDto.Items on p.ItemId equals q.Id
                               where p.AssemblyId == k.ItemAssemblyID
                               select p.RetailCost).Sum() * k.Quantity,
                    LaborCost = 0,
                    MaterialCost = 0,
                    ItemNames = "",
                    COID = k.COID
                }).ToList();

            var items = from p in jobDto.COEstimateItems.Where(p => p.ItemAssemblyType == 0)
                join q in estimateDto.Items on p.ItemAssemblyID equals q.Id
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
                    items = estimateDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = estimateDto.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        k.Code,
                        Name = k.AssemblyName,
                        Price = k.TotalRetailCost,
                        k.MaterialCost,
                        k.LaborCost,
                        ItemNames = string.Join(",",
                            from p in assemblyItems
                            join q in estimateDto.Items on p.ItemId equals q.Id
                            where p.AssemblyId == k.Id && q.ItemTypeId == (int) Core.Enums.ItemType.Material
                            select q.Name)
                    }).OrderBy(k => k.Description).ToList()
                }
            });
        }

        /// <summary>
        /// Toggle Job Invoice status
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult ToggleJobInvoiceStatus(int invoiceId, bool active)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasDeleted = jobRepo.ToggleJobInvoiceStatus(invoiceId, active);
            return Json(new {hasDeleted});
        }

        ///// <summary>
        ///// Save Job PO
        ///// </summary>
        ///// <param name="jobData"></param>
        ///// <returns></returns>
        public JsonResult SaveJobInvoice(JobData jobData)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasSaved = true;
            var message = string.Empty;
            try
            {
                jobData.JobInvoice.COIDs = jobData.JobInvoice.COIDs.Replace("~", ",");
                var lstJobInvoice = jobRepo.GetJobInvoices(jobData.JobInvoice.JobID);
                if (jobData.JobInvoice.InvoiceID > 0)
                {
                    if (lstJobInvoice.Any() && lstJobInvoice.Find(p =>
                            p.InvoiceCode == jobData.JobInvoice.InvoiceCode &&
                            p.InvoiceID != jobData.JobInvoice.InvoiceID) != null)
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
                    if (lstJobInvoice.Any() &&
                        lstJobInvoice.Find(p => p.InvoiceCode == jobData.JobInvoice.InvoiceCode) != null)
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

            return Json(new
            {
                hasSaved, message,
                INVs = jobRepo.GetJobInvoices(jobData.JobInvoice.JobID).OrderByDescending(t => t.InvoiceID)
            });
        }

        /// <summary>
        /// Get Job Invoice Detail
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JsonResult GetJobInvoiceDetail(int invoiceId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            var estimateDto = estimateRepo.ListAll();
            var jobDto = jobRepo.GetJobInvoice(invoiceId);
            var estDto = estimateRepo.Select(jobDto.JobInvoice.EstimateID ?? 0);
            var unitOfMeasure = estDto.UnitOfMeasure.ToDictionary(k => k.Id);
            var assemblyItems = estDto.AssemblyItems.ToList();
            return Json(new
            {
                JobINV = jobDto.JobInvoice,
                keyValue = new
                {
                    originalEstimateItems = estDto.EstimateItems,
                    estimates = estimateDto.Estimates.Where(e => e.Status == (int) EstimateStatusType.Complete)
                        .Select(k => new {k.ID, k.JobAddressID, Description = k.Name}).OrderBy(k => k.Description),
                    items = estDto.Items.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.Name,
                        k.Code,
                        k.Name,
                        Unit = k.UnitOfMeasureId != 0 && unitOfMeasure.ContainsKey(k.UnitOfMeasureId)
                            ? unitOfMeasure[k.UnitOfMeasureId].Name
                            : string.Empty,
                        k.Price,
                        MaterialCost = 0,
                        LaborCost = 0
                    }).OrderBy(k => k.Description).ToList(),
                    assemblies = estDto.Assemblies.Select(k => new
                    {
                        ID = k.Id,
                        Description = k.Code + " " + k.AssemblyName,
                        k.Code,
                        Name = k.AssemblyName,
                        Price = k.MaterialCost,
                        k.MaterialCost,
                        k.LaborCost,
                        ItemNames = string.Join("<br/>",
                            from p in assemblyItems
                            join q in estDto.Items on p.ItemId equals q.Id
                            where p.AssemblyId == k.Id && q.ItemTypeId == (int) Core.Enums.ItemType.Material
                            select q.Name)
                    }).OrderBy(k => k.Description).ToList()
                },
                estimateItems =
                    (from p in jobDto.InvoiceItems join a in estDto.Assemblies on p.ItemAssemblyID equals a.Id select p)
                    .Select(k => new
                    {
                        k.ID,
                        k.InvoiceID,
                        k.ItemAssemblyID,
                        k.ItemAssemblyType,
                        k.Price, //Price = k.Price,
                        k.Quantity,
                        k.Cost,
                        LaborCost = 0,
                        MaterialCost = 0,
                        ItemNames = "",
                        k.COID
                    })
            });
        }

        /// <summary>
        /// Generate Invoice Document
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public FileContentResult CreateInvoiceDocument(int invoiceId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();

            var b = jobRepo.CreateInvoiceDocument(invoiceId);
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + b.Name + ";");
            return new FileContentResult(b.Text,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        #endregion

        #region "Job Inspection"

        /// <summary>
        /// Get Job Inspection list by jobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public ActionResult GetJobInspectionList(int jobId)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var inspList = jobRepo.GetJobInspections(jobId);
            var employDto = empRepo.GetEmployees();
            var permits = jobRepo.GetPermitList();
            var permitInspection = jobRepo.GetPermitInspectionList();
            var permitStatus = jobRepo.GetPermitStatusList();
            var jobDto = jobRepo.GetJobDetail(jobId);
            var jobAddress = jobRepo.GetJobAddress(jobId);
            return Json(new
            {
                keyValue = new
                {
                    Permits = inspList.Select(k => new
                    {
                        k.InspID,
                        k.JobID,
                        k.JobAddress,
                        k.EmployeeID,
                        EmployeeName = employDto.Employees.FirstOrDefault(p => p.EmpId == k.EmployeeID) != null
                            ? employDto.Employees.FirstOrDefault(p => p.EmpId == k.EmployeeID).FirstName + " " +
                              employDto.Employees.FirstOrDefault(p => p.EmpId == k.EmployeeID).LastName
                            : "",
                        Address = "",
                        k.PermitID,
                        Permit = permits.FirstOrDefault(p => p.ID == k.PermitID) != null
                            ? permits.FirstOrDefault(p => p.ID == k.PermitID).Description
                            : "",
                        k.Phone,
                        k.CalledDate,
                        k.ResultDate,
                        k.InspectionID,
                        Inspection = permitInspection.FirstOrDefault(p => p.ID == k.InspectionID) != null
                            ? permitInspection.FirstOrDefault(p => p.ID == k.InspectionID).Inspections
                            : "",
                        k.StatusID,
                        Status = permitStatus.FirstOrDefault(p => p.ID == k.StatusID) != null
                            ? permitStatus.Where(p => p.ID == k.StatusID).FirstOrDefault().Description
                            : "",
                        k.InspectorID,
                        InspectorName =
                            employDto.Employees.FirstOrDefault(p => p.EmpId == k.InspectorID) != null
                                ? employDto.Employees.FirstOrDefault(p => p.EmpId == k.InspectorID).FirstName +
                                  " " + employDto.Employees.FirstOrDefault(p => p.EmpId == k.InspectorID)
                                      .LastName
                                : "",
                        k.Notes,
                        k.Active
                    }).OrderByDescending(p => p.InspID),
                    employees = employDto.Employees.Where(e => e.Status == true)
                        .Select(k => new {ID = k.EmpId, Description = k.FirstName + " " + k.LastName})
                        .OrderBy(k => k.Description),
                    permits = permits.Where(e => e.Active).OrderBy(e => e.Description),
                    permitInspections = permitInspection.Where(e => e.Active)
                        .Select(k => new {k.ID, Description = k.Inspections}).OrderBy(k => k.Description),
                    permitStatus = permitStatus.Where(e => e.Active).OrderBy(k => k.Description),
                    job = jobDto.Jobs[0],
                    jobAddress
                }
            });
        }

        /// <summary>
        /// Toggle Job Inspection status
        /// </summary>
        /// <param name="inspId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public JsonResult ToggleJobInspectionStatus(int inspId, bool active)
        {
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasDeleted = jobRepo.ToggleJobInspectionStatus(inspId, active);
            return Json(new {hasDeleted});
        }

        /// <summary>
        /// Save Job Inspection
        /// </summary>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public JsonResult SaveJobInspection(JobData jobData)
        {
            var empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var hasSaved = true;
            var message = string.Empty;
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
            var employDto = empRepo.GetEmployees();
            var permits = jobRepo.GetPermitList();
            var permitInspection = jobRepo.GetPermitInspectionList();
            var permitStatus = jobRepo.GetPermitStatusList();
            return Json(new
            {
                hasSaved,
                message,
                Permits = inspList.Select(k => new
                {
                    k.InspID,
                    k.JobID,
                    k.JobAddress,
                    k.EmployeeID,
                    EmployeeName = employDto.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault() != null
                        ? employDto.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault().FirstName + " " +
                          employDto.Employees.Where(p => p.EmpId == k.EmployeeID).FirstOrDefault().LastName
                        : "",
                    Address = "",
                    k.PermitID,
                    Permit = permits.Where(p => p.ID == k.PermitID).FirstOrDefault() != null
                        ? permits.Where(p => p.ID == k.PermitID).FirstOrDefault().Description
                        : "",
                    k.Phone,
                    k.CalledDate,
                    k.ResultDate,
                    k.InspectionID,
                    Inspection = permitInspection.Where(p => p.ID == k.InspectionID).FirstOrDefault() != null
                        ? permitInspection.Where(p => p.ID == k.InspectionID).FirstOrDefault().Inspections
                        : "",
                    k.StatusID,
                    Status = permitStatus.Where(p => p.ID == k.StatusID).FirstOrDefault() != null
                        ? permitStatus.Where(p => p.ID == k.StatusID).FirstOrDefault().Description
                        : "",
                    k.InspectorID,
                    InspectorName = employDto.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault() != null
                        ? employDto.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault().FirstName + " " +
                          employDto.Employees.Where(p => p.EmpId == k.InspectorID).FirstOrDefault().LastName
                        : "",
                    k.Notes,
                    k.Active
                }).OrderByDescending(p => p.InspID),
            });
        }

        /// <summary>
        /// Get Job Inspection Detail
        /// </summary>
        /// <param name="inspId"></param>
        /// <returns></returns>
        public JsonResult GetJobInspectionDetail(int inspId, int jobId)
        {
            var empRepo = ObjectFactory.GetInstance<IEmployeeManagementRepository>();
            var jobRepo = ObjectFactory.GetInstance<IJobManagementRepository>();
            var jobDto = jobRepo.GetJobInspection(inspId);
            var permits = jobRepo.GetPermitList();
            var permitInspection = jobRepo.GetPermitInspectionList();
            var permitStatus = jobRepo.GetPermitStatusList();
            var employDto = empRepo.GetEmployees();
            return Json(new
            {
                jobDto.JobInspection,
                keyValue = new
                {
                    employees = employDto.Employees.Where(e => e.Status == true)
                        .Select(k => new {ID = k.EmpId, Description = k.FirstName + " " + k.LastName})
                        .OrderBy(k => k.Description),
                    permits = permits.Where(e => e.Active),
                    permitInspections = permitInspection.Where(e => e.Active)
                        .Select(k => new {k.ID, Description = k.Inspections}).OrderBy(k => k.Description),
                    permitStatus = permitStatus.Where(e => e.Active)
                }
            });
        }

        #endregion
    }
}