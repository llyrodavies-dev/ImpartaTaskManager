namespace TaskManager.Application.Common.Interfaces
{
    public interface IJobStatusService
    {
        Task UpdateJobStatusAsync(Guid jobId, string modifiedBy, CancellationToken cancellationToken = default);
    }
}
