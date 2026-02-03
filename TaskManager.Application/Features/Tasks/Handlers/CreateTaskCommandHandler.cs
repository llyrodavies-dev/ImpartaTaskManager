using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IJobRepository _jobRepository;
        private readonly ITaskItemRepository _taskRepository;
        private readonly IJobStatusService _jobStatusService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTaskCommandHandler(IJobRepository jobRepository, ITaskItemRepository taskRepository, IUnitOfWork unitOfWork, IJobStatusService jobStatusService, IUserAuthorizationService userAuthorizationService)
        {
            _jobRepository = jobRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _jobStatusService = jobStatusService;
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<TaskItemDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken = default)
        {
            User domainUser = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);

            Job job = await _jobRepository.GetJobByIdAndTasksAsync(request.JobId, cancellationToken)
                ?? throw new NotFoundException("Job", request.JobId);

            if (job.UserId != domainUser.Id)
                throw new UnauthorizedAccessException("You don't have permission to add tasks to this job");

            var newTask = new TaskItem(job.Id, request.Title, request.Description, domainUser.Email ?? "system");
            await _taskRepository.AddTaskAsync(newTask, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Update parent job status based on all its tasks
            await _jobStatusService.UpdateJobStatusAsync(job.Id, domainUser.Email ?? "system", cancellationToken);

            return TaskItemDto.FromDomain(newTask);
        }
    }
}
