using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Commands
{
    public record DeleteJobCommand(Guid Id) : IRequest<Unit>
    {
    }
}
