using Fundation.Abstractions.Persistence;
using MediatR;

namespace Fundation.Abstractions.CQRS.Command;

public interface ITxCreateCommand<out TResponse> : ICommand<TResponse>, ITxRequest
    where TResponse : notnull
{
}

public interface ITxCreateCommand : ITxCreateCommand<Unit>
{
}
