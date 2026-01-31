namespace TaskManager.Domain.Common
{
    public class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAtUtc { get; protected set; }
        public string CreatedBy { get; protected set; } = null!;

        public DateTime? ModifiedAtUtc { get; protected set; }
        public string? ModifiedBy { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        public void SetCreated(string createdBy)
        {
            CreatedBy = createdBy;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void SetModified(string modifiedBy)
        {
            ModifiedBy = modifiedBy;
            ModifiedAtUtc = DateTime.UtcNow;
        }
    }
}
