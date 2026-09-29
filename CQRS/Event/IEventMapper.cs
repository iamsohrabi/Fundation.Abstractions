using Fundation.Abstractions.CQRS.Event.Internal;
using Fundation.Abstractions.Messaging;

namespace Fundation.Abstractions.CQRS.Event;

public interface IEventMapper : IIDomainNotificationEventMapper, IIntegrationEventMapper
{
}

public interface IIDomainNotificationEventMapper
{
    IReadOnlyList<IDomainNotificationEvent> MapToDomainNotificationEvents(IReadOnlyList<IDomainEvent> domainEvents);
    IDomainNotificationEvent MapToDomainNotificationEvent(IDomainEvent domainEvent);
}

public interface IIntegrationEventMapper
{
    IReadOnlyList<IIntegrationEvent> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents);
    IIntegrationEvent MapToIntegrationEvent(IDomainEvent domainEvent);
}
