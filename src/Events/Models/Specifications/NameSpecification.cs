namespace Events.Models.Specifications;

public class NameSpecification(string name) : ISpecification
{
    public bool IsSatisfiedBy(Event @event)
    {
        return @event.Name.Contains(name);
    }
}