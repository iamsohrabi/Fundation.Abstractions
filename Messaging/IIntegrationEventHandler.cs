using Fundation.Abstractions.CQRS.Event;

namespace Fundation.Abstractions.Messaging;

public interface IIntegrationEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
}
