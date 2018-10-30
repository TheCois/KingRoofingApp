using System;
using System.Collections.Generic;
using KRF.Core.DTO.Job;
using KRF.Core.Entities.Customer;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;

namespace KRF.Persistence.RepositoryImplementation
{
    public class JobManagementRepository : IJobManagementRepository
    {
        private readonly IJobManagement jobManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public JobManagementRepository()
        {
            jobManagement_ = ObjectFactory.GetInstance<IJobManagement>();
        }
        /// <summary>
        /// Get Job Detail by JobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public JobDTO GetJobDetail(int jobId)
        {
            return jobManagement_.GetJobDetail(jobId);
        }
        /// <summary>
        /// Get Job List
        /// </summary>
        /// <returns></returns>
        public JobDTO GetJobs()
        {
            return jobManagement_.GetJobs();
        }

        /// <summary>
        /// Create new Job Information record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public int CreateJobInformation(Job job)
        {
            return jobManagement_.CreateJobInformation(job);
        }
        /// <summary>
        /// Update Job Information record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool EditJobInformation(Job job)
        {
            return jobManagement_.EditJobInformation(job);
        }
        /// <summary>
        /// Update Job Summary record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool EditJobSummary(Job job)
        {
            return jobManagement_.EditJobSummary(job);
        }
        /// <summary>
        /// Get List of Job Tasks
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobTask> GetJobTasks(int jobId)
        {
            return jobManagement_.GetJobTasks(jobId);
        }
        /// <summary>
        /// Create Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        public int CreateJobTask(JobTask jobTask)
        {
            return jobManagement_.CreateJobTask(jobTask);
        }
        /// <summary>
        /// Edit Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        public bool EditJobTask(JobTask jobTask)
        {
            return jobManagement_.EditJobTask(jobTask);
        }
        /// <summary>
        /// Set Task status
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(int jobId, bool isCompleted)
        {
            return jobManagement_.UpdateTaskStatus(jobId, isCompleted);
        }
        /// <summary>
        /// Edit Job Assignment records
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobAssignments"></param>
        /// <returns></returns>
        public bool EditJobAssignment(Job job, List<JobAssignment> jobAssignments)
        {
            return jobManagement_.EditJobAssignment(job, jobAssignments);
        }

        /// <summary>
        /// Calculate Estimated Labor Hours
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobStartDate"></param>
        /// <param name="jobAssignments"></param>
        /// <returns></returns>
        public decimal CalculateEstimatedLaborCost(int jobId, DateTime jobStartDate, List<JobAssignment> jobAssignments)
        {
            return jobManagement_.CalculateEstimatedLaborCost(jobId, jobStartDate, jobAssignments);
        }
        /// <summary>
        /// Calculate Job End Date from estimated Labor hours
        /// </summary>
        /// <param name="jobStartDate"></param>
        /// <param name="estimatedWorkingHours"></param>
        /// <param name="avgWorkingHours"></param>
        /// <returns></returns>
        public DateTime CalculateJobEndDate(DateTime jobStartDate, decimal estimatedWorkingHours, int avgWorkingHours)
        {
            return jobManagement_.CalculateJobEndDate(jobStartDate, estimatedWorkingHours, avgWorkingHours);
        }
        /// <summary>
        /// Mark job status as complete
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleJobStatus(int jobId, bool tobeEnabled)
        {
            return jobManagement_.ToggleJobStatus(jobId, tobeEnabled);
        }
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="poId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobPOStatus(int poId, bool active)
        {
            return jobManagement_.ToggleJobPOStatus(poId, active);
        }
        /// <summary>
        /// Get List of Job PO
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public JobDTO GetJobPO(int poId)
        {
            return jobManagement_.GetJobPO(poId);
        }
        /// <summary>
        /// Get List of Job POs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobPO> GetJobPOs(int jobId)
        {
            return jobManagement_.GetJobPOs(jobId);
        }
        /// <summary>
        /// Create Job PO
        /// </summary>
        /// <param name="jobPo"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobPO(JobPO jobPo, List<POEstimateItem> poEstimateItems)
        {
            return jobManagement_.CreateJobPO(jobPo, poEstimateItems);
        }
        /// <summary>
        /// Edit Job PO
        /// </summary>
        /// <param name="jobPo"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobPO(JobPO jobPo, List<POEstimateItem> poEstimateItems)
        {
            return jobManagement_.EditJobPO(jobPo, poEstimateItems);
        }
        /// <summary>
        /// Create PO Document
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public byte[] CreatePODocument(int poId)
        {
            return jobManagement_.CreatePODocument(poId);
        }
        /// <summary>
        /// Get List of Job CO
        /// </summary>
        /// <param name="coId"></param>
        /// <returns></returns>
        public JobDTO GetJobCO(int coId)
        {
            return jobManagement_.GetJobCO(coId);
        }
        /// <summary>
        /// Get List of Job COs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobCO> GetJobCOs(int jobId)
        {
            return jobManagement_.GetJobCOs(jobId);
        }
        /// <summary>
        /// Create Job CO
        /// </summary>
        /// <param name="jobCo"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobCO(JobCO jobCo, List<COEstimateItem> coEstimateItems)
        {
            return jobManagement_.CreateJobCO(jobCo, coEstimateItems);
        }
        /// <summary>
        /// Edit Job CO
        /// </summary>
        /// <param name="jobCo"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobCO(JobCO jobCo, List<COEstimateItem> coEstimateItems)
        {
            return jobManagement_.EditJobCO(jobCo, coEstimateItems);
        }
        /// <summary>
        /// Toggle job CO status
        /// </summary>
        /// <param name="coId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobCOStatus(int coId, bool active)
        {
            return jobManagement_.ToggleJobCOStatus(coId, active);
        }
        /// <summary>
        /// Create CO Document
        /// </summary>
        /// <param name="coId"></param>
        /// <returns></returns>
        public byte[] CreateCODocument(int coId)
        {
            return jobManagement_.CreateCODocument(coId);
        }
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <returns></returns>
        public List<JobDocumentType> GetJobDocumentTypes()
        {
            return jobManagement_.GetJobDocumentTypes();
        }
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobDocument> GetJobDocuments(int jobId)
        {
            return jobManagement_.GetJobDocuments(jobId);
        }
        /// <summary>
        /// Get a job document
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        public JobDocument GetJobDocument(int jobDocumentId)
        {
            return jobManagement_.GetJobDocument(jobDocumentId);
        }
        /// <summary>
        /// Save Job document
        /// </summary>
        /// <param name="jobDocument"></param>
        /// <returns></returns>
        public int SaveDocument(JobDocument jobDocument)
        {
            return jobManagement_.SaveDocument(jobDocument);
        }
        /// <summary>
        /// Delete job document by jobDocumentID
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        public bool DeleteJobDocument(int jobDocumentId)
        {
            return jobManagement_.DeleteJobDocument(jobDocumentId);
        }
        /// <summary>
        /// Get List of Job WO
        /// </summary>
        /// <param name="woId"></param>
        /// <returns></returns>
        public JobDTO GetJobWO(int woId)
        {
            return jobManagement_.GetJobWO(woId);
        }
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="woId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobWOStatus(int woId, bool active)
        {
            return jobManagement_.ToggleJobWOStatus(woId, active);
        }
        /// <summary>
        /// Get JobAssignment List
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobAssignment> GetJobAssignments(int jobId)
        {
            return jobManagement_.GetJobAssignments(jobId);
        }
        /// <summary>
        /// Get JobAssignment Crew Leader List
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<vw_JobAssignmentCrewLeaders> GetJobAssignmentCrewLeaders(int jobId)
        {
            return jobManagement_.GetJobAssignmentCrewLeaders(jobId);
        }

        /// <summary>
        /// Get List of Job WOs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobWO> GetJobWOs(int jobId)
        {
            return jobManagement_.GetJobWOs(jobId);
        }
        /// <summary>
        /// Create Job WO
        /// </summary>
        /// <param name="jobWo"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobWO(JobWO jobWo, List<WOEstimateItem> woEstimateItems)
        {
            return jobManagement_.CreateJobWO(jobWo, woEstimateItems);
        }
        /// <summary>
        /// Edit Job WO
        /// </summary>
        /// <param name="jobWo"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobWO(JobWO jobWo, List<WOEstimateItem> woEstimateItems)
        {
            return jobManagement_.EditJobWO(jobWo, woEstimateItems);
        }
        /// <summary>
        /// Create WO Document
        /// </summary>
        /// <param name="woId"></param>
        /// <returns></returns>
        public JobDocument CreateWODocument(int woId)
        {
            return jobManagement_.CreateWODocument(woId);
        }
        /// <summary>
        /// Get List of Job Invoices
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobInvoice> GetJobInvoices(int jobId)
        {
            return jobManagement_.GetJobInvoices(jobId);
        }
        /// <summary>
        /// Create Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        public int CreateJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems)
        {
            return jobManagement_.CreateJobInvoice(jobInvoice, invoiceItems);
        }

        /// <summary>
        /// Edit Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        public bool EditJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems)
        {
            return jobManagement_.EditJobInvoice(jobInvoice, invoiceItems);
        }
        /// <summary>
        /// Get Job Invoice Detail
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JobDTO GetJobInvoice(int invoiceId)
        {
            return jobManagement_.GetJobInvoice(invoiceId);
        }
        /// <summary>
        /// Toggle Job Invoice status
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobInvoiceStatus(int invoiceId, bool active)
        {
            return jobManagement_.ToggleJobInvoiceStatus(invoiceId, active);
        }
        /// <summary>
        /// Create Invoice Document
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JobDocument CreateInvoiceDocument(int invoiceId)
        {
            return jobManagement_.CreateInvoiceDocument(invoiceId);
        }

        /// <summary>
        /// Get List of Job Inspection
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <returns></returns>
        public JobDTO GetJobInspection(int inspectionId)
        {
            return jobManagement_.GetJobInspection(inspectionId);
        }
        /// <summary>
        /// Toggle job Inspection status
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobInspectionStatus(int inspectionId, bool active)
        {
            return jobManagement_.ToggleJobInspectionStatus(inspectionId, active);
        }
        /// <summary>
        /// Get List of Job Inspections
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobInspection> GetJobInspections(int jobId)
        {
            return jobManagement_.GetJobInspections(jobId);
        }
        /// <summary>
        /// Create Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        public int CreateJobInspection(JobInspection jobInspection)
        {
            return jobManagement_.CreateJobInspection(jobInspection);
        }
        /// <summary>
        /// Edit Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        public bool EditJobInspection(JobInspection jobInspection)
        {
            return jobManagement_.EditJobInspection(jobInspection);
        }
        /// <summary>
        /// Get Permit List
        /// </summary>
        /// <returns></returns>
        public List<Permit> GetPermitList()
        {
            return jobManagement_.GetPrmitList();
        }
        /// <summary>
        /// Get Permit Inspection List
        /// </summary>
        /// <returns></returns>
        public List<PermitInspection> GetPermitInspectionList()
        {
            return jobManagement_.GetPrmitInspectionList();
        }
        /// <summary>
        /// Get Permit Status List
        /// </summary>
        /// <returns></returns>
        public List<PermitStatus> GetPermitStatusList()
        {
            return jobManagement_.GetPrmitStatusList();
        }
        public string GetJobAddress(int jobId)
        {
            return jobManagement_.GetJobAddress(jobId);
        }
    }
}
