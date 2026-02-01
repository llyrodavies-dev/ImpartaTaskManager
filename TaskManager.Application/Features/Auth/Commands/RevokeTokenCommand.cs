using Utility.Mediator;

namespace TaskManager.Application.Features.Auth.Commands
{
    public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;
}
