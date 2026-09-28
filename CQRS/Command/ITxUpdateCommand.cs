using Fundation.Abstractions.Persistence;
using MediatR;

namespace Fundation.Abstractions.CQRS.Command;

public interface ITxUpdateCommand<out TResponse> : IUpdateCommand<TResponse>, ITxRequest
    where TResponse : notnull
{
}

public interface ITxUpdateCommand : ITxUpdateCommand<Unit>
{
}
