using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Commands
{
    public record UpdateTaskStatusCommand(Guid TaskId, int Status) : IRequest<Unit>;
}
