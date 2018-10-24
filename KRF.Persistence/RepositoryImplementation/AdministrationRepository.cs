using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.Entities.MISC;
using KRF.Core.Entities.ValueList;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.RepositoryImplementation
{
    public class AdministrationRepository : IAdministrationRepository
    {
        private readonly IAdministrationManagement _AdministrationManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public AdministrationRepository()
        {
            _AdministrationManagement = ObjectFactory.GetInstance<IAdministrationManagement>();
        }
        /// <summary>
        /// Create Master Record by Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="extraField1"></param>
        /// <returns></returns>
        public int Create(int type, string description, string extraField1 = "")
        {
            return _AdministrationManagement.Create(type, description, extraField1);
        }

        /// <summary>
        /// Edit Master Record by Type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="extraField1"></param>
        /// <returns></returns>
        public bool Edit(int type, int id, string description, bool active, string extraField1 = "")
        {
            return _AdministrationManagement.Edit(type, id, description, active, extraField1);
        }

        /// <summary>
        /// Set Active/Inactive master record by type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public bool SetActiveInactive(int type, int id, bool active)
        {
            return _AdministrationManagement.SetActiveInactive(type, id, active);
        }

        /// <summary>
        /// Get Master Records by Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<MasterRecords> GetMasterRecordsByType(int type)
        {
            return _AdministrationManagement.GetMasterRecordsByType(type);
        }
        /// <summary>
        /// Get Master Records by Type and ID
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public MasterRecords GetMasterRecordsByTypeAndID(int type, int id)
        {
            return _AdministrationManagement.GetMasterRecordsByTypeAndID(type, id);
        }
        /// <summary>
        /// Get administration types
        /// </summary>
        /// <returns></returns>
        public List<AdministrationType> GetAdministrationType()
        {
            return _AdministrationManagement.GetAdministrationType();
        }
    }
}
