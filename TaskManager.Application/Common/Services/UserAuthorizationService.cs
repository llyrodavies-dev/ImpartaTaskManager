using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Common.Services
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITaskItemRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;

        public UserAuthorizationService(ICurrentUserService currentUserService, ITaskItemRepository taskRepository, IUserRepository userRepository, IJobRepository jobRepository)
        {
            _currentUserService = currentUserService;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _jobRepository = jobRepository;
        }

        public async Task<User> GetAuthenticatedUserAsync(CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            return await _userRepository.GetUserByIdentityUserId(_currentUserService.UserId.Value, cancellationToken)
                ?? throw new NotFoundException("User", _currentUserService.UserId.Value);
        }

        public async Task ValidateTaskOwnershipAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            var user = await GetAuthenticatedUserAsync(cancellationToken);

            var task = await _taskRepository.GetTaskItemByIdAsync(taskId, cancellationToken)
                ?? throw new NotFoundException("Task", taskId);

            var job = await _jobRepository.GetJobByIdAsync(task.JobId, cancellationToken)
                ?? throw new NotFoundException("Job", task.JobId);

            if (job.UserId != user.Id)
                throw new UnauthorizedAccessException("You don't have permission to modify this task");
        }
    }
}