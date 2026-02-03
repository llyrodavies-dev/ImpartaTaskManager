using TaskManager.Application.Features.Dashboard.DTOs;
using Utility.Mediator;

namespace TaskManager.Application.Features.Dashboard.Query
{
    public record GetDashboardDataQuery : IRequest<DashboardDataDto>
    {
    }
}
