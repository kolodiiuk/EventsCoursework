using System;

namespace Events.Models.Specifications;

public class TimeSpecification(TimeSpan time) : ISpecification
{
    private readonly TimeSpan? _time = time;

    public bool IsSatisfiedBy(Event @event)
    {
        return _time != null
               && (@event.DateTime.HasValue
                   && @event.DateTime.Value.TimeOfDay == _time.Value);
    }
}