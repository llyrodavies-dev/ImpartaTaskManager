using TaskManager.Application.Features.Jobs.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.DTOs
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }

        // Mapping methods
        public static TaskItemDto FromDomain(TaskItem task)
        {
            return new TaskItemDto
            {
                Id = task.Id,
                JobId = task.JobId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAtUtc = task.CreatedAtUtc,
                CreatedBy = task.CreatedBy,
                ModifiedAtUtc = task.ModifiedAtUtc,
                ModifiedBy = task.ModifiedBy
            };
        }

        public static List<TaskItemDto> FromDomainList(List<TaskItem> tasks)
        {
            return [.. tasks.Select(FromDomain)];
        }
    }
}
