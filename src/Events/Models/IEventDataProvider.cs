using System;
using System.Collections.Generic;
using Events.Utilities;

namespace Events.Models;

public interface IEventDataProvider
{
    Result AddEvent(Event @event);
    Result UpdateEvent(Event @event);
    Result DeleteEvent(Guid id);
    Result<List<Event>> GetAllEvents();
    Result<Event> GetEventById(Guid id);
    Result SubmitChanges();
}

