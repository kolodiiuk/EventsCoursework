using System;

namespace Events.Models.Specifications;

public class DurationSpecification(TimeSpan duration) : ISpecification
{
    public bool IsSatisfiedBy(Event @event)
    {
        return @event.Duration != null &&  @event.Duration == duration;
    }
}