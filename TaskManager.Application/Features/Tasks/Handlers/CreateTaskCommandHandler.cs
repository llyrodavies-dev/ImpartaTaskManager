using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Handlers
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskItemDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;
        private readonly ITaskItemRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskCommandHandler(ICurrentUserService currentUserService, IUserRepository userRepository, IJobRepository jobRepository, ITaskItemRepository taskRepository, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _jobRepository = jobRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskItemDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            var domainUser = await _userRepository.GetUserByIdentityUserId(_currentUserService.UserId.Value)
                ?? throw new NotFoundException("User", _currentUserService.UserId.Value);

            Job job = await _jobRepository.GetJobByIdAndTasksAsync(request.JobId, cancellationToken)
                ?? throw new NotFoundException("Job", request.JobId);

            if (job.UserId != domainUser.Id)
                throw new UnauthorizedAccessException("You don't have permission to add tasks to this job");

            var newTask = new TaskItem(job.Id, request.Title, request.Description, _currentUserService.Email ?? "system");
            await _taskRepository.AddTaskAsync(newTask, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return TaskItemDto.FromDomain(newTask);
        }
    }
}
