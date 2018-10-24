using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRF.Common
{
    public interface IDatabaseConnection
    {
        string ConnectionString { get; }
    }
}
