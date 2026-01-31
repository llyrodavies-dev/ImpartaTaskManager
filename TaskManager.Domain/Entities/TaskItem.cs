using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public Guid JobId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public TaskItemStatus Status { get; private set; } = TaskItemStatus.NotStarted;

        public TaskItem() { }

        public TaskItem(Guid jobId, string title, string description, string createdBy)
        {
            JobId = jobId;
            Title = title;
            Description = description;
            SetCreated(createdBy);
        }
    }
}
