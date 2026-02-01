
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Jobs.DTOs
{
    public class JobDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAtUtc { get; set; }
        public string? ModifiedBy { get; set; }

        // Mapping methods
        public static JobDto FromDomain(Job job)
        {
            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                CreatedAtUtc = job.CreatedAtUtc,
                CreatedBy = job.CreatedBy,
                ModifiedAtUtc = job.ModifiedAtUtc,
                ModifiedBy = job.ModifiedBy
            };
        }

        public static List<JobDto> FromDomainList(List<Job> jobs)
        {
            return [.. jobs.Select(FromDomain)];
        }
    }
}
