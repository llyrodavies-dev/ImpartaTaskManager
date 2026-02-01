using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime? RevokedAt { get; private set; }

        public RefreshToken() { }

        public RefreshToken(string token, Guid userId, DateTime expiresAt, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Refresh token cannot be empty.");

            if (userId == Guid.Empty)
                throw new DomainException("User ID cannot be empty.");

            if (expiresAt <= DateTime.UtcNow)
                throw new DomainException("Expiry date must be in the future.");

            Token = token;
            UserId = userId;
            ExpiresAt = expiresAt;
            IsRevoked = false;

            SetCreated(createdBy);
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;

        public void Revoke(string revokedBy)
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            SetModified(revokedBy);
        }
    }
}
