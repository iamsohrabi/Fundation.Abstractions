using Fundation.Abstractions.Persistence;

namespace Fundation.Abstractions.CQRS.Command;

public interface ITxInternalCommand : IInternalCommand, ITxRequest
{
}
