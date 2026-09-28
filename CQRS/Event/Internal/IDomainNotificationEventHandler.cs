namespace Fundation.Abstractions.CQRS.Event.Internal;

public interface IDomainNotificationEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainNotificationEvent

{
}
