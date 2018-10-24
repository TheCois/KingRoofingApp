using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.Actors;

namespace KRF.Core.Entities.Sales
{
    public class Proposal
    {
        /// <summary>
        /// Holds Proposal unique identifier
        /// </summary>
        public int  Id { get; set; }

        /// <summary>
        /// Proposal name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Details of the job.
        /// </summary>
        public Job JobDetail { get; set; }

        /// <summary>
        /// Proposal status (Prepared / Uploaded)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Proposal price submitted to the customer.
        /// </summary>
        public decimal ProposalPrice { get; set; }

        /// <summary>
        /// Is Proposal signed (holds True / False)
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// If signed, holds date and time.
        /// </summary>
        public DateTime ApprovedDateAndTime { get; set; }

        /// <summary>
        /// Proposal approved user details.
        /// </summary>
        public Lead ApprovedBy { get; set; }

    }
}
