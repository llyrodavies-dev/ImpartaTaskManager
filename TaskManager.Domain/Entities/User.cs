using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities
{
    public class User : BaseEntity
    {
        // Link to Identity User (Foreign Key)
        public Guid IdentityUserId { get; private set; }

        public string Email { get; private set; } = null!;
        public string DisplayName { get; private set; } = null!;
        public string? ProfileImagePath { get; private set; }

        private readonly List<Job> _jobs = [];
        public IReadOnlyCollection<Job> Jobs => _jobs;
        public User() { }

        public User(Guid identityUserId, string email, string displayName, string createdBy)
        {
            if(identityUserId == Guid.Empty)
                throw new DomainException("Identity User ID cannot be empty.");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(displayName))
                throw new DomainException("Display name cannot be empty.");

            IdentityUserId = identityUserId;
            Email = email;
            DisplayName = displayName;

            SetCreated(createdBy);
        }
    }
}
