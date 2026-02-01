using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Auth.Commands;
using Utility.Mediator;

namespace TaskManager.Application.Features.Auth.Handlers
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Unit>
    {
        private readonly IAuthService _authService;

        public RevokeTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            await _authService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
            return Unit.Value;
        }
    }
}
