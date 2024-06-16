namespace Events.Models.Specifications;

public class CategorySpecification(string category) : ISpecification
{
    public bool IsSatisfiedBy(Event @event)
    {
        return @event.Category != null && @event.Category.Contains(category);
    }
}