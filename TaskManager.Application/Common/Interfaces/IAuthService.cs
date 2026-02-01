using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Auth.Commands;

namespace TaskManager.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<AuthResult> RegisterAsync(RegisterCommand request, CancellationToken cancellationToken = default);
        Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
