using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KRF.Core.Entities.Product
{
    public class Inventory
    {
        /// <summary>
        /// Inventory unique identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Assembly ID
        /// </summary>
        public int? AssemblyID { get; set; }

        /// <summary>
        /// Assembly ID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// Assembly Qty.
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// DateUpdated
        /// </summary>
        public DateTime DateUpdated { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
    }
    public class InventoryAudit
    {
        public int InventoryAuditID { get; set; }
        /// <summary>
        /// Inventory unique identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Assembly ID
        /// </summary>
        public int? AssemblyID { get; set; }
        public int ItemID { get; set; }

        /// <summary>
        /// Assembly Qty.
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// DateUpdated
        /// </summary>
        public DateTime DateUpdated { get; set; }
        /// <summary>
        /// DateCreated
        /// </summary>
        public DateTime DateCreated { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
    }
}
