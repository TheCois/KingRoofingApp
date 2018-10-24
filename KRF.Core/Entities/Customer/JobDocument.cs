using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.Customer
{
    public class JobDocument
    {
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// JOB ID from Estimate class
        /// </summary>
        public int JobID { get; set; }

        /// <summary>
        /// Documet Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Document type (e.g pdf, jpg etc)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Description of the document
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// File content in byte array
        /// </summary>
        public byte[] Text { get; set; }

        /// <summary>
        /// Upload DateTime of the document
        /// </summary>
        public System.Nullable<DateTime> UploadDateTime { get; set; }
    }
    public class JobDocumentType
    {
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Document Type
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// DocumentTypeDesc
        /// </summary>
        public string DocumentTypeDesc { get; set; }
    }
}
