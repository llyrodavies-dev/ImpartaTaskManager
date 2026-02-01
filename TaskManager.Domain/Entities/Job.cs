using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class Job : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public JobStatus Status { get; private set; } = JobStatus.NotStarted;

        private readonly List<TaskItem> _tasks = [];
        public IReadOnlyCollection<TaskItem> Tasks => _tasks;

        public Job() { }

        public Job(Guid userId, string title, string createdBy)
        {
            UserId = userId;

            Title = title;
            SetCreated(createdBy);
        }

        public void AddTask(string title, string description, string createdBy)
        {
            var task = new TaskItem(Id, title, description, createdBy);
            _tasks.Add(task);
        }

        public void UpdateTitle(string title, string modifiedBy)
        {
            Title = title;
            SetModified(modifiedBy);
        }

        public void UpdateStatus(JobStatus status, string modifiedBy)
        {
            Status = status;
            SetModified(modifiedBy);
        }
    }
}
