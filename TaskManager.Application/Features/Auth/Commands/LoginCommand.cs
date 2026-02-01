using TaskManager.Application.Common.Models;
using Utility.Mediator;

namespace TaskManager.Application.Features.Auth.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResult> { }
}
