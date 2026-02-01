using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email); // TODO: Remove if not used
        Task<User?> GetUserByIdentityUserId(Guid identityUserId);
        Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    }
}
