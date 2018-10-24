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
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobTask> GetJobTasks(int jobID);
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
        /// <param name="taskID"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        bool UpdateTaskStatus(int taskID, bool isCompleted);
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
        /// <param name="jobID"></param>
        /// <param name="jobStartDate"></param>
        /// <param name="jobAssigments"></param>
        /// <returns></returns>
        decimal CalculateEstimatedLaborCost(int jobID, DateTime jobStartDate, List<JobAssignment> jobAssigments);
        /// <summary>
        /// Calculate Job End Date from estimated Labor hours
        /// </summary>
        /// <param name="jobStartDate"></param>
        /// <param name="estimatedWorkgHours"></param>
        /// <param name="avgWorkingHours"></param>
        /// <returns></returns>
        DateTime CalculateJobEndDate(DateTime jobStartDate, decimal estimatedWorkgHours, int avgWorkingHours);
        /// <summary>
        /// Mark job status as complete
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        bool ToggleJobStatus(int jobID, bool tobeEnabled);
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="poID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobPOStatus(int poID, bool active);
        /// <summary>
        /// Get List of Job PO
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        JobDTO GetJobPO(int poID);
        /// <summary>
        /// Get List of Job POs
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobPO> GetJobPOs(int jobID);
        /// <summary>
        /// Create Job PO
        /// </summary>
        /// <param name="jobPO"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        int CreateJobPO(JobPO jobPO, List<POEstimateItem> poEstimateItems);
        /// <summary>
        /// Edit Job PO
        /// </summary>
        /// <param name="jobPO"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        bool EditJobPO(JobPO jobPO, List<POEstimateItem> poEstimateItems);
        /// <summary>
        /// Create PO Document
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        byte[] CreatePODocument(int poID);
        /// <summary>
        /// Get List of Job CO
        /// </summary>
        /// <param name="coID"></param>
        /// <returns></returns>
        JobDTO GetJobCO(int coID);
        /// <summary>
        /// Get List of Job COs
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobCO> GetJobCOs(int jobID);
        /// <summary>
        /// Create Job CO
        /// </summary>
        /// <param name="jobCO"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        int CreateJobCO(JobCO jobCO, List<COEstimateItem> coEstimateItems);
        /// <summary>
        /// Edit Job CO
        /// </summary>
        /// <param name="jobCO"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        bool EditJobCO(JobCO jobCO, List<COEstimateItem> coEstimateItems);
        /// <summary>
        /// Toggle job CO status
        /// </summary>
        /// <param name="coID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobCOStatus(int coID, bool active);
        /// <summary>
        /// Create CO Document
        /// </summary>
        /// <param name="coID"></param>
        /// <returns></returns>
        byte[] CreateCODocument(int coID);
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <returns></returns>
        List<JobDocumentType> GetJobDocumentTypes();
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobDocument> GetJobDocuments(int jobID);
        /// <summary>
        /// Get a job document
        /// </summary>
        /// <param name="jobDocumentID"></param>
        /// <returns></returns>
        JobDocument GetJobDocument(int jobDocumentID);
        /// <summary>
        /// Save Job document
        /// </summary>
        /// <param name="jobDocument"></param>
        /// <returns></returns>
        int SaveDocument(JobDocument jobDocument);
        /// <summary>
        /// Delete job document by jobDocumentID
        /// </summary>
        /// <param name="jobDocumentID"></param>
        /// <returns></returns>
        bool DeleteJobDocument(int jobDocumentID);
        /// <summary>
        /// Get List of Job WO
        /// </summary>
        /// <param name="woID"></param>
        /// <returns></returns>
        JobDTO GetJobWO(int woID);
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="woID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobWOStatus(int woID, bool active);
        /// <summary>
        /// Get JobAssignment List
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobAssignment> GetJobAssignments(int jobID);
        /// <summary>
        /// Get JobAssignment Crew Leader List
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<vw_JobAssignmentCrewLeaders> GetJobAssignmentCrewLeaders(int jobID);
        /// <summary>
        /// Get List of Job WOs
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobWO> GetJobWOs(int jobID);
        /// <summary>
        /// Create Job WO
        /// </summary>
        /// <param name="jobWO"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        int CreateJobWO(JobWO jobWO, List<WOEstimateItem> woEstimateItems);
        /// <summary>
        /// Edit Job WO
        /// </summary>
        /// <param name="jobWO"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        bool EditJobWO(JobWO jobWO, List<WOEstimateItem> woEstimateItems);
        /// <summary>
        /// Create WO Document
        /// </summary>
        /// <param name="woID"></param>
        /// <returns></returns>
        JobDocument CreateWODocument(int woID);
        /// <summary>
        /// Get List of Job Invoices
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobInvoice> GetJobInvoices(int jobID);
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
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        JobDTO GetJobInvoice(int invoiceID);
        /// <summary>
        /// Toggle Job Invoice status
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobInvoiceStatus(int invoiceID, bool active);
        /// <summary>
        /// Create Invoice Document
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        JobDocument CreateInvoiceDocument(int invoiceID);

        /// <summary>
        /// Get List of Job Inspection
        /// </summary>
        /// <param name="inspID"></param>
        /// <returns></returns>
        JobDTO GetJobInspection(int inspID);
        /// <summary>
        /// Toggle job Inspection status
        /// </summary>
        /// <param name="inspID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool ToggleJobInspectionStatus(int inspID, bool active);
        /// <summary>
        /// Get List of Job Inspections
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        List<JobInspection> GetJobInspections(int jobID);
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
        List<Permit> GetPrmitList();
        /// <summary>
        /// Get Permit Inspection List
        /// </summary>
        /// <returns></returns>
        List<PermitInspection> GetPrmitInspectionList();
        /// <summary>
        /// Get Permit Status List
        /// </summary>
        /// <returns></returns>
        List<PermitStatus> GetPrmitStatusList();
        string GetJobAddress(int jobID);
    }
}
