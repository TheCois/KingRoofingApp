﻿namespace KRF.Core.Entities.ValueList
{
    public class tbl_SkillLevel : ValueList
    {
        /// <summary>
        /// Holds the territory ID information.
        /// </summary>
        public int SkillLevelID { get; set; }

        /// <summary>
        /// Holds the territory Name information.
        /// </summary>
        public string SkillLevelDesc { get; set; }
        public bool Active { get; set; }
    }
}
