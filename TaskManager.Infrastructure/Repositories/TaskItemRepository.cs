using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Projections;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskItemRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResponse<TaskItemProjection>> GetTaskItemsFilteredAsync(string queryString, Dictionary<string, object> args, Func<IQueryable<TaskItemProjection>, IQueryable<TaskItemProjection>>? sort, CancellationToken cancellationToken, int skip = 0, int? take = null)
        {
            IQueryable<TaskItemProjection> query = _dbContext.Tasks
                .AsNoTracking()
                .Join(_dbContext.Jobs,
                    taskItem => taskItem.JobId,
                    job => job.Id,
                    (taskItem, job) => new TaskItemProjection
                    {
                        Id = taskItem.Id,
                        JobId = job.Id,
                        UserId = job.UserId,
                        Title = taskItem.Title,
                        Description = taskItem.Description,
                        Status = taskItem.Status,
                        CreatedAtUtc = taskItem.CreatedAtUtc,
                        CreatedBy = taskItem.CreatedBy,
                        ModifiedAtUtc = taskItem.ModifiedAtUtc,
                        ModifiedBy = taskItem.ModifiedBy
                    });

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                // This is using Dynamic LINQ for more complex query strings
                query = query.Where(queryString, args);
            }

            // Apply sorting logic
            if (sort != null)
            {
                query = sort(query);
            }

            // Get total count BEFORE pagination - async and efficient
            var totalCount = await query.CountAsync(cancellationToken);

            if (skip > 0)
            {
                query = query.Skip(skip);
            }
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return new PagedResponse<TaskItemProjection>
            {
                Items = await query.ToListAsync(),
                TotalCount = totalCount
            };
        }
    }
}
