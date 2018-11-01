using System.Collections.Generic;
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
