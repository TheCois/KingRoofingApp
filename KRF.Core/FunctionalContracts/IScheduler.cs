using System;
using System.Collections.Generic;
using KRF.Core.Entities.MISC;

namespace KRF.Core.FunctionalContracts
{
    public interface IScheduler
    {
        bool ScheduleMeeting(Contact contactPerson1, IList<Contact> ContactPersons, DateTime from, DateTime to);
    }
}
