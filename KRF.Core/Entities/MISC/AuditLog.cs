using System;
using KRF.Core.Entities.Actors;

namespace KRF.Core.Entities.MISC
{
    public class AuditLog
    {
        /// <summary>
        /// Audit log id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date and Time at which the log or action took place in the system.
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// User who changed the corresponding record or data.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Module name where the change is made
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Logs the field level details that are changed.
        /// </summary>
        public string ModifiedRecord { get; set; }

        /// <summary>
        /// Logs the Old value details.
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// Logs the New value details. 
        /// </summary>
        public string NewValue { get; set; }
    }
}
