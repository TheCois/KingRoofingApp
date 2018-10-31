using System.Collections.Generic;
using KRF.Core.Entities.Customer;

namespace KRF.Web.Models
{
    public class JobData
    {
        /// <summary>
        /// Holds Job Information
        /// </summary>
        public Job Job { get; set; }
        /// <summary>
        /// Holds Job Assignment records
        /// </summary>
        public List<JobAssignment> JobAssignments { get; set; }
        /// <summary>
        /// Holds Job Task Records
        /// </summary>
        public JobTask JobTask { get; set; }
        /// <summary>
        /// Holds Job PO Record
        /// </summary>
        public JobPO JobPO { get; set; }
        /// <summary>
        /// Holds Job PO Estimate Items
        /// </summary>
        public List<POEstimateItem> POEstimateItems { get; set; }
        /// <summary>
        /// Holds Job CO Record
        /// </summary>
        public JobCO JobCO { get; set; }
        /// <summary>
        /// Holds Job CO Estimate Items
        /// </summary>
        public List<COEstimateItem> COEstimateItems { get; set; }
        /// <summary>
        /// Holds Job WO Record
        /// </summary>
        public JobWO JobWO { get; set; }
        /// <summary>
        /// Holds Job WO Estimate Items
        /// </summary>
        public List<WOEstimateItem> WOEstimateItems { get; set; }
        /// <summary>
        /// Holds Job Invoice Record
        /// </summary>
        public JobInvoice JobInvoice { get; set; }
        /// <summary>
        /// Holds Job Invoice Items
        /// </summary>
        public List<InvoiceItems> InvoiceItems { get; set; }
        /// <summary>
        /// Holds Job Inspection Record
        /// </summary>
        public JobInspection JobInspection { get; set; }
    }

    public class CrewEmpDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public class ItemAssemblies
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
    }
    public class JobAssignmentDetails
    {
        public int JobAssignmentID { get; set; }
        public int JobID { get; set; }
        /// <summary>
        /// Hold EmployeeID/CrewID based on Type field
        /// </summary>
        public int ObjectPKID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class POCOEstimateItems
    {
        public int ID { get; set; }
        public int EstimateID { get; set; }
        public int ItemAssemblyID { get; set; }
        public int ItemAssemblyType { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal MaterialCost { get; set; }
        public string ItemNames { get; set; }
        public int COID { get; set; }
        public int ItemID { get; set; }
    }
    public class WOCOEstimateItems
    {
        public int ID { get; set; }
        public int EstimateID { get; set; }
        public int ItemAssemblyID { get; set; }
        public int ItemAssemblyType { get; set; }
        public decimal Budget { get; set; }
        public decimal Used { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal LaborCost { get; set; }
        public decimal MaterialCost { get; set; }
        public string ItemNames { get; set; }
        public int COID { get; set; }
        public int ItemID { get; set; }
    }
}