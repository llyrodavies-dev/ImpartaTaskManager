using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Projections
{
    public class TaskItemProjection
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid UserId { get; set; } // Mapped From Job
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
