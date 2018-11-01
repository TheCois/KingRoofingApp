namespace KRF.Core.Entities.ValueList
{
    public class FleetStatus : ValueList
    {
        public int FleetStatusID { get; set; }

        public string StatusName { get; set; }

        public bool Active { get; set; }
    }
}
