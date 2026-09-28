namespace Fundation.Abstractions.CQRS.Event.Internal;

public interface IDomainEventContext
{
    IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();
    void MarkUncommittedDomainEventAsCommitted();
}
