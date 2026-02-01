using TaskManager.Application.Features.Tasks.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Jobs.DTOs
{
    public class JobDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }

        // Related data
        public List<TaskItemDto> Tasks { get; set; } = [];

        // Mapping method
        public static JobDetailDto FromDomain(Job job)
        {
            return new JobDetailDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                CreatedAtUtc = job.CreatedAtUtc,
                CreatedBy = job.CreatedBy,
                ModifiedAtUtc = job.ModifiedAtUtc,
                ModifiedBy = job.ModifiedBy,
                Tasks = TaskItemDto.FromDomainList(job.Tasks.ToList())
            };
        }
    }
}
