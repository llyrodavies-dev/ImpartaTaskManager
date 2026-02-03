using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Common.Services
{
    public class JobStatusService : IJobStatusService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IUnitOfWork _unitOfWork;

        public JobStatusService(IJobRepository jobRepository, IUnitOfWork unitOfWork)
        {
            _jobRepository = jobRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateJobStatusAsync(Guid jobId, string modifiedBy, CancellationToken cancellationToken = default)
        {
            // Get the job with all its tasks
            Job? job = await _jobRepository.GetJobByIdAndTasksAsync(jobId, cancellationToken);

            if (job == null)
                return; // Job not found, nothing to update

            // Calculate new status based on tasks
            JobStatus newJobStatus = CalculateJobStatus(job.Tasks);

            // Only update if status has changed
            if (job.Status != newJobStatus)
            {
                job.UpdateStatus(newJobStatus, modifiedBy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        private static JobStatus CalculateJobStatus(IReadOnlyCollection<TaskItem> tasks)
        {
            // If no tasks, job should be NotStarted
            if (tasks.Count == 0)
                return JobStatus.NotStarted;

            // Get all task statuses (excluding Unspecified which shouldn't exist in business logic)
            var taskStatuses = tasks.Select(t => t.Status).Where(s => s != TaskItemStatus.Unspecified).ToList();

            // If no valid tasks, default to NotStarted
            if (taskStatuses.Count == 0)
                return JobStatus.NotStarted;

            // If all tasks are Completed, job is Completed
            if (taskStatuses.All(s => s == TaskItemStatus.Completed))
                return JobStatus.Completed;

            // If all tasks are NotStarted, job is NotStarted
            if (taskStatuses.All(s => s == TaskItemStatus.NotStarted))
                return JobStatus.NotStarted;

            // If at least one task is InProgress or Blocked, or there's a mix of statuses, job is InProgress
            return JobStatus.InProgress;
        }
    }
}
