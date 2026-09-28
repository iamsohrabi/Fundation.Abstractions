using MediatR;

namespace Fundation.Abstractions.Messaging;

public interface IMessage : INotification
{
    Guid MessageId { get; }
    DateTime Created { get; }
}
