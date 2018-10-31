using KRF.Core.DTO.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;

namespace KRF.Persistence.RepositoryImplementation
{
    public class CrewManagementRepository : ICrewManagementRepository
    {
        private readonly ICrewManagement crewManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public CrewManagementRepository()
        {
            crewManagement_ = ObjectFactory.GetInstance<ICrewManagement>();
        }
        /// <summary>
        /// Create Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        public int Create(CrewDTO crew)
        {
            return crewManagement_.CreateCrew(crew);
        }
        /// <summary>
        /// Edit Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        public bool Edit(CrewDTO crew)
        {
            return crewManagement_.EditCrew(crew);
        }
        /// <summary>
        /// Active/Inactive Crew status
        /// </summary>
        /// <param name="crewId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool SetActiveInactiveCrew(int crewId, bool tobeEnabled)
        {
            return crewManagement_.SetActiveInactiveCrew(crewId, tobeEnabled);
        }
        /// <summary>
        /// Active/Inactive Crew detail status
        /// </summary>
        /// <param name="crewDetailId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool SetActiveInactiveCrewDetail(int crewDetailId, bool tobeEnabled)
        {
            return crewManagement_.SetActiveInactiveCrewDetail(crewDetailId, tobeEnabled);
        }
        /// <summary>
        /// Get Crew by CrewID
        /// </summary>
        /// <param name="crewId"></param>
        /// <returns></returns>
        public CrewDTO GetCrew(int crewId)
        {
            return crewManagement_.GetCrew(crewId);
        }
        /// <summary>
        /// Get all Crews
        /// </summary>
        /// <returns></returns>
        public CrewDTO GetCrews()
        {
            return crewManagement_.ListAllCrews();
        }

    }
}
