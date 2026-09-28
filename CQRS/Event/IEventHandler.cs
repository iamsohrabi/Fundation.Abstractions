using MediatR;

namespace Fundation.Abstractions.CQRS.Event;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{
}
