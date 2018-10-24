using KRF.Core.Entities.Sales;
using KRF.Core.Entities.ValueList;
using System.Collections.Generic;
namespace KRF.Core.DTO.Job
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below job classes
    /// </summary>
    public class JobDTO
    {
        /// <summary>
        /// Holds the Job List
        /// </summary>
        public IList<KRF.Core.Entities.Customer.Job> Jobs { get; set; }
        
        /// <summary>
        /// Holds Lead List
        /// </summary>
        public IList<Lead> Leads { get; set; }
        /// <summary>
        /// Holds Customer Address List
        /// </summary>
        public IList<KRF.Core.Entities.Sales.CustomerAddress> CustomerAddress { get; set; }
        /// <summary>
        /// Holds Customer City List
        /// </summary>
        public IList<KRF.Core.Entities.ValueList.City> Cities { get; set; }
        /// <summary>
        /// Holds Customer State List
        /// </summary>
        public IList<KRF.Core.Entities.ValueList.State> States { get; set; }
        /// <summary>
        /// Holds Job Assignment List
        /// </summary>
        public IList<KRF.Core.Entities.Customer.JobAssignment> JobAssignments { get; set; }
        /// <summary>
        /// Holds Job Task List
        /// </summary>
        public IList<KRF.Core.Entities.Customer.JobTask> JobTasks { get; set; }
        /// <summary>
        /// Holds Job PO List
        /// </summary>
        public IList<KRF.Core.Entities.Customer.JobPO> JobPOs { get; set; }
        public KRF.Core.Entities.Customer.JobPO JobPO { get; set; }
        public List<KRF.Core.Entities.Customer.POEstimateItem> POEstimateItems { get; set; }
        public IList<KRF.Core.Entities.Customer.JobCO> JobCOs { get; set; }
        public KRF.Core.Entities.Customer.JobCO JobCO { get; set; }
        public List<KRF.Core.Entities.Customer.COEstimateItem> COEstimateItems { get; set; }
        public IList<KRF.Core.Entities.Product.Item> Items { get; set; }
        public IList<KRF.Core.Entities.Product.Assembly> Assemblies { get; set; }
        public IList<UnitOfMeasure> UnitOfMeasures { get; set; }
        /// <summary>
        /// Holds Job WO List
        /// </summary>
        public IList<KRF.Core.Entities.Customer.JobWO> JobWOs { get; set; }
        public KRF.Core.Entities.Customer.JobWO JobWO { get; set; }
        public List<KRF.Core.Entities.Customer.WOEstimateItem> WOEstimateItems { get; set; }
        public KRF.Core.Entities.Customer.JobInvoice JobInvoice { get; set; }
        public List<KRF.Core.Entities.Customer.InvoiceItems> InvoiceItems { get; set; }

        public IList<KRF.Core.Entities.Customer.JobInspection> JobInspections { get; set; }
        public KRF.Core.Entities.Customer.JobInspection JobInspection { get; set; }
        public IList<Status> JobStatus { get; set; }
    }
}
