namespace Fundation.Abstractions.CQRS.Event.Internal;

public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
}
