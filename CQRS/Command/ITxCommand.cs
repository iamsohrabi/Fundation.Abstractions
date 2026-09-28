using Fundation.Abstractions.Persistence;
using MediatR;

namespace Fundation.Abstractions.CQRS.Command;

public interface ITxCommand : ITxCommand<Unit>
{
}

public interface ITxCommand<out T> : ICommand<T>, ITxRequest
    where T : notnull
{
}
