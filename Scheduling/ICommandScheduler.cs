using Fundation.Abstractions.CQRS.Command;

namespace Fundation.Abstractions.Scheduling;

public interface ICommandScheduler
{
    Task ScheduleAsync(
        IInternalCommand internalCommandCommand,
        CancellationToken cancellationToken = default);

    Task ScheduleAsync(
        IInternalCommand[] internalCommandCommands,
        CancellationToken cancellationToken = default);
}
