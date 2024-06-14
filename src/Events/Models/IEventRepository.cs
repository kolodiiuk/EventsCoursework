using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Events.Models;

public interface IEventRepository
{
    Task<Result> AddEventAsync(Event @event);
    Task<Result<Event>> GetSingleEventByConditionAsync(Func<Event, bool> condition);
    Task<Result<IEnumerable<Event>>> GetEventListByConditionAsync(Func<Event, bool> condition);
    Task<Result> UpdateEventAsync(Event @event, Func<Event, bool> condition);
    Task<Result> DeleteEventAsync(Func<Event, bool> condition);
}