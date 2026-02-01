using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Jobs.Commands;
using TaskManager.Application.Features.Jobs.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Handlers
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, JobDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;

        public CreateJobCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IUserRepository userRepository, IJobRepository jobRepository)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _jobRepository = jobRepository;
        }

        public async Task<JobDto> Handle(CreateJobCommand request, CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            User? domainUser = await _userRepository.GetUserByIdentityUserId(_currentUserService.UserId.Value)
                ?? throw new NotFoundException("IdentityUser", _currentUserService.UserId.Value);

            Job newJob = new(domainUser.Id, request.Title, _currentUserService.Email ?? "system");

            await _jobRepository.AddAsync(newJob, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return JobDto.FromDomain(newJob);
        }
    }
}
