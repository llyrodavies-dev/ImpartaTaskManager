using TaskManager.Domain.Common;

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
    }
}
