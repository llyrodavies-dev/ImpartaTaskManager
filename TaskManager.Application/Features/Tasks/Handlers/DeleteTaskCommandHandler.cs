using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Handlers
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly ITaskItemRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaskCommandHandler(
            ITaskItemRepository taskRepository,
            IUnitOfWork unitOfWork,
            IUserAuthorizationService userAuthorizationService)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetTaskItemByIdAsync(request.TaskId, cancellationToken)
                ?? throw new NotFoundException("Task", request.TaskId);

            await _userAuthorizationService.ValidateTaskOwnershipAsync(task.Id, cancellationToken);

            _taskRepository.DeleteTask(task);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
