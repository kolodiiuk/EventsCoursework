namespace Events.Models.Specifications;

public class DoneSpecification(bool done) : ISpecification
{
    public bool IsSatisfiedBy(Event @event)
    {
        return @event.Done != null && @event.Done == done;
    }
}