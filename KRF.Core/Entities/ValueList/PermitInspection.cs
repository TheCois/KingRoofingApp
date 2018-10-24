using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class PermitInspection : ValueList
    {
        public int ID { get; set; }
        public string Inspections { get; set; }
        public bool Active { get; set; }
    }
}
