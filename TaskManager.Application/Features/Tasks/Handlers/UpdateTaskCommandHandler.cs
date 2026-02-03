using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Handlers
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly ITaskItemRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(IUserAuthorizationService userAuthorizationService, ITaskItemRepository taskRepository, IUnitOfWork unitOfWork)
        {
            _userAuthorizationService = userAuthorizationService;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken = default)
        {
            User domainUser = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);

            await _userAuthorizationService.ValidateTaskOwnershipAsync(request.Id, cancellationToken);

            TaskItem task = await _taskRepository.GetTaskItemByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Task", request.Id);

            task.UpdateDetails(request.Title, request.Description, domainUser.DisplayName);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
