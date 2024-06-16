using System;

namespace Events.Models.Specifications;

public class DateSpecification(DateTime fromDate, DateTime toDate) : ISpecification
{
    public bool IsSatisfiedBy(Event @event)
    {
        return @event.DateTime.HasValue 
                && @event.DateTime.Value.Date >= fromDate.Date
                && @event.DateTime.Value.Date <= toDate.Date; 
    }
}