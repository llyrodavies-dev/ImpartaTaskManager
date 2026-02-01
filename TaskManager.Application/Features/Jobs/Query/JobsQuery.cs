using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Jobs.DTOs;
using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Query
{
    public record JobsQuery() : IRequest<PagedResponse<JobDto>>;
}
