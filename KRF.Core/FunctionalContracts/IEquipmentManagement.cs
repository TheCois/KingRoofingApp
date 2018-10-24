using System.Collections.Generic;
using KRF.Core.Entities.AccessControl;
using KRF.Core.Entities.Master;
using KRF.Core.DTO.Master;

namespace KRF.Core.FunctionalContracts
{
    public interface IEquipmentManagement
    {
        /// <summary>
        /// Create Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        int CreateEquipment(Equipment equipment);
        /// <summary>
        /// Edit Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        bool EditEquipment(Equipment equipment);
        /// <summary>
        /// Active/Inactive Equipment status
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        bool ToggleEquipmentStatus(int equipmentID, bool tobeEnabled);
        /// <summary>
        /// Get Equipment by EquipmentID
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <returns></returns>
        EquipmentDTO GetEquipment(int equipmentID);
        /// <summary>
        /// Get all Equipments
        /// </summary>
        /// <returns></returns>
        EquipmentDTO GetEquipments();

    }
}
