using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.ValueList;
using KRF.Core.Entities.Product;

namespace KRF.Core.DTO.Product
{
    public class AssemblyItemDTO
    {
        /// <summary>
        /// Assembly 
        /// </summary>
        public Assembly assembly { get; set; }

        /// <summary>
        /// Item composition within the assembly.
        /// </summary>
        public IList<AssemblyItem> assemblyItem { get; set; }

    }
}
