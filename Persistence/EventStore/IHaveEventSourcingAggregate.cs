using Fundation.Abstractions.CQRS.Event.Internal;
using Fundation.Abstractions.Domain;
using Fundation.Abstractions.Domain.EventSourcing;
using Fundation.Abstractions.Persistence.EventStore.Projections;

namespace Fundation.Abstractions.Persistence.EventStore;

public interface IHaveEventSourcingAggregate :
    IHaveAggregateStateProjection,
    IHaveAggregate,
    IHaveEventSourcedAggregateVersion
{
    /// <summary>
    /// Loads the current state of the aggregate from a list of events.
    /// </summary>
    /// <param name="history">Domain events from the aggregate stream.</param>
    void LoadFromHistory(IEnumerable<IDomainEvent> history);
}
