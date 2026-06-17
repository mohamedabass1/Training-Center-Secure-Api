using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.UserId);

            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.Property(x => x.Email)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(x => x.PasswordHash)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(x => x.Role)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Users_Role",
                    "[Role] IN ('Student','Instructor','Admin')");
            });
        }
    }
}
