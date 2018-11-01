using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;

namespace KRF.Core.Repository
{
    public interface IEquipmentManagementRepository
    {
        /// <summary>
        /// Create Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        int Create(Equipment equipment);
        /// <summary>
        /// Edit Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        bool Edit(Equipment equipment);
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
