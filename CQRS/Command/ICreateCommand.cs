namespace Fundation.Abstractions.CQRS.Command;

public interface ICreateCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
{
}

public interface ICreateCommand : ICommand
{
}
