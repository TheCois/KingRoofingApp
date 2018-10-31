
namespace KRF.Core.Entities.Master
{
    public class CrewDetail
    {
        public int CrewDetailID { get; set; }
        public int CrewID { get; set; }
        public int EmpId { get; set; }
        public bool IsLead { get; set; }
        public bool Active { get; set; }
    }
}
