using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Tasks.DTOs;
using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Query
{
    public class TasksQuery : IRequest<PagedResponse<TaskItemDto>>;
}
