using TaskManager.Application.Features.Jobs.DTOs;
using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Commands
{
    public record CreateJobCommand(string Title) : IRequest<JobDto>;
}
