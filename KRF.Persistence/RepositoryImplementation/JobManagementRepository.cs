using KRF.Core.DTO.Job;
using KRF.Core.Entities.Customer;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class JobManagementRepository : IJobManagementRepository
    {
        private readonly IJobManagement _JobManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public JobManagementRepository()
        {
            _JobManagement = ObjectFactory.GetInstance<IJobManagement>();
        }
        /// <summary>
        /// Get Job Detail by JobID
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public JobDTO GetJobDetail(int jobID)
        {
            return _JobManagement.GetJobDetail(jobID);
        }
        /// <summary>
        /// Get Job List
        /// </summary>
        /// <returns></returns>
        public JobDTO GetJobs()
        {
            return _JobManagement.GetJobs();
        }

        /// <summary>
        /// Create new Job Information record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public int CreateJobInformation(Job job)
        {
            return _JobManagement.CreateJobInformation(job);
        }
        /// <summary>
        /// Update Job Information record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool EditJobInformation(Job job)
        {
            return _JobManagement.EditJobInformation(job);
        }
        /// <summary>
        /// Update Job Summary record
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool EditJobSummary(Job job)
        {
            return _JobManagement.EditJobSummary(job);
        }
        /// <summary>
        /// Get List of Job Tasks
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobTask> GetJobTasks(int jobID)
        {
            return _JobManagement.GetJobTasks(jobID);
        }
        /// <summary>
        /// Create Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        public int CreateJobTask(JobTask jobTask)
        {
            return _JobManagement.CreateJobTask(jobTask);
        }
        /// <summary>
        /// Edit Job Task
        /// </summary>
        /// <param name="jobTask"></param>
        /// <returns></returns>
        public bool EditJobTask(JobTask jobTask)
        {
            return _JobManagement.EditJobTask(jobTask);
        }
        /// <summary>
        /// Set Task status
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(int jobID, bool isCompleted)
        {
            return _JobManagement.UpdateTaskStatus(jobID, isCompleted);
        }
        /// <summary>
        /// Edit Job Assignment records
        /// </summary>
        /// <param name="job"></param>
        /// <param name="jobAssignments"></param>
        /// <returns></returns>
        public bool EditJobAssignment(Job job, List<JobAssignment> jobAssignments)
        {
            return _JobManagement.EditJobAssignment(job, jobAssignments);
        }

        /// <summary>
        /// Calculate Estimated Labor Hours
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="jobStartDate"></param>
        /// <param name="jobAssigments"></param>
        /// <returns></returns>
        public decimal CalculateEstimatedLaborCost(int jobID, System.DateTime jobStartDate, List<JobAssignment> jobAssigments)
        {
            return _JobManagement.CalculateEstimatedLaborCost(jobID, jobStartDate, jobAssigments);
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
            return _JobManagement.CalculateJobEndDate(jobStartDate, estimatedWorkgHours, avgWorkingHours);
        }
        /// <summary>
        /// Mark job status as complete
        /// </summary>
        /// <param name="jobID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleJobStatus(int jobID, bool tobeEnabled)
        {
            return _JobManagement.ToggleJobStatus(jobID, tobeEnabled);
        }
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="poID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobPOStatus(int poID, bool active)
        {
            return _JobManagement.ToggleJobPOStatus(poID, active);
        }
        /// <summary>
        /// Get List of Job PO
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        public JobDTO GetJobPO(int poID)
        {
            return _JobManagement.GetJobPO(poID);
        }
        /// <summary>
        /// Get List of Job POs
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobPO> GetJobPOs(int jobID)
        {
            return _JobManagement.GetJobPOs(jobID);
        }
        /// <summary>
        /// Create Job PO
        /// </summary>
        /// <param name="jobPO"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobPO(JobPO jobPO, List<POEstimateItem> poEstimateItems)
        {
            return _JobManagement.CreateJobPO(jobPO, poEstimateItems);
        }
        /// <summary>
        /// Edit Job PO
        /// </summary>
        /// <param name="jobPO"></param>
        /// <param name="poEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobPO(JobPO jobPO, List<POEstimateItem> poEstimateItems)
        {
            return _JobManagement.EditJobPO(jobPO, poEstimateItems);
        }
        /// <summary>
        /// Create PO Document
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        public byte[] CreatePODocument(int poID)
        {
            return _JobManagement.CreatePODocument(poID);
        }
        /// <summary>
        /// Get List of Job CO
        /// </summary>
        /// <param name="coID"></param>
        /// <returns></returns>
        public JobDTO GetJobCO(int coID)
        {
            return _JobManagement.GetJobCO(coID);
        }
        /// <summary>
        /// Get List of Job COs
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobCO> GetJobCOs(int jobID)
        {
            return _JobManagement.GetJobCOs(jobID);
        }
        /// <summary>
        /// Create Job CO
        /// </summary>
        /// <param name="jobCO"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobCO(JobCO jobCO, List<COEstimateItem> coEstimateItems)
        {
            return _JobManagement.CreateJobCO(jobCO, coEstimateItems);
        }
        /// <summary>
        /// Edit Job CO
        /// </summary>
        /// <param name="jobCO"></param>
        /// <param name="coEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobCO(JobCO jobCO, List<COEstimateItem> coEstimateItems)
        {
            return _JobManagement.EditJobCO(jobCO, coEstimateItems);
        }
        /// <summary>
        /// Toggle job CO status
        /// </summary>
        /// <param name="coID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobCOStatus(int coID, bool active)
        {
            return _JobManagement.ToggleJobCOStatus(coID, active);
        }
        /// <summary>
        /// Create CO Document
        /// </summary>
        /// <param name="coID"></param>
        /// <returns></returns>
        public byte[] CreateCODocument(int coID)
        {
            return _JobManagement.CreateCODocument(coID);
        }
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <returns></returns>
        public List<JobDocumentType> GetJobDocumentTypes()
        {
            return _JobManagement.GetJobDocumentTypes();
        }
        /// <summary>
        /// Get List of Job Documents
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobDocument> GetJobDocuments(int jobID)
        {
            return _JobManagement.GetJobDocuments(jobID);
        }
        /// <summary>
        /// Get a job document
        /// </summary>
        /// <param name="jobDocumentID"></param>
        /// <returns></returns>
        public JobDocument GetJobDocument(int jobDocumentID)
        {
            return _JobManagement.GetJobDocument(jobDocumentID);
        }
        /// <summary>
        /// Save Job document
        /// </summary>
        /// <param name="jobDocument"></param>
        /// <returns></returns>
        public int SaveDocument(JobDocument jobDocument)
        {
            return _JobManagement.SaveDocument(jobDocument);
        }
        /// <summary>
        /// Delete job document by jobDocumentID
        /// </summary>
        /// <param name="jobDocumentID"></param>
        /// <returns></returns>
        public bool DeleteJobDocument(int jobDocumentID)
        {
            return _JobManagement.DeleteJobDocument(jobDocumentID);
        }
        /// <summary>
        /// Get List of Job WO
        /// </summary>
        /// <param name="woID"></param>
        /// <returns></returns>
        public JobDTO GetJobWO(int woID)
        {
            return _JobManagement.GetJobWO(woID);
        }
        /// <summary>
        /// Toggle job PO status
        /// </summary>
        /// <param name="woID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobWOStatus(int woID, bool active)
        {
            return _JobManagement.ToggleJobWOStatus(woID, active);
        }
        /// <summary>
        /// Get JobAssignment List
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobAssignment> GetJobAssignments(int jobID)
        {
            return _JobManagement.GetJobAssignments(jobID);
        }
        /// <summary>
        /// Get JobAssignment Crew Leader List
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<vw_JobAssignmentCrewLeaders> GetJobAssignmentCrewLeaders(int jobID)
        {
            return _JobManagement.GetJobAssignmentCrewLeaders(jobID);
        }

        /// <summary>
        /// Get List of Job WOs
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobWO> GetJobWOs(int jobID)
        {
            return _JobManagement.GetJobWOs(jobID);
        }
        /// <summary>
        /// Create Job WO
        /// </summary>
        /// <param name="jobWO"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        public int CreateJobWO(JobWO jobWO, List<WOEstimateItem> woEstimateItems)
        {
            return _JobManagement.CreateJobWO(jobWO, woEstimateItems);
        }
        /// <summary>
        /// Edit Job WO
        /// </summary>
        /// <param name="jobWO"></param>
        /// <param name="woEstimateItems"></param>
        /// <returns></returns>
        public bool EditJobWO(JobWO jobWO, List<WOEstimateItem> woEstimateItems)
        {
            return _JobManagement.EditJobWO(jobWO, woEstimateItems);
        }
        /// <summary>
        /// Create WO Document
        /// </summary>
        /// <param name="woID"></param>
        /// <returns></returns>
        public JobDocument CreateWODocument(int woID)
        {
            return _JobManagement.CreateWODocument(woID);
        }
        /// <summary>
        /// Get List of Job Invoices
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobInvoice> GetJobInvoices(int jobID)
        {
            return _JobManagement.GetJobInvoices(jobID);
        }
        /// <summary>
        /// Create Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        public int CreateJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems)
        {
            return _JobManagement.CreateJobInvoice(jobInvoice, invoiceItems);
        }

        /// <summary>
        /// Edit Job Invoice
        /// </summary>
        /// <param name="jobInvoice"></param>
        /// <param name="invoiceItems"></param>
        /// <returns></returns>
        public bool EditJobInvoice(JobInvoice jobInvoice, List<InvoiceItems> invoiceItems)
        {
            return _JobManagement.EditJobInvoice(jobInvoice, invoiceItems);
        }
        /// <summary>
        /// Get Job Invoice Detail
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        public JobDTO GetJobInvoice(int invoiceID)
        {
            return _JobManagement.GetJobInvoice(invoiceID);
        }
        /// <summary>
        /// Toggle Job Invoice status
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobInvoiceStatus(int invoiceID, bool active)
        {
            return _JobManagement.ToggleJobInvoiceStatus(invoiceID, active);
        }
        /// <summary>
        /// Create Invoice Document
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        public JobDocument CreateInvoiceDocument(int invoiceID)
        {
            return _JobManagement.CreateInvoiceDocument(invoiceID);
        }

        /// <summary>
        /// Get List of Job Inspection
        /// </summary>
        /// <param name="inspID"></param>
        /// <returns></returns>
        public JobDTO GetJobInspection(int inspID)
        {
            return _JobManagement.GetJobInspection(inspID);
        }
        /// <summary>
        /// Toggle job Inspection status
        /// </summary>
        /// <param name="inspID"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool ToggleJobInspectionStatus(int inspID, bool active)
        {
            return _JobManagement.ToggleJobInspectionStatus(inspID, active);
        }
        /// <summary>
        /// Get List of Job Inspections
        /// </summary>
        /// <param name="jobID"></param>
        /// <returns></returns>
        public List<JobInspection> GetJobInspections(int jobID)
        {
            return _JobManagement.GetJobInspections(jobID);
        }
        /// <summary>
        /// Create Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        public int CreateJobInspection(JobInspection jobInspection)
        {
            return _JobManagement.CreateJobInspection(jobInspection);
        }
        /// <summary>
        /// Edit Job Inspection
        /// </summary>
        /// <param name="jobInspection"></param>
        /// <returns></returns>
        public bool EditJobInspection(JobInspection jobInspection)
        {
            return _JobManagement.EditJobInspection(jobInspection);
        }
        /// <summary>
        /// Get Permit List
        /// </summary>
        /// <returns></returns>
        public List<Permit> GetPrmitList()
        {
            return _JobManagement.GetPrmitList();
        }
        /// <summary>
        /// Get Permit Inspection List
        /// </summary>
        /// <returns></returns>
        public List<PermitInspection> GetPrmitInspectionList()
        {
            return _JobManagement.GetPrmitInspectionList();
        }
        /// <summary>
        /// Get Permit Status List
        /// </summary>
        /// <returns></returns>
        public List<PermitStatus> GetPrmitStatusList()
        {
            return _JobManagement.GetPrmitStatusList();
        }
        public string GetJobAddress(int jobID)
        {
            return _JobManagement.GetJobAddress(jobID);
        }
    }
}
