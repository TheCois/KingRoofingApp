using KRF.Core.DTO.Job;
using KRF.Core.Entities.Customer;
using KRF.Core.Entities.ValueList;
using System;
using System.Collections.Generic;

namespace KRF.Core.Repository
{
    public interface IJobManagementRepository
    {
        /// <summary>
        /// Create new job record in DB
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        int CreateJobInformation(Job job);
        /// <summary>
        /// Edit Job record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        bool EditJobInformation(Job job);
        /// <summary>
        /// Edit Job Summary
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        bool EditJobSummary(Job job);
        /// <summary>
        /// Edit Job Assignment records
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobAssignments"></param>
        /// <returns></returns>
        bool EditJobAssignment(Job job, List<JobAssignment> jobAssignments);
        /// <summary>
        /// Get List of Job Tasks
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobTask> GetJobTasks(int jobId);
        /// <summary>
        /// Create Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        int CreateJobTask(JobTask jobTask);
        /// <summary>
        /// Edit Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        bool EditJobTask(JobTask jobTask);
        /// <summary>
        /// Set Task status
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        bool UpdateTaskStatus(int jobId, bool isCompleted);
        /// <summary>
        /// Get Job Detail by JobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        JobDTO GetJobDetail(int jobID);
        /// <summary>
        /// Get Job Listing
        /// </summary>
        /// <returns></returns>
        JobDTO GetJobs();
        /// <summary>
        /// Calculate Estimated Labor Cost
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobStartDate"></param>
        /// <param name="jobAssignments"></param>
        /// <returns></returns>
        decimal CalculateEstimatedLaborCost(int jobId, DateTime jobStartDate, List<JobAssignment> jobAssignments);
        /// <summary>
        /// Calculate Job End Date from estimated Labor hours
        /// </summary>
        /// <param name="jobStartDate"></param>
        /// <param name="estimatedWorkingHours"></param>
        /// <param name="avgWorkingHours"></param>
        /// <returns></returns>
        DateTime CalculateJobEndDate(DateTime jobStartDate, decimal estimatedWorkingHours, int avgWorkingHours);
        /// <summary>
        /// Mark job status as complete
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        bool ToggleJobStatus(int jobId, bool tobeEnabled);
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="poId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobPOStatus(int poId, bool active);
        /// <summary>
        /// Get List of Job PO
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        JobDTO GetJobPO(int poId);
        /// <summary>
        /// Get List of Job POs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobPO> GetJobPOs(int jobId);
        /// <summary>
        /// Create Job PO
        /// </summary>
        /// <param name="jobPo"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        int CreateJobPO(JobPO jobPo, List<POEstimateItem> poEstimateItems);
        /// <summary>
        /// Edit Job PO
        /// </summary>
        /// <param name="jobPo"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        bool EditJobPO(JobPO jobPo, List<POEstimateItem> poEstimateItems);
        /// <summary>
        /// Create PO Document
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        byte[] CreatePODocument(int poId);
        /// <summary>
        /// Get List of Job CO
        /// </summary>
        /// <param name="coId"></param>
        /// <returns></returns>
        JobDTO GetJobCO(int coId);
        /// <summary>
        /// Get List of Job COs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobCO> GetJobCOs(int jobId);
        /// <summary>
        /// Create Job CO
        /// </summary>
        /// <param name="jobCo"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        int CreateJobCO(JobCO jobCo, List<COEstimateItem> coEstimateItems);
        /// <summary>
        /// Edit Job CO
        /// </summary>
        /// <param name="jobCo"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        bool EditJobCO(JobCO jobCo, List<COEstimateItem> coEstimateItems);
        /// <summary>
        /// Toggle job CO status
        /// </summary>
        /// <param name="coId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobCOStatus(int coId, bool active);
        /// <summary>
        /// Create CO Document
        /// </summary>
        /// <param name="coId"></param>
        /// <returns></returns>
        byte[] CreateCODocument(int coId);
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <returns></returns>
        List<JobDocumentType> GetJobDocumentTypes();
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobDocument> GetJobDocuments(int jobId);
        /// <summary>
        /// Get a job document
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        JobDocument GetJobDocument(int jobDocumentId);
        /// <summary>
        /// Save Job document
        /// </summary>
        /// <param name="jobDocument"></param>
        /// <returns></returns>
        int SaveDocument(JobDocument jobDocument);
        /// <summary>
        /// Delete job document by jobDocumentID
        /// </summary>
        /// <param name="jobDocumentId"></param>
        /// <returns></returns>
        bool DeleteJobDocument(int jobDocumentId);
        /// <summary>
        /// Get List of Job WO
        /// </summary>
        /// <param name="woId"></param>
        /// <returns></returns>
        JobDTO GetJobWO(int woId);
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="woId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobWOStatus(int woId, bool active);
        /// <summary>
        /// Get JobAssignment List
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobAssignment> GetJobAssignments(int jobId);
        /// <summary>
        /// Get JobAssignment Crew Leader List
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<vw_JobAssignmentCrewLeaders> GetJobAssignmentCrewLeaders(int jobId);
        /// <summary>
        /// Get List of Job WOs
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobWO> GetJobWOs(int jobId);
        /// <summary>
        /// Create Job WO
        /// </summary>
        /// <param name="jobWo"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        int CreateJobWO(JobWO jobWo, List<WOEstimateItem> woEstimateItems);
        /// <summary>
        /// Edit Job WO
        /// </summary>
        /// <param name="jobWo"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        bool EditJobWO(JobWO jobWo, List<WOEstimateItem> woEstimateItems);
        /// <summary>
        /// Create WO Document
        /// </summary>
        /// <param name="woId"></param>
        /// <returns></returns>
        JobDocument CreateWODocument(int woId);
        /// <summary>
        /// Get List of Job Invoices
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobInvoice> GetJobInvoices(int jobId);
        /// <summary>
        /// Create Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        int CreateJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems);
        /// <summary>
        /// Edit Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        bool EditJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems);
        /// <summary>
        /// Get Job Invoice Detail
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        JobDTO GetJobInvoice(int invoiceId);
        /// <summary>
        /// Toggle Job Invoice status
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobInvoiceStatus(int invoiceId, bool active);
        /// <summary>
        /// Create Invoice Document
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        JobDocument CreateInvoiceDocument(int invoiceId);

        /// <summary>
        /// Get List of Job Inspection
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <returns></returns>
        JobDTO GetJobInspection(int inspectionId);
        /// <summary>
        /// Toggle job Inspection status
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobInspectionStatus(int inspectionId, bool active);
        /// <summary>
        /// Get List of Job Inspections
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        List<JobInspection> GetJobInspections(int jobId);
        /// <summary>
        /// Create Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        int CreateJobInspection(JobInspection jobInspection);
        /// <summary>
        /// Edit Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        bool EditJobInspection(JobInspection jobInspection);
        /// <summary>
        /// Get Permit List
        /// </summary>
        /// <returns></returns>
        List<Permit> GetPermitList();
        /// <summary>
        /// Get Permit Inspection List
        /// </summary>
        /// <returns></returns>
        List<PermitInspection> GetPermitInspectionList();
        /// <summary>
        /// Get Permit Status List
        /// </summary>
        /// <returns></returns>
        List<PermitStatus> GetPermitStatusList();
        string GetJobAddress(int jobId);
    }
}
