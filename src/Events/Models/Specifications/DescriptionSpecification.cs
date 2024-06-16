namespace Events.Models.Specifications;

public class DescriptionSpecification(string description) : ISpecification
{  
    public bool IsSatisfiedBy(Event @event)
    {
        return @event.Description != null && @event.Description.Contains(description);
    }
}