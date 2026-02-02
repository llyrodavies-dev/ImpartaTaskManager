using TaskManager.Application.Common.Models;
using TaskManager.Domain.Projections;

namespace TaskManager.Application.Common.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<PagedResponse<TaskItemProjection>> GetTaskItemsFilteredAsync(string queryString, Dictionary<string, object> args, Func<IQueryable<TaskItemProjection>, IQueryable<TaskItemProjection>>? sort, CancellationToken cancellationToken, int skip = 0, int? take = null);
    }
}
