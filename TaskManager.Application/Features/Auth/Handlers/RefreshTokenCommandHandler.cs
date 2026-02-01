using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Auth.Commands;
using Utility.Mediator;

namespace TaskManager.Application.Features.Auth.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        }
    }
}
