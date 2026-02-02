using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.EntityConfiguration
{
    public class TaskItemEntityConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("TaskItems");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.JobId)
                .IsRequired();
        }
    }
}
