using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Dashboard.DTOs;
using TaskManager.Application.Features.Dashboard.Query;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Dashboard.Handlers
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDataDto>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public GetDashboardDataQueryHandler(
            IJobRepository jobRepository,
            IUserAuthorizationService userAuthorizationService)
        {
            _jobRepository = jobRepository;
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<DashboardDataDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken = default)
        {
            User user = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);
            List<Job> jobs = await _jobRepository.GetAllJobsWithTasksAsync(user.Id, cancellationToken);

            // Calculate job summary
            int totalJobs = jobs.Count;
            int activeJobs = jobs.Count(j => j.Status == JobStatus.InProgress || j.Status == JobStatus.NotStarted);
            int completedJobs = jobs.Count(j => j.Status == JobStatus.Completed);

            // Get all tasks from all jobs
            List<TaskItem> allTasks = jobs.SelectMany(j => j.Tasks).ToList();

            // Calculate task summary
            int totalTasks = allTasks.Count;
            int notStartedTasks = allTasks.Count(t => t.Status == TaskItemStatus.NotStarted);
            int inProgressTasks = allTasks.Count(t => t.Status == TaskItemStatus.InProgress);
            int blockedTasks = allTasks.Count(t => t.Status == TaskItemStatus.Blocked);
            int completedTasks = allTasks.Count(t => t.Status == TaskItemStatus.Completed);

            // Calculate overall progress
            int overallPercent = totalTasks > 0
                ? (int)Math.Round((double)completedTasks / totalTasks * 100)
                : 0;

            JobSummaryDto jobSummary = new JobSummaryDto(totalJobs, activeJobs, completedJobs);
            TaskSummaryDto taskSummary = new TaskSummaryDto(totalTasks, notStartedTasks, inProgressTasks, blockedTasks, completedTasks);
            ProgressDto progress = new ProgressDto(overallPercent);

            return new DashboardDataDto(jobSummary, taskSummary, progress);
        }
    }
}
