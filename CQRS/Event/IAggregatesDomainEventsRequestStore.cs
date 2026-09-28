using Fundation.Abstractions.CQRS.Event.Internal;
using Fundation.Abstractions.Domain;

namespace Fundation.Abstractions.CQRS.Event;

public interface IAggregatesDomainEventsRequestStore
{
    IReadOnlyList<IDomainEvent> AddEventsFromAggregate<T>(T aggregate)
        where T : IHaveAggregate;

    void AddEvents(IReadOnlyList<IDomainEvent> events);

    IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();
}
