using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Services;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Handlers
{
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Unit>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly ITaskItemRepository _taskRepository;
        private readonly IJobStatusService _jobStatusService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskStatusCommandHandler(
            IUserAuthorizationService userAuthorizationService,
            ITaskItemRepository taskRepository,
            IUnitOfWork unitOfWork,
            IJobStatusService jobStatusService)
        {
            _userAuthorizationService = userAuthorizationService;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _jobStatusService = jobStatusService;
        }

        public async Task<Unit> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken = default)
        {
            User domainUser = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);

            TaskItem task = await _taskRepository.GetTaskItemByIdAsync(request.TaskId, cancellationToken)
                ?? throw new NotFoundException("Task", request.TaskId);

            // Prevent modification of completed tasks
            if (task.Status == TaskItemStatus.Completed)
                throw new BadRequestException("Cannot modify a task that has already been completed");

            TaskItemStatus status = (TaskItemStatus)request.Status;

            if (!Enum.IsDefined(typeof(TaskItemStatus), status))
                throw new ArgumentException($"Invalid status value: {request.Status}");

            task.UpdateStatus(status, domainUser.Email ?? "system");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update parent job status based on all its tasks
            await _jobStatusService.UpdateJobStatusAsync(task.JobId, domainUser.Email ?? "system", cancellationToken);

            return Unit.Value;
        }
    }
}
