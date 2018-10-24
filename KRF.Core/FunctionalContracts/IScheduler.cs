using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRF.Core.Entities.MISC;

namespace KRF.Core.FunctionalContracts
{
    public interface IScheduler
    {
        bool ScheduleMeeting(Contact contactPerson1, IList<Contact> ContactPersons, DateTime from, DateTime to);
    }
}
