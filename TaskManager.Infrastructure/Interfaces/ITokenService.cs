using TaskManager.Infrastructure.Identity;

namespace TaskManager.Infrastructure.Interfaces
{
    public interface ITokenService
    {
        (string, DateTime) GenerateToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}