using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities
{
    public class Job : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;

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
    }
}
