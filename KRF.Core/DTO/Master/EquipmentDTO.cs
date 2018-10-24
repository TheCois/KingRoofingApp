using KRF.Core.Entities.Master;
using KRF.Core.Entities.ValueList;
using System.Collections.Generic;
namespace KRF.Core.DTO.Master
{
    /// <summary>
    /// This class does not have database table. This class acts as a container for below products classes
    /// </summary>
    public class EquipmentDTO
    {
        public IList<Equipment> Equipments { get; set; }

        public IList<EquipmentStatus> EquipmentStatus { get; set; }
    }
}
