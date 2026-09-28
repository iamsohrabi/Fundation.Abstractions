using Fundation.Abstractions.Messaging.Context;

namespace Fundation.Abstractions.Messaging;

public interface IMessageHandler<in TMessage>
    where TMessage : class, IMessage
{
    Task HandleAsync(IConsumeContext<TMessage> messageContext, CancellationToken cancellationToken = default);
}
