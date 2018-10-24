using KRF.Core.DTO.Master;
using KRF.Core.Entities.Master;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using StructureMap;
using System.Collections.Generic;

namespace KRF.Persistence.FunctionalContractImplementation
{
    public class EquipmentManagementRepository : IEquipmentManagementRepository
    {
        private readonly IEquipmentManagement _EquipmentManagement;

        /// <summary>
        /// Constructor
        /// </summary>
        public EquipmentManagementRepository()
        {
            _EquipmentManagement = ObjectFactory.GetInstance<IEquipmentManagement>();
        }
        /// <summary>
        /// Create Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public int Create(Equipment equipment)
        {
            return _EquipmentManagement.CreateEquipment(equipment);
        }
        /// <summary>
        /// Edit Equipment
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public bool Edit(Equipment equipment)
        {
            return _EquipmentManagement.EditEquipment(equipment);
        }
        /// <summary>
        /// Active/Inactive Equipment status
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <param name="tobeEnabled"></param>
        /// <returns></returns>
        public bool ToggleEquipmentStatus(int equipmentID, bool tobeEnabled)
        {
            return _EquipmentManagement.ToggleEquipmentStatus(equipmentID, tobeEnabled);
        }
        /// <summary>
        /// Get Equipment by EquipmentID
        /// </summary>
        /// <param name="equipmentID"></param>
        /// <returns></returns>
        public EquipmentDTO GetEquipment(int equipmentID)
        {
            return _EquipmentManagement.GetEquipment(equipmentID);
        }
        /// <summary>
        /// Get all Equipments
        /// </summary>
        /// <returns></returns>
        public EquipmentDTO GetEquipments()
        {
            return _EquipmentManagement.GetEquipments();
        }

    }
}
