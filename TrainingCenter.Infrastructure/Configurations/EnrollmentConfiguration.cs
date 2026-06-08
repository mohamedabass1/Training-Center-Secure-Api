using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(e => e.EnrollmentId);

            // Indexes for faster search
            builder.HasIndex(e => e.CourseId, "IX_Enrollments_CourseId");
            builder.HasIndex(e => e.Status, "IX_Enrollments_Status");
            builder.HasIndex(e => e.StudentId, "IX_Enrollments_StudentId");


            // Prevent duplicate enrollment:
            // Same student cannot enroll twice
            builder.HasIndex(e => new { e.StudentId, e.CourseId },
                            "UQ_Enrollments_StudentId_CourseId")
                  .IsUnique();


            // Completion date
            builder.Property(e => e.CompletionDate)
                  .HasColumnType("datetime");


            // Default enrollment date = now
            builder.Property(e => e.EnrollmentDate)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime").IsRequired();


            // Decimal grades
            builder.Property(e => e.FinalGrade)
                  .HasColumnType("decimal(5, 2)");


            builder.Property(e => e.ProgressPercent)
                  .HasColumnType("decimal(5, 2)")
                  .IsRequired();


            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();


            // Many enrollments belong to one course
            builder.HasOne(d => d.Course)
                  .WithMany(p => p.Enrollments)
                  .HasForeignKey(d => d.CourseId)
                  .HasConstraintName("FK_Enrollments_Courses");


            // Many enrollments belong to one student
            builder.HasOne(d => d.Student)
                  .WithMany(p => p.Enrollments)
                  .HasForeignKey(d => d.StudentId)
                  .HasConstraintName("FK_Enrollments_Students");
        }
    }
}
