namespace Events.Models.Specifications;

public class LocationSpecification(string location) : ISpecification
{
    public bool IsSatisfiedBy(Event @event)
    {
        return @event.Location != null && @event.Location.Contains(location);
    }
}