using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Commands
{
    public record UpdateTaskCommand(Guid Id, string Title, string Description) : IRequest<Unit>;
}
