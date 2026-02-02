using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Common.Services
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public UserAuthorizationService(ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<User> GetAuthenticatedUserAsync(CancellationToken cancellationToken = default)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            return await _userRepository.GetUserByIdentityUserId(_currentUserService.UserId.Value, cancellationToken)
                ?? throw new NotFoundException("User", _currentUserService.UserId.Value);
        }
    }
}