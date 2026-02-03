using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Jobs.Commands;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Handlers
{
    public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand, Unit>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteJobCommandHandler(
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IJobRepository jobRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _jobRepository = jobRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteJobCommand request, CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            var domainUser = await _userRepository.GetUserByIdentityUserId(_currentUserService.UserId.Value)
                ?? throw new NotFoundException("User", _currentUserService.UserId.Value);

            var job = await _jobRepository.GetJobByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Job", request.Id);

            // Verify the job belongs to the current user
            if (job.UserId != domainUser.Id)
                throw new UnauthorizedAccessException("You don't have permission to delete this job");

            _jobRepository.Delete(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
