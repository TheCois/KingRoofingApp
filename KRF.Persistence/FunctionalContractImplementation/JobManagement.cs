using DapperExtensions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using KRF.Core.DTO.Job;
using KRF.Core.Entities.Customer;
using KRF.Core.Entities.Employee;
using KRF.Core.Entities.Master;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;
using KRF.Core.Entities.Product;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class JobManagement : IJobManagement
    {
        /// <summary>
        /// Get Job detail by JobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public JobDTO GetJobDetail(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var job = conn.Get<Job>(jobId);
                IList<Job> p = new List<Job>();
                p.Add(job);

                IList<Core.Entities.Sales.Lead> leads = conn.GetList<Core.Entities.Sales.Lead>().ToList();
                var customerAddress = conn.GetList<KRF.Core.Entities.Sales.CustomerAddress>().ToList();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<State>(s => s.Active, Operator.Eq, true));
                IList<State> states = conn.GetList<State>(predicateGroup).ToList();
                predicateGroup = new PredicateGroup {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(
                    Predicates.Field<KRF.Core.Entities.ValueList.City>(s => s.Active, Operator.Eq, true));
                IList<KRF.Core.Entities.ValueList.City> cities =
                    conn.GetList<KRF.Core.Entities.ValueList.City>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobAssignment>(s => s.JobID, Operator.Eq, jobId));
                IList<JobAssignment> jobAssignments = conn.GetList<JobAssignment>(predicateGroup).ToList();

                //predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<JobTask>(s => s.JobID, Operator.Eq, jobID));
                //IList<JobTask> jobTasks = conn.GetList<JobTask>(predicateGroup).ToList();

                //predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<JobPO>(s => s.JobID, Operator.Eq, jobID));
                //IList<JobPO> jobPOs = conn.GetList<JobPO>(predicateGroup).ToList();

                //predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
                //predicateGroup.Predicates.Add(Predicates.Field<JobCO>(s => s.JobID, Operator.Eq, jobID));
                //IList<JobCO> jobCOs = conn.GetList<JobCO>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<UnitOfMeasure>(s => s.Active, Operator.Eq, true));

                var itemMgt = ObjectFactory.GetInstance<IItemManagement>();
                var assemblyMgt = ObjectFactory.GetInstance<IAssemblyManagement>();

                return new JobDTO
                {
                    Jobs = p,
                    Leads = leads,
                    CustomerAddress = customerAddress,
                    Cities = cities,
                    States = states,
                    JobAssignments = jobAssignments,
                    Items = itemMgt.GetAllItems().ToList(),
                    Assemblies = assemblyMgt.GetAllAssemblies().ToList(),
                    UnitOfMeasures = conn.GetList<UnitOfMeasure>(predicateGroup).ToList()
                };
            }
        }

        /// <summary>
        /// Get Job List
        /// </summary>
        /// <returns></returns>
        public JobDTO GetJobs()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                IList<Job> jobs = conn.GetList<Job>().ToList();
                IList<Core.Entities.Sales.Lead> leads = conn.GetList<Core.Entities.Sales.Lead>().ToList();
                IList<Status> status = conn.GetList<Status>().ToList();
                return new JobDTO
                {
                    Jobs = jobs,
                    Leads = leads,
                    JobStatus = status
                };
            }
        }

        /// <summary>
        /// Create Job Information record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public int CreateJobInformation(Job job)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    job.JobStatusID = (int) Core.Entities.MISC.Status.New;
                    int id = conn.Insert(job);
                    var jobCurrent = conn.Get<Job>(id);
                    jobCurrent.JobCode = id.ToString();
                    conn.Update(jobCurrent);
                    var customerJobAdd =
                        conn.Get<KRF.Core.Entities.Sales.CustomerAddress>(job.JobAddressID);
                    if (customerJobAdd != null)
                    {
                        job.JobAddress1 = customerJobAdd.Address1;
                        job.JobAddress2 = customerJobAdd.Address2;
                        job.JobCity = customerJobAdd.City;
                        job.JobState = customerJobAdd.State;
                        job.JobZipCode = customerJobAdd.ZipCode;
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Update Job Information record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool EditJobInformation(Job job)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = job.Id;
                    var jobCurrent = conn.Get<Job>(id);
                    job.CreatedOn = jobCurrent.CreatedOn;
                    job.Status = jobCurrent.Status;
                    job.JobStatusID = jobCurrent.JobStatusID;
                    job.JobCode = jobCurrent.JobCode;
                    if (job.Description == null)
                    {
                        job.Description = jobCurrent.Description;
                    }

                    if (job.Title == null)
                    {
                        job.Title = jobCurrent.Title;
                    }

                    if (job.StartDate == null)
                    {
                        job.StartDate = jobCurrent.StartDate;
                    }

                    if (job.EndDate == null)
                    {
                        job.EndDate = jobCurrent.EndDate;
                    }

                    var custJobAdd =
                        conn.Get<KRF.Core.Entities.Sales.CustomerAddress>(job.JobAddressID);
                    if (custJobAdd != null)
                    {
                        job.JobAddress1 = custJobAdd.Address1;
                        job.JobAddress2 = custJobAdd.Address2;
                        job.JobCity = custJobAdd.City;
                        job.JobState = custJobAdd.State;
                        job.JobZipCode = custJobAdd.ZipCode;
                    }

                    var isEdited = conn.Update(job);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Update Job Summary
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool EditJobSummary(Job job)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = job.Id;
                    var jobCurrent = conn.Get<Job>(id);
                    jobCurrent.Notes = job.Notes;
                    jobCurrent.JobStatusID = job.JobStatusID;
                    jobCurrent.Status = job.Status;
                    jobCurrent.DateUpdated = DateTime.Now;

                    var isEdited = conn.Update(jobCurrent);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Edit Job Assignment record
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobAssignments"></param>
        /// <returns></returns>
        public bool EditJobAssignment(Job job, List<JobAssignment> jobAssignments)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = job.Id;
                    var jobCurrent = conn.Get<Job>(id);
                    jobCurrent.StartDate = job.StartDate;
                    jobCurrent.EndDate = job.EndDate;
                    jobCurrent.EstimatedLabourHours = job.EstimatedLabourHours;
                    jobCurrent.AverageWorkingHours = job.AverageWorkingHours;
                    jobCurrent.DateUpdated = DateTime.Now;

                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<JobAssignment>(s => s.JobID, Operator.Eq, id));
                    IList<JobAssignment> jobAsgmts = conn.GetList<JobAssignment>(predicateGroup).ToList();
                    foreach (var jobAsgmt in jobAsgmts)
                    {
                        conn.Delete(jobAsgmt);
                    }

                    if (jobAssignments != null)
                    {
                        foreach (var jobAssigment in jobAssignments)
                        {
                            jobAssigment.JobID = id;
                            conn.Insert(jobAssigment);
                        }
                    }

                    var isEdited = conn.Update(jobCurrent);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Get List of Job Tasks
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobTask> GetJobTasks(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())
            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobTask>(s => s.JobID, Operator.Eq, jobId));
                IList<JobTask> jobTasks = conn.GetList<JobTask>(predicateGroup).ToList();
                return jobTasks.ToList();
            }
        }

        /// <summary>
        /// Create Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        public int CreateJobTask(JobTask jobTask)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    jobTask.DateCreated = DateTime.Now;
                    if (jobTask.TaskCompletedDate != null && jobTask.TaskCompletedDate.Value.Year > 1970)
                    {
                        jobTask.IsCompleted = true;
                    }
                    else
                    {
                        jobTask.IsCompleted = false;
                    }

                    var id = conn.Insert(jobTask);

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        public bool EditJobTask(JobTask jobTask)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var id = jobTask.TaskID;
                    var jobTaskCurrent = conn.Get<JobTask>(id);
                    jobTask.DateCreated = jobTaskCurrent.DateCreated;
                    if (jobTask.TaskCompletedDate != null && jobTask.TaskCompletedDate.Value.Year > 1970)
                    {
                        jobTask.IsCompleted = true;
                    }
                    else
                    {
                        jobTask.IsCompleted = false;
                    }

                    jobTask.DateUpdated = DateTime.Now;

                    var isEdited = conn.Update(jobTask);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Set Task status
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(int taskId, bool isCompleted)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var jobTaskCurrent = conn.Get<JobTask>(taskId);
                    jobTaskCurrent.IsCompleted = isCompleted;
                    jobTaskCurrent.DateUpdated = DateTime.Now;

                    var isEdited = conn.Update(jobTaskCurrent);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Calculate Estimated Labor Hours
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobStartDate"></param>
        /// <param name="jobAssigments"></param>
        /// <returns></returns>
        public decimal CalculateEstimatedLaborCost(int jobId, DateTime jobStartDate, List<JobAssignment> jobAssigments)
        {
            decimal totalLaborCost = 0;
            decimal estimatedLaborHours = 0;
            decimal totalHourlyRate = 0;
            try
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())
                {
                    conn.Open();
                    var job = conn.Get<Job>(jobId);
                    if (job != null)
                    {
                        var estimate =
                            conn.Get<Core.Entities.Sales.Estimate>(job.EstimateID);
                        if (estimate != null)
                        {
                            totalLaborCost = estimate.TotalLaborCost;
                        }
                    }

                    foreach (var jobAssignment in jobAssigments)
                    {
                        if (jobAssignment.Type == "E")
                        {
                            var emp = conn.Get<Employee>(jobAssignment.ObjectPKID);
                            if (emp != null)
                            {
                                totalHourlyRate += Convert.ToDecimal(emp.HourlyRate);
                            }
                        }
                        else if (jobAssignment.Type == "C")
                        {
                            var predicateGroup = new PredicateGroup
                                {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                            predicateGroup.Predicates.Add(Predicates.Field<CrewDetail>(s => s.CrewID, Operator.Eq,
                                jobAssignment.ObjectPKID));
                            IList<CrewDetail> crewDetails = conn.GetList<CrewDetail>(predicateGroup).ToList();
                            foreach (var crewDetail in crewDetails)
                            {
                                if (crewDetail.Active)
                                {
                                    var emp = conn.Get<Employee>(crewDetail.EmpId);
                                    if (emp != null)
                                    {
                                        totalHourlyRate += Convert.ToDecimal(emp.HourlyRate);
                                    }
                                }
                            }
                        }
                    }

                    if (totalHourlyRate > 0)
                    {
                        estimatedLaborHours = totalLaborCost / totalHourlyRate;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return estimatedLaborHours;
        }

        /// <summary>
        /// Calculate Job End Date from estimated Labor hours
        /// </summary>
        /// <param name="jobStartDate"></param>
        /// <param name="estimatedWorkgHours"></param>
        /// <param name="avgWorkingHours"></param>
        /// <returns></returns>
        public DateTime CalculateJobEndDate(DateTime jobStartDate, decimal estimatedWorkgHours, int avgWorkingHours)
        {
            var jobEndDate = new DateTime();
            try
            {
                var totalHours = estimatedWorkgHours / avgWorkingHours;
                var intPart = 0;
                decimal decimalPart = 0;
                intPart = (int) decimal.Truncate(totalHours);
                decimalPart = totalHours - decimal.Truncate(intPart);
                var businessDays = intPart;
                if (decimalPart > 0)
                {
                    businessDays += 1;
                }

                jobEndDate = CalculateBusinessDaysFromInputDate(jobStartDate, businessDays);
                //if(businessDays>0)
                //{
                //    jobEndDate = jobEndDate.AddDays(-1);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return jobEndDate;
        }

        public DateTime CalculateBusinessDaysFromInputDate(DateTime startDate, int numberOfBusinessDays)
        {
            //Knock the start date down one day if it is on a weekend.
            if (startDate.DayOfWeek == DayOfWeek.Saturday |
                startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                numberOfBusinessDays -= 1;
            }

            if (startDate.DayOfWeek == DayOfWeek.Monday |
                startDate.DayOfWeek == DayOfWeek.Tuesday |
                startDate.DayOfWeek == DayOfWeek.Wednesday |
                startDate.DayOfWeek == DayOfWeek.Thursday |
                startDate.DayOfWeek == DayOfWeek.Friday)
            {
                numberOfBusinessDays = numberOfBusinessDays - 1;
            }

            int index;

            for (index = 1; index <= numberOfBusinessDays; index++)
            {
                switch (startDate.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        startDate = startDate.AddDays(2);
                        break;
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        startDate = startDate.AddDays(1);
                        break;
                    case DayOfWeek.Saturday:
                        startDate = startDate.AddDays(3);
                        break;
                }
            }

            //check to see if the end date is on a weekend.
            //If so move it ahead to Monday.
            //You could also bump it back to the Friday before if you desired to. 
            //Just change the code to -2 and -1.
            if (startDate.DayOfWeek == DayOfWeek.Saturday)
            {
                startDate = startDate.AddDays(2);
            }
            else if (startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startDate = startDate.AddDays(1);
            }

            return startDate;
        }

        /// <summary>
        /// Mark job status as complete
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleJobStatus(int jobId, bool tobeEnabled)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var job = conn.Get<Job>(jobId);
                if (tobeEnabled)
                {
                    job.JobStatusID = (int) Core.Entities.MISC.Status.Complete;
                }

                job.Status = tobeEnabled;
                job.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(job);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get List of Job POs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobPO> GetJobPOs(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobPO>(s => s.JobID, Operator.Eq, jobId));
                predicateGroup.Predicates.Add(Predicates.Field<JobPO>(s => s.Active, Operator.Eq, true));
                IList<JobPO> jobPOs = conn.GetList<JobPO>(predicateGroup).ToList();
                return jobPOs.ToList();
            }
        }

        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="poId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobPOStatus(int poId, bool active)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobPo = conn.Get<JobPO>(poId);
                jobPo.Active = active;
                jobPo.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(jobPo);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get List of Job PO
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public JobDTO GetJobPO(int poId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobPo = conn.Get<JobPO>(poId);
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<POEstimateItem>(s => s.POID, Operator.Eq, poId));
                IList<POEstimateItem> jobPoItems = conn.GetList<POEstimateItem>(predicateGroup).ToList();

                return new JobDTO()
                {
                    JobPO = jobPo,
                    POEstimateItems = jobPoItems.ToList()
                };
            }
        }

        /// <summary>
        /// Create Job PO
        /// </summary>
        /// <param name="jobPo"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobPO(JobPO jobPo, List<POEstimateItem> poEstimateItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    conn.Open();
                    jobPo.DateCreated = DateTime.Now;
                    jobPo.Active = true;

                    var jobPoLast = conn.GetList<JobPO>().OrderByDescending(p => p.POID).FirstOrDefault();
                    var pocodeid = 0;
                    if (jobPoLast != null)
                    {
                        if (jobPoLast.POCode.Contains("-"))
                        {
                            pocodeid = Convert.ToInt32(jobPoLast.POCode.Split('-')[1]);
                        }
                    }

                    var poCode = "PO-";
                    pocodeid = (pocodeid == 0 ? 1000 : (pocodeid + 1));
                    poCode += pocodeid;
                    jobPo.POCode = poCode;
                    var id = conn.Insert(jobPo);

                    foreach (var poItem in poEstimateItems)
                    {
                        poItem.POID = id;
                        conn.Insert(poItem);
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit Job PO
        /// </summary>
        /// <param name="jobPo"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobPO(JobPO jobPo, List<POEstimateItem> poEstimateItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    var id = jobPo.POID;
                    var jobPoCurrent = conn.Get<JobPO>(id);
                    jobPo.DateCreated = jobPoCurrent.DateCreated;
                    jobPo.Active = jobPoCurrent.Active;
                    jobPo.DateUpdated = DateTime.Now;


                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<POEstimateItem>(s => s.POID, Operator.Eq, id));
                    IList<POEstimateItem> jobPoItems = conn.GetList<POEstimateItem>(predicateGroup).ToList();
                    foreach (var jobPoItem in jobPoItems)
                    {
                        conn.Delete(jobPoItem);
                    }

                    if (poEstimateItems != null)
                    {
                        foreach (var poItem in poEstimateItems)
                        {
                            poItem.POID = id;
                            conn.Insert(poItem);
                        }
                    }

                    var isEdited = conn.Update(jobPo);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        public JobDocument GetJobDocumentByType(int type)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobDocument>(s => s.Type, Operator.Eq, type));

                var jobDocument = conn.GetList<JobDocument>(predicateGroup).First();
                return jobDocument;
            }
        }

        public byte[] CreatePODocument(int poId)
        {
            JobPO jobPo = null;
            Job job = null;
            KRF.Core.Entities.Sales.CustomerAddress custAdd = null;
            Core.Entities.Sales.Lead lead = null;
            var poEstimates = new List<POEstimateItem>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            KRF.Core.Entities.ValueList.City city = null;
            State state = null;
            Vendor vendor = null;

            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();

                jobPo = conn.Get<JobPO>(poId);
                job = conn.Get<Job>(jobPo.JobID);
                custAdd = conn.Get<KRF.Core.Entities.Sales.CustomerAddress>(job.JobAddressID);
                city = conn.Get<KRF.Core.Entities.ValueList.City>(custAdd.City);
                state = conn.Get<State>(custAdd.State);
                lead = conn.Get<Core.Entities.Sales.Lead>(job.LeadID);
                vendor = conn.Get<Vendor>(jobPo.VendorID);

                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<POEstimateItem>(s => s.POID, Operator.Eq, jobPo.POID));

                poEstimates = conn.GetList<POEstimateItem>(predicateGroup).ToList();
            }

            var estDto = estimateRepo.Select(jobPo.EstimateID ?? 0);

            var document = GetJobDocumentByType(-1); // -1 = Purchase Order Document
            var byteArray = document.Text;

            using (var mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, byteArray.Length);
                using (var wordDoc =
                    WordprocessingDocument.Open(mem, true))
                {
                    XNamespace w =
                        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";


                    IDictionary<string, BookmarkStart> bookmarkMap = new Dictionary<string, BookmarkStart>();

                    foreach (var bookmarkStart in wordDoc.MainDocumentPart.RootElement
                        .Descendants<BookmarkStart>())
                    {
                        bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                    }

                    foreach (var bookmarkStart in bookmarkMap.Values)
                    {
                        if (!bookmarkStart.Name.ToString().Trim().StartsWith("_"))
                        {
                            var bookmarkText = bookmarkStart.NextSibling<Run>();
                            if (bookmarkText != null)
                            {
                                if (bookmarkStart.Name.ToString() == "PONO")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobPo.POCode;
                                }

                                if (bookmarkStart.Name.ToString() == "CURRENTDATE")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = DateTime.Now.ToString("MM/dd/yyyy");
                                }

                                if (bookmarkStart.Name.ToString() == "VENDORNAME")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (vendor != null ? vendor.VendorName : "");
                                }

                                if (bookmarkStart.Name.ToString() == "VENDORADDRESS")
                                {
                                    var vendorAddress = (vendor != null ? vendor.VendorAddress : "");
                                    if (!string.IsNullOrEmpty(vendorAddress))
                                    {
                                        var jobAddresses = vendorAddress.Split('\n');
                                        var cnt = jobAddresses.Length;
                                        var index = 1;
                                        foreach (var jobAdd in jobAddresses)
                                        {
                                            bookmarkText.AppendChild(new Text(jobAdd));
                                            if (index < cnt)
                                            {
                                                bookmarkText.AppendChild(new Break());
                                            }

                                            index++;
                                        }
                                    }
                                }

                                if (bookmarkStart.Name.ToString() == "VENDORPHONE")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (vendor != null
                                        ? Common.EncryptDecrypt.formatPhoneNumber(vendor.Phone, "(###) ###-####")
                                        : "");
                                }

                                if (bookmarkStart.Name.ToString() == "VENDORFAX")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (vendor != null
                                        ? Common.EncryptDecrypt.formatPhoneNumber(vendor.Fax, "(###) ###-####")
                                        : "");
                                }

                                if (bookmarkStart.Name.ToString() == "VENDORCELL")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (vendor != null
                                        ? Common.EncryptDecrypt.formatPhoneNumber(vendor.Cell, "(###) ###-####")
                                        : "");
                                }

                                if (bookmarkStart.Name.ToString() == "DELIVERYDATE")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        string.Format("{0:MMMM dd, yyyy}", jobPo.DeliveryDate);
                                }

                                if (bookmarkStart.Name.ToString() == "VENDOREMAIL")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (vendor != null ? vendor.Email : "");
                                }

                                if (bookmarkStart.Name.ToString() == "VENDORREP")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobPo.VendorRep;
                                }

                                //Job Address
                                if (custAdd != null)
                                {
                                    if (bookmarkStart.Name.ToString() == "JOBADDRESS1")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = custAdd.Address1;
                                    }

                                    if (bookmarkStart.Name.ToString() == "JOBADDRESS2")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = custAdd.Address2;
                                    }

                                    if (bookmarkStart.Name.ToString() == "JOBSTATECITYZIP")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            city.Description + ", " + state.Abbreviation.Trim() + ", " +
                                            custAdd.ZipCode;
                                    }
                                }

                                //Job Info
                                if (bookmarkStart.Name.ToString() == "JOBNUMBER")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = job.Id.ToString();
                                }

                                if (bookmarkStart.Name.ToString() == "JOBNAME")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = job.Title;
                                }

                                if (bookmarkStart.Name.ToString() == "STORIES")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.NumberOfStories.ToString();
                                }

                                if (bookmarkStart.Name.ToString() == "ITEMTOTALAMOUNT")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobPo.TotalAmount.ToString();
                                }

                                if (bookmarkStart.Name.ToString() == "PERCENTTOTAL")
                                {
                                    var percentTotal = (jobPo.TotalAmount * 6) / 100;
                                    percentTotal = Math.Round(percentTotal, 2);
                                    var total = jobPo.TotalAmount + percentTotal;
                                    bookmarkText.GetFirstChild<Text>().Text = (percentTotal).ToString();

                                    var bookmarkSt = bookmarkMap["TOTAL"];
                                    bookmarkText = bookmarkSt.NextSibling<Run>();
                                    if (bookmarkText != null)
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            "$" + total.ToString("N", new CultureInfo("en-US"));
                                    }
                                }
                            }
                        }
                    }

                    var maxrows = poEstimates.Count() < 15 ? poEstimates.Count() : 15;
                    for (var rowindex = 1; rowindex <= maxrows; rowindex++)
                    {
                        var bookmarkStart = bookmarkMap["QTY" + rowindex];
                        var bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            //if (poEstimates[rowindex - 1].COID > 0)
                            //{
                            //    Color color = new Color();
                            //    color.Val = "3F48CC";
                            //    runProperties.AppendChild(color);
                            //}
                            runProperties.Append(new Text() {Text = poEstimates[rowindex - 1].Quantity.ToString()});
                        }

                        bookmarkStart = bookmarkMap["UM" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = ""; //TODO: get UOM
                        }

                        var itemCode = string.Empty;
                        var itemDesc = string.Empty;
                        if (poEstimates[rowindex - 1].ItemAssemblyType == 1)
                        {
                            var assembly = estDto.Assemblies.FirstOrDefault(p => p.Id == poEstimates[rowindex - 1].ItemAssemblyID);
                            if (assembly != null)
                            {
                                itemCode = assembly.Code;
                                itemDesc = assembly.AssemblyName;
                            }
                        }
                        else
                        {
                            var item = estDto.Items
                                .FirstOrDefault(p => p.Id == poEstimates[rowindex - 1].ItemAssemblyID);
                            if (item != null)
                            {
                                itemCode = item.Code;
                                itemDesc = item.Name;
                            }
                        }

                        bookmarkStart = bookmarkMap["ITEMCODE" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            //if (poEstimates[rowindex - 1].COID > 0)
                            //{
                            //    Color color = new Color();
                            //    color.Val = "3F48CC";
                            //    runProperties.AppendChild(color);
                            //}
                            runProperties.Append(new Text {Text = itemCode});
                            //bookmarkText.GetFirstChild<Text>().Text = itemCode;
                        }

                        bookmarkStart = bookmarkMap["ITEMDESC" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            //if (poEstimates[rowindex - 1].COID > 0)
                            //{
                            //    Color color = new Color();
                            //    color.Val = "3F48CC";
                            //    runProperties.AppendChild(color);
                            //}
                            runProperties.Append(new Text() {Text = itemDesc});
                            //bookmarkText.GetFirstChild<Text>().Text = itemDesc;
                        }

                        bookmarkStart = bookmarkMap["ITEMCOST" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            //if (poEstimates[rowindex - 1].COID > 0)
                            //{
                            //    Color color = new Color();
                            //    color.Val = "3F48CC";
                            //    runProperties.AppendChild(color);
                            //}
                            runProperties.Append(new Text() {Text = poEstimates[rowindex - 1].Price.ToString()});
                            //bookmarkText.GetFirstChild<Text>().Text = poEstimates[rowindex - 1].Price.ToString();
                        }

                        bookmarkStart = bookmarkMap["ITEMAMOUNT" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            //if (poEstimates[rowindex - 1].COID > 0)
                            //{
                            //    Color color = new Color();
                            //    color.Val = "3F48CC";
                            //    runProperties.AppendChild(color);
                            //}
                            runProperties.Append(new Text() {Text = poEstimates[rowindex - 1].Cost.ToString()});
                            //bookmarkText.GetFirstChild<Text>().Text = poEstimates[rowindex - 1].Cost.ToString();
                        }
                    }
                }

                var e = new JobDocument
                {
                    JobID = jobPo.JobID,
                    Name = "Purchase Order.docx",
                    Description = "Purchase Order",
                    UploadDateTime = DateTime.Now,
                    Text = mem.ToArray()
                };

                return e.Text;
            }
        }

        /// <summary>
        /// Get List of Job CO
        /// </summary>
        /// <param name="coId"></param>
        /// <returns></returns>
        public JobDTO GetJobCO(int coId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobCo = conn.Get<JobCO>(coId);
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<COEstimateItem>(s => s.COID, Operator.Eq, coId));
                IList<COEstimateItem> jobCoItems = conn.GetList<COEstimateItem>(predicateGroup).ToList();

                return new JobDTO()
                {
                    JobCO = jobCo,
                    COEstimateItems = jobCoItems.ToList()
                };
            }
        }

        /// <summary>
        /// Get List of Job COs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobCO> GetJobCOs(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobCO>(s => s.JobID, Operator.Eq, jobId));
                predicateGroup.Predicates.Add(Predicates.Field<JobCO>(s => s.Active, Operator.Eq, true));
                IList<JobCO> jobCOs = conn.GetList<JobCO>(predicateGroup).ToList();
                return jobCOs.ToList();
            }
        }

        /// <summary>
        /// Create Job CO
        /// </summary>
        /// <param name="jobCo"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobCO(JobCO jobCo, List<COEstimateItem> coEstimateItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    conn.Open();
                    jobCo.DateCreated = DateTime.Now;
                    jobCo.Active = true;
                    jobCo.SalesRepID = (jobCo.SalesRepID == 0 ? null : jobCo.SalesRepID);

                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<JobCO>(s => s.JobID, Operator.Eq, jobCo.JobID));
                    var jobCoLast = conn.GetList<JobCO>(predicateGroup).OrderByDescending(p => p.COID)
                        .FirstOrDefault();
                    var coCode = "CO-";
                    var cocodeid = 0;
                    if (jobCoLast != null)
                    {
                        if (jobCoLast.COCode.Contains("-"))
                        {
                            cocodeid = Convert.ToInt32(jobCoLast.COCode.Split('-')[1]);
                        }
                    }
                    else
                    {
                        cocodeid += 1;
                    }

                    if (cocodeid < 10)
                    {
                        coCode += "0" + (cocodeid + 1);
                    }

                    jobCo.COCode = coCode;
                    var id = conn.Insert(jobCo);

                    foreach (var coItem in coEstimateItems)
                    {
                        coItem.COID = id;
                        conn.Insert(coItem);
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit Job CO
        /// </summary>
        /// <param name="jobCo"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobCO(JobCO jobCo, List<COEstimateItem> coEstimateItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    var id = jobCo.COID;
                    var jobCoCurrent = conn.Get<JobCO>(id);
                    jobCo.DateCreated = jobCoCurrent.DateCreated;
                    jobCo.Active = jobCoCurrent.Active;
                    jobCo.DateUpdated = DateTime.Now;
                    jobCo.SalesRepID = (jobCo.SalesRepID == 0 ? null : jobCo.SalesRepID);


                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<COEstimateItem>(s => s.COID, Operator.Eq, id));
                    IList<COEstimateItem> jobCoItems = conn.GetList<COEstimateItem>(predicateGroup).ToList();
                    foreach (var jobCoItem in jobCoItems)
                    {
                        conn.Delete(jobCoItem);
                    }

                    if (coEstimateItems != null)
                    {
                        foreach (var coItem in coEstimateItems)
                        {
                            coItem.COID = id;
                            conn.Insert(coItem);
                        }
                    }

                    var isEdited = conn.Update(jobCo);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Toggle job CO status
        /// </summary>
        /// <param name="coId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobCOStatus(int coId, bool active)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobCo = conn.Get<JobCO>(coId);
                jobCo.Active = active;
                jobCo.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(jobCo);
                return isUpdated;
            }
        }

        /// <summary>
        /// Created Job Change Order document
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        public byte[] CreateCODocument(int coId)
        {
            JobCO jobCo;
            Job job;
            Core.Entities.Sales.Lead customer;
            KRF.Core.Entities.Sales.CustomerAddress customerAddress;
            List<COEstimateItem> coEstimates;
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            KRF.Core.Entities.ValueList.City city;
            State state;
            KRF.Core.Entities.ValueList.City customerCity;
            State customerState;
            Employee rep;
            List<Item> items;
            List<Assembly> assemblies;

            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();

                jobCo = conn.Get<JobCO>(coId);
                job = conn.Get<Job>(jobCo.JobID);
                customer = conn.Get<Core.Entities.Sales.Lead>(job.LeadID);
                customerAddress = conn.Get<KRF.Core.Entities.Sales.CustomerAddress>(job.JobAddressID);
                city = conn.Get<KRF.Core.Entities.ValueList.City>(customerAddress.City);
                state = conn.Get<State>(customerAddress.State);
                customerCity = conn.Get<KRF.Core.Entities.ValueList.City>(customer.BillCity);
                customerState = conn.Get<State>(customer.BillState);
                items = conn.GetList<Item>().ToList();
                assemblies = conn.GetList<Assembly>().ToList();


                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<COEstimateItem>(s => s.COID, Operator.Eq, jobCo.COID));
                coEstimates = conn.GetList<COEstimateItem>(predicateGroup).ToList();

                predicateGroup = new PredicateGroup {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Employee>(s => s.EmpId, Operator.Eq, jobCo.SalesRepID));
                rep = conn.GetList<Employee>(predicateGroup).FirstOrDefault();

            }

            var document = GetJobDocumentByType(-2); // -2 = Change Order Document
            var byteArray = document.Text;

            using (var mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, byteArray.Length);
                using (var wordDoc =
                    WordprocessingDocument.Open(mem, true))
                {
                    XNamespace w =
                        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";


                    IDictionary<string, BookmarkStart> bookmarkMap = new Dictionary<string, BookmarkStart>();

                    foreach (var bookmarkStart in wordDoc.MainDocumentPart.RootElement
                        .Descendants<BookmarkStart>())
                    {
                        bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                    }

                    foreach (var bookmarkStart in bookmarkMap.Values)
                    {
                        if (!bookmarkStart.Name.ToString().Trim().StartsWith("_"))
                        {
                            var bookmarkText = bookmarkStart.NextSibling<Run>();
                            if (bookmarkText != null)
                            {
                                if (bookmarkStart.Name.ToString() == "CONO")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobCo.COCode;
                                }

                                if (bookmarkStart.Name.ToString() == "DATE")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobCo.Date.ToShortDateString();
                                }

                                if (bookmarkStart.Name.ToString() == "CUSTNAME")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = customer.BusinessName;
                                }

                                if (bookmarkStart.Name.ToString() == "CUSTADDRESS")
                                {
                                    if (customer != null)
                                    {
                                        bookmarkText.AppendChild(new Text(customer.BillAddress1));
                                        if (!string.IsNullOrEmpty(customer.BillAddress2))
                                        {
                                            bookmarkText.AppendChild(new Break());
                                            bookmarkText.AppendChild(new Text(customer.BillAddress2));
                                        }

                                        bookmarkText.AppendChild(new Break());
                                        bookmarkText.AppendChild(new Text(
                                            (customerCity != null ? customerCity.Description : "") + ", " +
                                            (customerState != null ? customerState.Abbreviation.Trim() : "") + " " +
                                            customer.BillZipCode));
                                    }
                                }

                                if (bookmarkStart.Name.ToString() == "PHONE")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (customer != null
                                        ? Common.EncryptDecrypt.formatPhoneNumber(customer.Telephone, "(###) ###-####")
                                        : "");
                                }

                                if (bookmarkStart.Name.ToString() == "SALESREPEMAIL1")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobCo.SalesRepEmail;
                                }

                                if (bookmarkStart.Name.ToString() == "SALESREP")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        (rep != null ? (rep.FirstName + " " + rep.LastName) : string.Empty);
                                }

                                //Job Address
                                if (bookmarkStart.Name.ToString() == "ADDRESS")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (customerAddress != null ? customerAddress.Address1 : "");
                                }

                                if (bookmarkStart.Name.ToString() == "CITYSTATEZIP")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        (city != null ? city.Description + ", " : "") +
                                        (state != null ? state.Abbreviation.Trim() : "") + " " +
                                        (customerAddress != null ? customerAddress.ZipCode : "");
                                }

                                //Job Info
                                if (bookmarkStart.Name.ToString() == "PROJECTNAME")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = job.Title;
                                }

                                if (bookmarkStart.Name.ToString() == "NAME")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = (customer != null ? customer.LeadName : "");
                                }

                                if (bookmarkStart.Name.ToString() == "TOTAL")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        "$" + jobCo.TotalAmount.ToString("N", new CultureInfo("en-US"));
                                }
                            }
                        }
                    }

                    var maxrows = coEstimates.Count() < 15 ? coEstimates.Count() : 15;
                    for (var rowindex = 1; rowindex <= maxrows; rowindex++)
                    {
                        var bookmarkStart = bookmarkMap["QTY" + rowindex];
                        var bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = coEstimates[rowindex - 1].Quantity.ToString();
                        }

                        var itemCode = string.Empty;
                        var itemDesc = string.Empty;
                        if (coEstimates[rowindex - 1].ItemAssemblyType == 1)
                        {
                            var assembly = assemblies
                                .FirstOrDefault(p => p.Id == coEstimates[rowindex - 1].ItemAssemblyID);
                            if (assembly != null)
                            {
                                itemCode = assembly.Code;
                                itemDesc = assembly.AssemblyName;
                            }
                        }
                        else
                        {
                            var item = Enumerable.Where(items, p => p.Id == coEstimates[rowindex - 1].ItemAssemblyID)
                                .FirstOrDefault();
                            if (item != null)
                            {
                                itemCode = item.Code;
                                itemDesc = item.Name;
                            }
                        }

                        bookmarkStart = bookmarkMap["DESC" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = itemCode + " - " + itemDesc;
                        }

                        bookmarkStart = bookmarkMap["COST" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = coEstimates[rowindex - 1].Price.ToString();
                        }

                        bookmarkStart = bookmarkMap["AMOUNT" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = coEstimates[rowindex - 1].Cost.ToString();
                        }
                    }
                }

                var e = new JobDocument
                {
                    //JobID = jobCO.JobID,
                    //Name = "Change Order.docx",
                    //Description = "Change Order",
                    //UploadDateTime = DateTime.Now,
                    Text = mem.ToArray()
                };

                return e.Text;
            }
        }

        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <returns></returns>
        public List<JobDocumentType> GetJobDocumentTypes()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                IList<JobDocumentType> jobDocumentTypes = conn.GetList<JobDocumentType>().ToList();
                return jobDocumentTypes.ToList();
            }
        }

        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobDocument> GetJobDocuments(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobDocument>(s => s.JobID, Operator.Eq, jobId));
                predicateGroup.Predicates.Add(Predicates.Field<JobDocument>(s => s.Type, Operator.Gt, 0));
                IList<JobDocument> jobDocuments = conn.GetList<JobDocument>(predicateGroup).ToList();
                return jobDocuments.ToList();
            }
        }

        /// <summary>
        /// Get a job document
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        public JobDocument GetJobDocument(int jobDocumentId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                var jobDocument = conn.Get<JobDocument>(jobDocumentId);
                return jobDocument;
            }
        }

        /// <summary>
        /// Save Job document
        /// </summary>
        /// <param name="jobDocument"></param>
        /// <returns></returns>
        public int SaveDocument(JobDocument jobDocument)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    conn.Open();

                    if (jobDocument.ID > 0)
                    {
                        var curDoc = conn.Get<JobDocument>(jobDocument.ID);
                        curDoc.Description = jobDocument.Description;
                        curDoc.Type = jobDocument.Type;

                        var isEdited = conn.Update(curDoc);
                    }
                    else
                    {
                        jobDocument.ID = conn.Insert(jobDocument);
                    }

                    transactionScope.Complete();
                    return jobDocument.ID;
                }
            }
        }

        /// <summary>
        /// Delete job document by jobDocumentID
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        public bool DeleteJobDocument(int jobDocumentId)
        {
            var isDeleted = false;
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<JobDocument>(s => s.ID, Operator.Eq, jobDocumentId));

                    conn.Open();
                    isDeleted = conn.Delete<JobDocument>(predicateGroup);
                }

                transactionScope.Complete();
                return isDeleted;
            }
        }

        /// <summary>
        /// Get Job Wo Detail
        /// </summary>
        /// <param name="woId"></param>
        /// <returns></returns>
        public JobDTO GetJobWO(int woId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobWo = conn.Get<JobWO>(woId);
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<WOEstimateItem>(s => s.WOID, Operator.Eq, woId));
                IList<WOEstimateItem> jobWoItems = conn.GetList<WOEstimateItem>(predicateGroup).ToList();

                return new JobDTO()
                {
                    JobWO = jobWo,
                    WOEstimateItems = jobWoItems.ToList()
                };
            }
        }

        /// <summary>
        /// Set Job WO status to active/inactive
        /// </summary>
        /// <param name="woId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobWOStatus(int woId, bool active)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobWo = conn.Get<JobWO>(woId);
                jobWo.Active = active;
                jobWo.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(jobWo);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get JobAssignment List
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobAssignment> GetJobAssignments(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobAssignment>(s => s.JobID, Operator.Eq, jobId));
                IList<JobAssignment> jobAssignments = conn.GetList<JobAssignment>(predicateGroup).ToList();
                return jobAssignments.ToList();
            }
        }

        /// <summary>
        /// Get JobAssignment Crew Leader List
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<vw_JobAssignmentCrewLeaders> GetJobAssignmentCrewLeaders(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(
                    Predicates.Field<vw_JobAssignmentCrewLeaders>(s => s.JobID, Operator.Eq, jobId));
                IList<vw_JobAssignmentCrewLeaders> vwJobAssignmentCrewLeaders =
                    conn.GetList<vw_JobAssignmentCrewLeaders>(predicateGroup).ToList();
                return vwJobAssignmentCrewLeaders.ToList();
            }
        }

        /// <summary>
        /// Get List of Job WOs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobWO> GetJobWOs(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobWO>(s => s.JobID, Operator.Eq, jobId));
                predicateGroup.Predicates.Add(Predicates.Field<JobWO>(s => s.Active, Operator.Eq, true));
                IList<JobWO> jobWOs = conn.GetList<JobWO>(predicateGroup).ToList();
                return jobWOs.ToList();
            }
        }

        /// <summary>
        /// Create Job WO
        /// </summary>
        /// <param name="jobWo"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobWO(JobWO jobWo, List<WOEstimateItem> woEstimateItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    var totalJobBalanceAmount = woEstimateItems.Sum(p => p.Budget * p.Rate);
                    conn.Open();
                    jobWo.DateCreated = DateTime.Now;
                    jobWo.Active = true;
                    jobWo.TotalJobBalanceAmount = totalJobBalanceAmount;
                    int id = conn.Insert(jobWo);

                    //Generate WO ID
                    var jobWoCurrent = conn.Get<JobWO>(id);
                    jobWoCurrent.WOCode = "WO-" + id;
                    conn.Update(jobWoCurrent);

                    foreach (var woItem in woEstimateItems)
                    {
                        woItem.WOID = id;
                        conn.Insert(woItem);
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit JobWO
        /// </summary>
        /// <param name="jobWo"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobWO(JobWO jobWo, List<WOEstimateItem> woEstimateItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    var totalJobBalanceAmount = woEstimateItems.Sum(p => p.Budget * p.Rate);
                    var id = jobWo.WOID;
                    var jobWoCurrent = conn.Get<JobWO>(id);
                    jobWo.DateCreated = jobWoCurrent.DateCreated;
                    jobWo.Active = jobWoCurrent.Active;
                    jobWo.DateUpdated = DateTime.Now;
                    jobWo.TotalJobBalanceAmount = totalJobBalanceAmount;

                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<WOEstimateItem>(s => s.WOID, Operator.Eq, id));
                    IList<WOEstimateItem> jobWoItems = conn.GetList<WOEstimateItem>(predicateGroup).ToList();
                    foreach (var jobWoItem in jobWoItems)
                    {
                        conn.Delete(jobWoItem);
                    }

                    if (woEstimateItems != null)
                    {
                        foreach (var woItem in woEstimateItems)
                        {
                            woItem.WOID = id;
                            conn.Insert(woItem);
                        }
                    }

                    var isEdited = conn.Update(jobWo);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        public JobDocument CreateWODocument(int woId)
        {
            JobWO jobWo = null;
            Job job = null;
            KRF.Core.Entities.Sales.CustomerAddress custSiteAdd = null;
            Core.Entities.Sales.Lead lead = null;
            var roofTypes = new List<RoofType>();
            var woEstimates = new List<WOEstimateItem>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            KRF.Core.Entities.ValueList.City city = null;
            State state = null;
            KRF.Core.Entities.ValueList.City custCity = null;
            State custState = null;
            Employee emp = null;

            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();

                jobWo = conn.Get<JobWO>(woId);
                job = conn.Get<Job>(jobWo.JobID);
                custSiteAdd = conn.Get<KRF.Core.Entities.Sales.CustomerAddress>(job.JobAddressID);
                city = conn.Get<KRF.Core.Entities.ValueList.City>(custSiteAdd.City);
                state = conn.Get<State>(custSiteAdd.State);
                emp = conn.Get<Employee>(jobWo.LeadID);
                roofTypes = conn.GetList<RoofType>().ToList();

                lead = conn.Get<Core.Entities.Sales.Lead>(job.LeadID);
                custCity = conn.Get<KRF.Core.Entities.ValueList.City>(lead.BillCity);
                custState = conn.Get<State>(lead.BillState);

                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<WOEstimateItem>(s => s.WOID, Operator.Eq, jobWo.WOID));

                woEstimates = conn.GetList<WOEstimateItem>(predicateGroup).ToList();
            }

            var estDto = estimateRepo.Select(jobWo.EstimateID ?? 0);

            var document = GetJobDocumentByType(-3); // -3 = Work Order Document
            var byteArray = document.Text;

            using (var mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, byteArray.Length);
                using (var wordDoc =
                    WordprocessingDocument.Open(mem, true))
                {
                    XNamespace w =
                        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";


                    IDictionary<string, BookmarkStart> bookmarkMap = new Dictionary<string, BookmarkStart>();

                    foreach (var bookmarkStart in wordDoc.MainDocumentPart.RootElement
                        .Descendants<BookmarkStart>())
                    {
                        bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                    }

                    foreach (var bookmarkStart in bookmarkMap.Values)
                    {
                        if (!bookmarkStart.Name.ToString().Trim().StartsWith("_"))
                        {
                            var bookmarkText = bookmarkStart.NextSibling<Run>();
                            if (bookmarkText != null)
                            {
                                if (bookmarkStart.Name.ToString() == "CustContactName")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.LeadName;
                                }

                                if (bookmarkStart.Name.ToString() == "CustAddress1")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.BillAddress1;
                                }

                                if (bookmarkStart.Name.ToString() == "CustCityStateZip")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        custCity.Description.Trim() + ", " + custState.Abbreviation.Trim() + " " +
                                        lead.BillZipCode;
                                }

                                if (bookmarkStart.Name.ToString() == "WO2")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobWo.WOCode;
                                }

                                if (bookmarkStart.Name.ToString() == "JobName")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = job.Title;
                                }

                                if (bookmarkStart.Name.ToString() == "LeadName")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = emp.FirstName + " " + emp.LastName;
                                }

                                if (bookmarkStart.Name.ToString() == "TotalJobBalanceAmount")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        "$" + jobWo.TotalJobBalanceAmount.ToString("N", new CultureInfo("en-US"));
                                }

                                if (bookmarkStart.Name.ToString() == "TOTALAMOUNT")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = "";
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        "$" + jobWo.TotalAmount.ToString("N", new CultureInfo("en-US"));
                                }

                                if (bookmarkStart.Name.ToString() == "EMPTOTALAMOUNT")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        "$" + jobWo.TotalAmount.ToString("N", new CultureInfo("en-US"));
                                }

                                if (bookmarkStart.Name.ToString() == "CurrentDate")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        string.Format("{0:MM/dd/yyyy}", DateTime.Now);
                                }

                                //Job Address
                                if (custSiteAdd != null)
                                {
                                    if (bookmarkStart.Name.ToString() == "SiteContactName")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = lead.LeadName;
                                    }

                                    if (bookmarkStart.Name.ToString() == "SiteAddress1")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = custSiteAdd.Address1;
                                    }

                                    if (bookmarkStart.Name.ToString() == "SiteCityStateZip")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            city.Description + ", " + state.Abbreviation.Trim() + " " +
                                            custSiteAdd.ZipCode;
                                    }
                                }

                                if (bookmarkStart.Name.ToString() == "WorkWeekEnding")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        string.Format("{0:MMMM dd, yyyy}", jobWo.WorkWeekEndingDate);
                                }

                                if (bookmarkStart.Name.ToString() == "RoofType")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = roofTypes
                                        .Where(p => p.ID == lead.RoofType).FirstOrDefault().Description;
                                }
                            }
                        }
                    }

                    var maxrows = woEstimates.Count() < 21 ? woEstimates.Count() : 21;
                    decimal totalBalance = 0;
                    for (var rowindex = 1; rowindex <= maxrows; rowindex++)
                    {
                        var bookmarkStart = bookmarkMap["BAL" + rowindex];
                        var bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var totalJob = woEstimates[rowindex - 1].Budget * woEstimates[rowindex - 1].Rate;
                            var totalBal = totalJob * woEstimates[rowindex - 1].Amount;
                            totalBalance += totalBal;
                            bookmarkText.GetFirstChild<Text>().Text =
                                "$" + Math.Round(totalBal, 2).ToString("N", new CultureInfo("en-US"));
                        }

                        var itemDesc = string.Empty;
                        if (woEstimates[rowindex - 1].ItemAssemblyType == 1)
                        {
                            var assembly = estDto.Assemblies.FirstOrDefault(p => p.Id == woEstimates[rowindex - 1].ItemAssemblyID);
                            if (assembly != null)
                            {
                                itemDesc = assembly.AssemblyName;
                            }
                        }
                        else
                        {
                            var item = estDto.Items
                                .FirstOrDefault(p => p.Id == woEstimates[rowindex - 1].ItemAssemblyID);
                            if (item != null)
                            {
                                itemDesc = item.Name;
                            }
                        }

                        bookmarkStart = bookmarkMap["DESC" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            runProperties.Append(new Text() {Text = itemDesc});
                        }

                        bookmarkStart = bookmarkMap["BUDGET" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            runProperties.Append(new Text()
                                {Text = woEstimates[rowindex - 1].Budget.ToString(CultureInfo.InvariantCulture)});
                        }

                        bookmarkStart = bookmarkMap["USED" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            runProperties.Append(new Text()
                                {Text = woEstimates[rowindex - 1].Used.ToString(CultureInfo.InvariantCulture)});
                        }

                        bookmarkStart = bookmarkMap["RATE" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            runProperties.Append(new Text()
                                {
                                Text = Math.Round(woEstimates[rowindex - 1].Rate, 2)
                                    .ToString(CultureInfo.InvariantCulture)
                            });
                        }

                        bookmarkStart = bookmarkMap["BALANCE" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            runProperties.Append(new Text()
                                {Text = woEstimates[rowindex - 1].Balance.ToString(CultureInfo.InvariantCulture)});
                        }

                        bookmarkStart = bookmarkMap["AMOUNT" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            bookmarkText.GetFirstChild<Text>().Text =
                                "$" + Math.Round(woEstimates[rowindex - 1].Amount, 2)
                                    .ToString("N", new CultureInfo("en-US"));
                        }
                    }

                    var bookmarkStartTotalBal = bookmarkMap["TOTALBAL"];
                    var bookmarkTextTotalBal = bookmarkStartTotalBal.NextSibling<Run>();
                    if (bookmarkTextTotalBal != null)
                    {
                        bookmarkTextTotalBal.GetFirstChild<Text>().Text =
                            "$" + totalBalance.ToString("N", new CultureInfo("en-US"));
                    }
                }

                var e = new JobDocument
                {
                    JobID = jobWo.JobID,
                    Name = "WorkOrder_" + jobWo.WorkWeekEndingDate.ToString("dd-MMM-yyyy") + ".docx",
                    Description = "Work Order",
                    UploadDateTime = DateTime.Now,
                    Text = mem.ToArray()
                };

                return e;
            }
        }

        /// <summary>
        /// Get List of Job Invoices
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobInvoice> GetJobInvoices(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobInvoice>(s => s.JobID, Operator.Eq, jobId));
                predicateGroup.Predicates.Add(Predicates.Field<JobInvoice>(s => s.Active, Operator.Eq, true));
                IList<JobInvoice> jobInvoices = conn.GetList<JobInvoice>(predicateGroup).ToList();
                return jobInvoices.ToList();
            }
        }

        /// <summary>
        /// Create Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        public int CreateJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    conn.Open();
                    jobInvoice.DateCreated = DateTime.Now;
                    jobInvoice.Active = true;
                    var id = conn.Insert(jobInvoice);

                    foreach (var invoiceItem in invoiceItems)
                    {
                        invoiceItem.InvoiceID = id;
                        conn.Insert(invoiceItem);
                    }

                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        public bool EditJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    var id = jobInvoice.InvoiceID;
                    var jobInvoiceCurrent = conn.Get<JobInvoice>(id);
                    jobInvoice.DateCreated = jobInvoiceCurrent.DateCreated;
                    jobInvoice.Active = jobInvoiceCurrent.Active;
                    jobInvoice.DateUpdated = DateTime.Now;


                    var predicateGroup = new PredicateGroup
                        {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                    predicateGroup.Predicates.Add(Predicates.Field<InvoiceItems>(s => s.InvoiceID, Operator.Eq, id));
                    IList<InvoiceItems> jobInvoiceItems = conn.GetList<InvoiceItems>(predicateGroup).ToList();
                    foreach (var jobInvoiceItem in jobInvoiceItems)
                    {
                        conn.Delete(jobInvoiceItem);
                    }

                    if (invoiceItems != null)
                    {
                        foreach (var invoiceItem in invoiceItems)
                        {
                            invoiceItem.InvoiceID = id;
                            conn.Insert(invoiceItem);
                        }
                    }

                    var isEdited = conn.Update(jobInvoice);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Get Job Invoice Detail
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JobDTO GetJobInvoice(int invoiceId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobInvoice = conn.Get<JobInvoice>(invoiceId);
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<InvoiceItems>(s => s.InvoiceID, Operator.Eq, invoiceId));
                IList<InvoiceItems> jobInvoiceItems = conn.GetList<InvoiceItems>(predicateGroup).ToList();

                return new JobDTO()
                {
                    JobInvoice = jobInvoice,
                    InvoiceItems = jobInvoiceItems.ToList()
                };
            }
        }

        /// <summary>
        /// Toggle Job Invoice status
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobInvoiceStatus(int invoiceId, bool active)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobInvoice = conn.Get<JobInvoice>(invoiceId);
                jobInvoice.Active = active;
                jobInvoice.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(jobInvoice);
                return isUpdated;
            }
        }

        /// <summary>
        /// Create Invoice Document
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JobDocument CreateInvoiceDocument(int invoiceId)
        {
            JobInvoice jobInvoice = null;
            Job job = null;
            KRF.Core.Entities.Sales.CustomerAddress custSiteAdd = null;
            var invoiceItems = new List<InvoiceItems>();
            var estimateRepo = ObjectFactory.GetInstance<IEstimateManagementRepository>();
            KRF.Core.Entities.ValueList.City city = null;
            State state = null;
            KRF.Core.Entities.ValueList.City custCity = null;
            State custState = null;
            Core.Entities.Sales.Lead lead = null;
            Employee emp = null;

            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();

                jobInvoice = conn.Get<JobInvoice>(invoiceId);
                job = conn.Get<Job>(jobInvoice.JobID);
                custSiteAdd = conn.Get<KRF.Core.Entities.Sales.CustomerAddress>(job.JobAddressID);
                city = conn.Get<KRF.Core.Entities.ValueList.City>(custSiteAdd.City);
                state = conn.Get<State>(custSiteAdd.State);

                lead = conn.Get<Core.Entities.Sales.Lead>(job.LeadID);
                custCity = conn.Get<KRF.Core.Entities.ValueList.City>(lead.BillCity);
                custState = conn.Get<State>(lead.BillState);

                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<InvoiceItems>(s => s.InvoiceID, Operator.Eq,
                    jobInvoice.InvoiceID));

                invoiceItems = conn.GetList<InvoiceItems>(predicateGroup).ToList();
            }

            var estDto = estimateRepo.Select(jobInvoice.EstimateID ?? 0);

            var document = GetJobDocumentByType(-4); // -4 = Invoice Document
            var byteArray = document.Text;

            using (var mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, byteArray.Length);
                using (var wordDoc =
                    WordprocessingDocument.Open(mem, true))
                {
                    XNamespace w =
                        "http://schemas.openxmlformats.org/wordprocessingml/2006/main";


                    IDictionary<string, BookmarkStart> bookmarkMap = new Dictionary<string, BookmarkStart>();

                    foreach (var bookmarkStart in wordDoc.MainDocumentPart.RootElement
                        .Descendants<BookmarkStart>())
                    {
                        bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                    }

                    foreach (var bookmarkStart in bookmarkMap.Values)
                    {
                        if (!bookmarkStart.Name.ToString().Trim().StartsWith("_"))
                        {
                            var bookmarkText = bookmarkStart.NextSibling<Run>();
                            if (bookmarkText != null)
                            {
                                if (bookmarkStart.Name.ToString() == "InvoiceNo")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = jobInvoice.InvoiceCode;
                                }

                                if (bookmarkStart.Name.ToString() == "InvoiceDate")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        $"{jobInvoice.InvoiceDate:MMMM dd, yyyy}";
                                }

                                if (bookmarkStart.Name.ToString() == "CustomerContact")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.LeadName;
                                }

                                if (bookmarkStart.Name.ToString() == "CustomerAddress1")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.BillAddress1;
                                }

                                if (bookmarkStart.Name.ToString() == "CustomerCityStateZip")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        custCity.Description.Trim() + ", " + custState.Abbreviation.Trim() + " " +
                                        lead.BillZipCode;
                                }

                                if (bookmarkStart.Name.ToString() == "CustomerEmail")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text = lead.Email ?? "";
                                }

                                if (bookmarkStart.Name.ToString() == "CustomerPhone")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        Common.EncryptDecrypt.formatPhoneNumber(lead.Telephone, "(###) ###-####");
                                }

                                if (bookmarkStart.Name.ToString() == "CustomerFax")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        Common.EncryptDecrypt.formatPhoneNumber(lead.Telephone, "(###) ###-####");
                                }

                                if (bookmarkStart.Name.ToString() == "CustomerCell")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        Common.EncryptDecrypt.formatPhoneNumber(lead.Telephone, "(###) ###-####");
                                }

                                if (bookmarkStart.Name.ToString() == "TOTALAMOUNT")
                                {
                                    bookmarkText.GetFirstChild<Text>().Text =
                                        "$" + jobInvoice.TotalAmount.ToString("N", new CultureInfo("en-US"));
                                }

                                //Job Address
                                if (custSiteAdd != null)
                                {
                                    if (bookmarkStart.Name.ToString() == "JobSiteContactName")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = lead.LeadName;
                                    }

                                    if (bookmarkStart.Name.ToString() == "JobSiteAddress1")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = custSiteAdd.Address1;
                                    }

                                    if (bookmarkStart.Name.ToString() == "JobSiteAddress2")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = custSiteAdd.Address2;
                                    }

                                    if (bookmarkStart.Name.ToString() == "JobSiteCityStateZip")
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text =
                                            city.Description + ", " + state.Abbreviation.Trim() + " " +
                                            custSiteAdd.ZipCode;
                                    }
                                }
                            }
                        }
                    }

                    var maxrows = invoiceItems.Count() < 21 ? invoiceItems.Count() : 21;
                    for (var rowindex = 1; rowindex <= maxrows; rowindex++)
                    {
                        var bookmarkStart = bookmarkMap["QTY" + rowindex];
                        var bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text =
                                Convert.ToString(invoiceItems[rowindex - 1].Quantity, CultureInfo.InvariantCulture);
                        }

                        var itemDesc = string.Empty;
                        if (invoiceItems[rowindex - 1].ItemAssemblyType == 1)
                        {
                            var assembly = estDto.Assemblies.FirstOrDefault(p =>
                                invoiceItems != null && p.Id == invoiceItems[rowindex - 1].ItemAssemblyID);
                            if (assembly != null)
                            {
                                itemDesc = assembly.AssemblyName;
                            }
                        }
                        else
                        {
                            var item = estDto.Items
                                .FirstOrDefault(p => p.Id == invoiceItems[rowindex - 1].ItemAssemblyID);
                            if (item != null)
                            {
                                itemDesc = item.Name;
                            }
                        }

                        bookmarkStart = bookmarkMap["DESC" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            runProperties.Append(new Text {Text = itemDesc});
                        }

                        bookmarkStart = bookmarkMap["UC" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            var runProperties = bookmarkText.AppendChild(new RunProperties());
                            runProperties.Append(new Text {Text = invoiceItems[rowindex - 1].Price.ToString()});
                        }

                        bookmarkStart = bookmarkMap["AMOUNT" + rowindex];
                        bookmarkText = bookmarkStart.NextSibling<Run>();
                        if (bookmarkText != null)
                        {
                            bookmarkText.GetFirstChild<Text>().Text = "";
                            bookmarkText.GetFirstChild<Text>().Text =
                                "$" + Math.Round(invoiceItems[rowindex - 1].Cost, 2)
                                    .ToString("N", new CultureInfo("en-US"));
                        }
                    }
                }

                var e = new JobDocument
                {
                    JobID = jobInvoice.JobID,
                    Name = "Invoice_" + jobInvoice.InvoiceDate.ToString("dd-MMM-yyyy") + ".docx",
                    Description = "Invoice",
                    UploadDateTime = DateTime.Now,
                    Text = mem.ToArray()
                };

                return e;
            }
        }

        /// <summary>
        /// Get List of Job Inspection
        /// </summary>
        /// <param name="inspId"></param>
        /// <returns></returns>
        public JobDTO GetJobInspection(int inspId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobInspection = conn.Get<JobInspection>(inspId);

                return new JobDTO()
                {
                    JobInspection = jobInspection
                };
            }
        }

        /// <summary>
        /// Toggle job Inspection status
        /// </summary>
        /// <param name="inspId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobInspectionStatus(int inspId, bool active)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var jobInspection = conn.Get<JobInspection>(inspId);
                jobInspection.Active = active;
                jobInspection.DateUpdated = DateTime.Now;
                var isUpdated = conn.Update(jobInspection);
                return isUpdated;
            }
        }

        /// <summary>
        /// Get List of Job Inspections
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobInspection> GetJobInspections(int jobId)
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<JobInspection>(s => s.JobID, Operator.Eq, jobId));
                predicateGroup.Predicates.Add(Predicates.Field<JobInspection>(s => s.Active, Operator.Eq, true));
                IList<JobInspection> jobInspections = conn.GetList<JobInspection>(predicateGroup).ToList();
                return jobInspections.ToList();
            }
        }

        /// <summary>
        /// Create Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        public int CreateJobInspection(JobInspection jobInspection)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    conn.Open();
                    jobInspection.DateCreated = DateTime.Now;
                    jobInspection.ResultDate = Convert.ToDateTime(jobInspection.ResultDate).Year < 1000
                        ? null
                        : jobInspection.ResultDate;
                    jobInspection.StatusID = (jobInspection.StatusID == 0 ? null : jobInspection.StatusID);
                    jobInspection.Active = true;
                    int id = conn.Insert(jobInspection);
                    transactionScope.Complete();
                    return id;
                }
            }
        }

        /// <summary>
        /// Edit Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        public bool EditJobInspection(JobInspection jobInspection)
        {
            using (var transactionScope = new TransactionScope())
            {
                var dbConnection = new DataAccessFactory();
                using (var conn = dbConnection.CreateConnection())

                {
                    var id = jobInspection.InspID;
                    var jobInspCurrent = conn.Get<JobInspection>(id);
                    jobInspection.DateCreated = jobInspCurrent.DateCreated;
                    jobInspection.Active = jobInspCurrent.Active;
                    jobInspection.DateUpdated = DateTime.Now;
                    jobInspection.ResultDate = Convert.ToDateTime(jobInspection.ResultDate).Year < 1000
                        ? null
                        : jobInspection.ResultDate;
                    jobInspection.StatusID = (jobInspection.StatusID == 0 ? null : jobInspection.StatusID);

                    var isEdited = conn.Update(jobInspection);
                    transactionScope.Complete();
                    return isEdited;
                }
            }
        }

        /// <summary>
        /// Get Permit List
        /// </summary>
        /// <returns></returns>
        public List<Permit> GetPermitList()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<Permit>(s => s.Active, Operator.Eq, true));
                IList<Permit> permits = conn.GetList<Permit>(predicateGroup).ToList();
                return permits.ToList();
            }
        }

        /// <summary>
        /// Get Permit Inspection List
        /// </summary>
        /// <returns></returns>
        public List<PermitInspection> GetPermitInspectionList()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<PermitInspection>(s => s.Active, Operator.Eq, true));
                IList<PermitInspection> permitInspection = conn.GetList<PermitInspection>(predicateGroup).ToList();
                return permitInspection.ToList();
            }
        }

        /// <summary>
        /// Get Permit Status List
        /// </summary>
        /// <returns></returns>
        public List<PermitStatus> GetPermitStatusList()
        {
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();
                var predicateGroup = new PredicateGroup
                    {Operator = GroupOperator.And, Predicates = new List<IPredicate>()};
                predicateGroup.Predicates.Add(Predicates.Field<PermitStatus>(s => s.Active, Operator.Eq, true));
                IList<PermitStatus> permitStatus = conn.GetList<PermitStatus>(predicateGroup).ToList();
                return permitStatus.ToList();
            }
        }

        public string GetJobAddress(int jobId)
        {
            Job job;
            KRF.Core.Entities.Sales.CustomerAddress custAdd;
            KRF.Core.Entities.ValueList.City city;
            State state;
            var jobAddress = string.Empty;
            var dbConnection = new DataAccessFactory();
            using (var conn = dbConnection.CreateConnection())

            {
                conn.Open();

                job = conn.Get<Job>(jobId);
                custAdd = conn.Get<KRF.Core.Entities.Sales.CustomerAddress>(job.JobAddressID);
                city = conn.Get<KRF.Core.Entities.ValueList.City>(custAdd.City);
                state = conn.Get<State>(custAdd.State);
                jobAddress = " " + custAdd.Address1.Trim() +
                             (custAdd.Address2 != null ? ",\n " + custAdd.Address2.Trim() : "") + ",\n " +
                             (city != null ? city.Description.Trim() : "") +
                             (state != null ? ", " + state.Abbreviation.Trim() : "") +
                             (custAdd.ZipCode != null ? ", " + custAdd.ZipCode.Trim() : "");
            }

            return jobAddress;
        }
    }
}