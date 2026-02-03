using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Commands
{
    public record UpdateJobCommand(Guid JobId, string Title) : IRequest<Unit>;
}
