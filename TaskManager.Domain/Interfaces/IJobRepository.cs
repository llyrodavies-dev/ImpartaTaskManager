using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces
{
    public interface IJobRepository
    {
        Task<List<Job>> GetAllJobsAsync(Guid UserId, CancellationToken cancellationToken = default);
        Task<(List<Job>, int)> GetJobsByUserIdPagedAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Job?> GetJobByIdAndTasksAsync(Guid jobId, CancellationToken cancellationToken = default);
        Task AddAsync(Job newJob, CancellationToken cancellationToken);
    }
}
