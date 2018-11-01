using KRF.Core.Entities.MISC;
using KRF.Core.Entities.ValueList;
using System.Collections.Generic;

namespace KRF.Core.FunctionalContracts
{
    public interface IAdministrationManagement
    {
        /// <summary>
        /// Create Master Record by Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="extraField1"></param>
        /// <returns></returns>
        int Create(int type, string description, string extraField1 = "");

        /// <summary>
        /// Edit Master Record by Type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="extraField1"></param>
        /// <returns></returns>
        bool Edit(int type, int id, string description, bool active, string extraField1 = "");

        /// <summary>
        /// Set Active/Inactive master record by type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        bool SetActiveInactive(int type, int id, bool active);
    
        /// <summary>
        /// Get Master Records by Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        List<MasterRecords> GetMasterRecordsByType(int type);
        /// <summary>
        /// Get Master Records by Type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
       MasterRecords GetMasterRecordsByTypeAndID(int type, int id);
       /// <summary>
       /// Get administration types
       /// </summary>
       /// <returns></returns>
       List<AdministrationType> GetAdministrationType();

    }
}
