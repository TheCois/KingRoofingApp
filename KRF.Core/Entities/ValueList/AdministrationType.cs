namespace KRF.Core.Entities.ValueList
{
    public class AdministrationType : ValueList
    {
        public int ID { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
