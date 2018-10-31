namespace KRF.Core.Entities.ValueList
{
    public class EquipmentStatus : ValueList
    {
        public int EquipmentStatusID { get; set; }

        public string StatusName { get; set; }

        public bool Active { get; set; }
    }
}
