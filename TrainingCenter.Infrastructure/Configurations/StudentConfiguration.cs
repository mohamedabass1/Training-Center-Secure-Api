using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(e => e.StudentId);

            builder.HasIndex(e => e.Status, "IX_Students_Status");

            // Email unique
            builder.HasIndex(e => e.Email, "UQ_Students_Email")
                  .IsUnique();


            builder.Property(e => e.Email).HasMaxLength(150).IsRequired();
            builder.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.PhoneNumber).HasMaxLength(30);


            // Registration date defaults to now
            builder.Property(e => e.RegisteredAt)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime")
                  .IsRequired();


            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
