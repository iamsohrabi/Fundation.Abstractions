using Fundation.Abstractions.CQRS.Event.Internal;

namespace Fundation.Abstractions.Persistence.EventStore.Projections;

public interface IHaveReadProjection
{
    Task ProjectAsync<T>(IStreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : IDomainEvent;
}
