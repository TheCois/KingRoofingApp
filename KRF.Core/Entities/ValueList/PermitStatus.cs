using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class PermitStatus : ValueList
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
