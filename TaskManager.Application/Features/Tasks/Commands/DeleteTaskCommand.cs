using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Commands
{
    public record DeleteTaskCommand(Guid TaskId) : IRequest<Unit>;
}
