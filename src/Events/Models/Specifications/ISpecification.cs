namespace Events.Models.Specifications;

public interface ISpecification
{
    bool IsSatisfiedBy(Event @event);
}