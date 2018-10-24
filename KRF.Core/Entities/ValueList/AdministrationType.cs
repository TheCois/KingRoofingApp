using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
