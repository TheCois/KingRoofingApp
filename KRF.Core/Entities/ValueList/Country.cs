﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Core.Entities.ValueList
{
    public class Country : ValueList
    {
        /// <summary>
        /// Holds the Country ID information.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Holds the Country Name information.
        /// </summary>
        public string Description { get; set; }
    }
}
