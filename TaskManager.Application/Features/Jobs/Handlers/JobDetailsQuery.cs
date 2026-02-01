using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Jobs.DTOs;
using TaskManager.Application.Features.Jobs.Query;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Handlers
{
    public class JobDetailsQueryHandler : IRequestHandler<JobDetailsQuery, JobDetailDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;

        public JobDetailsQueryHandler(ICurrentUserService currentUserService, IUserRepository userRepository, IJobRepository jobRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _jobRepository = jobRepository;
        }

        public async Task<JobDetailDto> Handle(JobDetailsQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            User? domainUser = await _userRepository.GetUserByIdentityUserId(_currentUserService.UserId.Value)
                ?? throw new NotFoundException("IdentityUser", _currentUserService.UserId.Value);

            Job? job = await _jobRepository.GetJobByIdAndTasksAsync(request.JobId, cancellationToken)
                ?? throw new NotFoundException(nameof(Job), request.JobId);

            if (job.UserId != domainUser.Id)
                throw new UnauthorizedAccessException("User does not have access to this job");

            return JobDetailDto.FromDomain(job);
        }
    }
}
