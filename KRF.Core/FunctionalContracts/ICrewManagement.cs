﻿using System.Collections.Generic;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.Master;
using KRF.Core.DTO.Master;

namespace KRF.Core.FunctionalContracts
{
    public interface ICrewManagement
    {
        /// <summary>
        /// Create Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        int CreateCrew(CrewDTO crew);
        /// <summary>
        /// Edit Crew
        /// </summary>
        /// <param name="crew"></param>
        /// <returns></returns>
        bool EditCrew(CrewDTO crew);
        /// <summary>
        /// Active/Inactive crew status
        /// </summary>
        /// <param name="crewID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        bool SetActiveInactiveCrew(int crewID, bool tobeEnabled);
        /// <summary>
        /// Active/Inactive crew detail status
        /// </summary>
        /// <param name="crewDetailID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        bool SetActiveInactiveCrewDetail(int crewDetailID, bool tobeEnabled);
        /// <summary>
        /// Get Crew by CrewID
        /// </summary>
        /// <param name="crewID"></param>
        /// <returns></returns>
        CrewDTO GetCrew(int crewID);
        /// <summary>
        /// Get all Crews
        /// </summary>
        /// <returns></returns>
        CrewDTO ListAllCrews();

    }
}