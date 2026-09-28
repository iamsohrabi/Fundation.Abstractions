using Fundation.Abstractions.CQRS.Event.Internal;

namespace Fundation.Abstractions.CQRS.Event;

public interface IDomainEventsAccessor
{
    IReadOnlyList<IDomainEvent> UnCommittedDomainEvents { get; }
}
