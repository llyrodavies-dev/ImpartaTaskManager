using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Users.DTOs;
using TaskManager.Application.Features.Users.Query;
using TaskManager.Domain.Entities;
using Utility.Mediator;

namespace TaskManager.Application.Features.Users.Handlers
{
    public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfoDto>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;

        public GetUserInfoQueryHandler(IUserAuthorizationService userAuthorizationService)
        {
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken = default)
        {
            User domainUser = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);

            return new UserInfoDto(
                domainUser.DisplayName,
                domainUser.Email
            );
        }
    }
}
