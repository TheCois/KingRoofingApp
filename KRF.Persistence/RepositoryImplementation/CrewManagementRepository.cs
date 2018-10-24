using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class CrewManagementRepository : ICrewManagementRepository
    {
        private readonly ICrewManagement _CrewManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public CrewManagementRepository()
        {
            _CrewManagement = ObjectFactory.GetInstance<ICrewManagement>();
        }
        /// <summary>
        /// Create Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        public int Create(CrewDTO crew)
        {
            return _CrewManagement.CreateCrew(crew);
        }
        /// <summary>
        /// Edit Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        public bool Edit(CrewDTO crew)
        {
            return _CrewManagement.EditCrew(crew);
        }
        /// <summary>
        /// Active/Inactive Crew status
        /// </summary>
        /// <param name="crewID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool SetActiveInactiveCrew(int crewID, bool tobeEnabled)
        {
            return _CrewManagement.SetActiveInactiveCrew(crewID, tobeEnabled);
        }
        /// <summary>
        /// Active/Inactive Crew detail status
        /// </summary>
        /// <param name="crewDetailID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool SetActiveInactiveCrewDetail(int crewDetailID, bool tobeEnabled)
        {
            return _CrewManagement.SetActiveInactiveCrewDetail(crewDetailID, tobeEnabled);
        }
        /// <summary>
        /// Get Crew by CrewID
        /// </summary>
        /// <param name="crewID"></param>
        /// <returns></returns>
        public CrewDTO GetCrew(int crewID)
        {
            return _CrewManagement.GetCrew(crewID);
        }
        /// <summary>
        /// Get all Crews
        /// </summary>
        /// <returns></returns>
        public CrewDTO GetCrews()
        {
            return _CrewManagement.ListAllCrews();
        }

    }
}
