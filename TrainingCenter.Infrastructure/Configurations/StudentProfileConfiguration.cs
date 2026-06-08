using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            // Primary key = StudentId
            builder.HasKey(e => e.StudentId);


            // Not auto increment
            builder.Property(e => e.StudentId)
                  .ValueGeneratedNever().IsRequired();

            builder.Property(e => e.Address).HasMaxLength(200);
            builder.Property(e => e.Bio).HasMaxLength(500);
            builder.Property(e => e.City).HasMaxLength(100);
            builder.Property(e => e.Country).HasMaxLength(100);
            builder.Property(e => e.LinkedInUrl).HasMaxLength(200);


            // One Student has one Profile
            builder.HasOne(d => d.Student)
                  .WithOne(p => p.StudentProfile)
                  .HasForeignKey<StudentProfile>(d => d.StudentId)
                  .HasConstraintName("FK_StudentProfiles_Students");
        }
    }
}
