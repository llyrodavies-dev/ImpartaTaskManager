using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUserByIdentityUserId(Guid identityUserId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.DomainUsers.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId, cancellationToken);
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddAsync(user, cancellationToken);
        }
    }
}
