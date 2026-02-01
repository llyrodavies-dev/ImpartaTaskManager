using TaskManager.Application.Features.Tasks.DTOs;
using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Commands
{
    public record CreateTaskCommand(Guid JobId, string Title, string Description) : IRequest<TaskItemDto>;
}
