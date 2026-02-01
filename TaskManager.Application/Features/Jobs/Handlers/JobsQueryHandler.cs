using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Jobs.DTOs;
using TaskManager.Application.Features.Jobs.Query;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Jobs.Handlers
{
    public class JobsQueryHandler : IRequestHandler<JobsQuery, PagedResponse<JobDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;

        public JobsQueryHandler(ICurrentUserService currentUserService, IUserRepository userRepository, IJobRepository jobRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _jobRepository = jobRepository;
        }

        public async Task<PagedResponse<JobDto>> Handle(JobsQuery request, CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            User? domainUser = await _userRepository.GetUserByIdentityUserId(_currentUserService.UserId.Value) 
                ?? throw new NotFoundException("IdentityUser", _currentUserService.UserId.Value);

            (List<Job> jobs, int totalCount) = await _jobRepository.GetJobsByUserIdPagedAsync(domainUser.Id, cancellationToken);

            return new PagedResponse<JobDto>()
            {
                Items = JobDto.FromDomainList(jobs),
                TotalCount = totalCount
            };
        }
    }
}
