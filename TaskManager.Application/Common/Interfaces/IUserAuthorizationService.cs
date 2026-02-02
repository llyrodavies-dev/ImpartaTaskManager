using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces
{
    public interface IUserAuthorizationService
    {
        Task<User> GetAuthenticatedUserAsync(CancellationToken cancellationToken = default);
    }
}
