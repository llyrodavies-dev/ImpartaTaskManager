using TaskManager.Application.Common.Models;
using Utility.Mediator;

namespace TaskManager.Application.Features.Auth.Commands
{
    public record RegisterCommand(string Email, string Password, string DisplayName) : IRequest<AuthResult>;
}
