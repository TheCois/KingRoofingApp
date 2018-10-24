using System;
using System.Collections.Generic;
using KRF.Core.Entities.MISC;

namespace KRF.Core.FunctionalContracts
{
    public interface IAuditLog
    {
        /// <summary>
        /// Get all logs recorded in the system.
        /// </summary>
        /// <returns>Audit log list</returns>
        List<AuditLog> GetAllLogs();

        /// <summary>
        /// Get all logs filtered by date range.
        /// </summary>
        /// <param name="start">Start date.</param>
        /// <param name="end"></param>
        /// <returns>Audit log list</returns>
        List<AuditLog> GetAllLogsFilteredByDateRange(DateTime start, DateTime end);

        /// <summary>
        /// Create new log based on the log object
        /// </summary>
        /// <param name="auditLog">audit log details.</param>
        /// <returns>True - if success, False - If failure</returns>
        bool CreateLog(AuditLog auditLog);
    }
}
