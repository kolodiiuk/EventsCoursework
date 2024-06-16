using System.Collections.Generic;
using System.Linq;

namespace Events.Models.Specifications;

public class CompositeSpecification(params ISpecification[] specification) : ISpecification
{
    private readonly List<ISpecification> _specifications = specification.ToList();

    public bool IsSatisfiedBy(Event @event)
    {
        return _specifications.All(
            specification => specification.IsSatisfiedBy(@event));
    } 
}