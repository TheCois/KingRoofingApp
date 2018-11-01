namespace KRF.Core.Entities.ValueList
{
    public class tbl_EmpTerritory : ValueList
    {
        /// <summary>
        /// Holds the territory ID information.
        /// </summary>
        public int TerritoryID { get; set; }

        /// <summary>
        /// Holds the territory Name information.
        /// </summary>
        public string TerritoryDesc { get; set; }

        public int EmpID { get; set; }
        
        /// <summary>
        /// Hold the true/false value
        /// </summary>
        public bool Active { get; set; }
    }
}
