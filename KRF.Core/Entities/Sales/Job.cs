using KRF.Core.Entities.MISC;

namespace KRF.Core.Entities.Sales
{
    public class Job
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Contact Contact { get; set; }
    }
}
