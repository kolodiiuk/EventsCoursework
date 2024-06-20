using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Events.Utilities;

namespace Events.Models;

public interface IEventDataProvider
{
    Result AddEvent(Event @event);
    Result<IEnumerable<Event>> GetEventListByCondition(Func<Event, bool> condition);
    Result UpdateEvent(Event @event);
    Result DeleteEvent(Guid id);
    Result<List<Event>> GetAllEvents();
}