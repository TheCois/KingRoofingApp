using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.Customer
{
    public class JobAssignment
    {
        public int JobAssignmentID { get; set; }
        public int JobID { get; set; }
        /// <summary>
        /// Hold EmployeeID/CrewID based on Type field
        /// </summary>
        public int ObjectPKID { get; set; }
        public string Type { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
    }
}
