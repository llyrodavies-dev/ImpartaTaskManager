using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
