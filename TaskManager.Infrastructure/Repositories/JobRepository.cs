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
            return await _dbContext.Jobs
                .AsNoTracking()
                .Where(x=>x.UserId == UserId).ToListAsync(cancellationToken);
        }

        public async Task<List<Job>> GetAllJobsWithTasksAsync(Guid UserId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs
                .AsNoTracking()
                .Include(j => j.Tasks)
                .Where(x=>x.UserId == UserId).ToListAsync(cancellationToken);
        }

        public async Task<(List<Job>, int)> GetJobsByUserIdPagedAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Jobs
                .AsNoTracking()
                .Include(j => j.Tasks)
                .Where(j => j.UserId == userId);

            var totalCount = await query.CountAsync(cancellationToken);

            return (await query.ToListAsync(cancellationToken), totalCount);
        }

        public async Task<Job?> GetJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        }

        public async Task<Job?> GetJobByIdAndTasksAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs
                .Include(j => j.Tasks)
                .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        }

        public async Task<Job?> GetJobByIdAndTasksAsNoTrackingAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Jobs
                .AsNoTracking()
                .Include(j => j.Tasks)
                .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        }

        public async Task AddAsync(Job newJob, CancellationToken cancellationToken)
        {
            await _dbContext.Jobs.AddAsync(newJob, cancellationToken);
        }

        public void Delete(Job job)
        {
            _dbContext.Jobs.Remove(job);
        }
    }
}
