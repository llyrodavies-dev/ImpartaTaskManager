using TaskManager.Application.Features.Users.DTOs;
using Utility.Mediator;

namespace TaskManager.Application.Features.Users.Query
{
    public record class GetUserInfoQuery : IRequest<UserInfoDto>;
}
