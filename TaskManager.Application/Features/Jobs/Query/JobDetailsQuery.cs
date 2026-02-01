using TaskManager.Application.Features.Jobs.DTOs;
using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Query
{
    public record JobDetailsQuery(Guid JobId) : IRequest<JobDetailDto>;
}
