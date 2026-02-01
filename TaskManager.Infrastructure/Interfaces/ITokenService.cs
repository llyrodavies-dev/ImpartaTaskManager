using TaskManager.Infrastructure.Identity;

namespace TaskManager.Infrastructure.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}