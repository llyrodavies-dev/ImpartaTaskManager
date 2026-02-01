using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public JobRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Job>> GetAllJobsAsync(Guid UserId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs.Where(x=>x.UserId == UserId).ToListAsync(cancellationToken);
        }

        public async Task<(List<Job>, int)> GetJobsByUserIdPagedAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Jobs
                .Where(j => j.UserId == userId);

            var totalCount = await query.CountAsync(cancellationToken);

            return (await query.ToListAsync(cancellationToken), totalCount);
        }
    }
}
