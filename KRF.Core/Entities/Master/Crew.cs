using System;
using System.Collections.Generic;

namespace KRF.Core.Entities.Master
{
    public class CrewData 
    {
        public Crew crew { get; set; }
        public List<CrewDetail> crewDetails {get;set;}
    }
    public class Crew
    {
        public int CrewID { get; set; }
        public string CrewName { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public System.Nullable<DateTime> DateUpdated { get; set; }
    }
}
