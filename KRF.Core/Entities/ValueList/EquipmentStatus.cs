using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class EquipmentStatus : ValueList
    {
        public int EquipmentStatusID { get; set; }

        public string StatusName { get; set; }

        public bool Active { get; set; }
    }
}
