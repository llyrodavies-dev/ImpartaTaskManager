using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.EntityConfiguration
{
    public class JobEntityConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Status)
                .IsRequired();

            builder.Property(j => j.UserId)
                .IsRequired();

            // Configure the Tasks collection with backing field
            builder.HasMany(j => j.Tasks)
                .WithOne()  // No inverse navigation property
                .HasForeignKey(t => t.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tell EF Core to use the backing field
            builder.Navigation(j => j.Tasks)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Explicitly map the backing field
            builder.Metadata
                .FindNavigation(nameof(Job.Tasks))!
                .SetField("_tasks");
        }
    }
}
