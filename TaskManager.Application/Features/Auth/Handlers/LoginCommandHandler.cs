using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Domain.Exceptions;
using Utility.Mediator;

namespace TaskManager.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken = default)
        {
            if(!await _authService.UserExistsAsync(request.Email, cancellationToken))
                throw new DomainException("User does not exist"); //TODO: Update to application exception

            AuthResult authResult = await _authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
            return authResult;
        }
    }
}
