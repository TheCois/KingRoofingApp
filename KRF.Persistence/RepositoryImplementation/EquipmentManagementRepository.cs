using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;

namespace KRF.Persistence.RepositoryImplementation
{
    public class EquipmentManagementRepository : IEquipmentManagementRepository
    {
        private readonly IEquipmentManagement equipmentManagement_;

        /// <summary>
        /// Constructor
        /// </summary>
        public EquipmentManagementRepository()
        {
            equipmentManagement_ = ObjectFactory.GetInstance<IEquipmentManagement>();
        }
        /// <summary>
        /// Create Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public int Create(Equipment equipment)
        {
            return equipmentManagement_.CreateEquipment(equipment);
        }
        /// <summary>
        /// Edit Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public bool Edit(Equipment equipment)
        {
            return equipmentManagement_.EditEquipment(equipment);
        }
        /// <summary>
        /// Active/Inactive Equipment status
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleEquipmentStatus(int equipmentId, bool tobeEnabled)
        {
            return equipmentManagement_.ToggleEquipmentStatus(equipmentId, tobeEnabled);
        }
        /// <summary>
        /// Get Equipment by EquipmentID
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <returns></returns>
        public EquipmentDTO GetEquipment(int equipmentId)
        {
            return equipmentManagement_.GetEquipment(equipmentId);
        }
        /// <summary>
        /// Get all Equipments
        /// </summary>
        /// <returns></returns>
        public EquipmentDTO GetEquipments()
        {
            return equipmentManagement_.GetEquipments();
        }

    }
}
