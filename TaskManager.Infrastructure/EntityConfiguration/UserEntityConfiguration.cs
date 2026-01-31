using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Configuration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.ProfileImagePath).HasMaxLength(500);
            builder.Property(u => u.CreatedBy).IsRequired().HasMaxLength(256);
            builder.Property(u => u.ModifiedBy).HasMaxLength(256);

            builder.HasMany(u => u.Jobs).WithOne().HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
