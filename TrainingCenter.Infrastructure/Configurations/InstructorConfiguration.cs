using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(e => e.InstructorId);

            // Self-reference manager relationship
            builder.HasIndex(e => e.ManagerId, "IX_Instructors_ManagerId");


            // Email must be unique
            builder.HasIndex(e => e.Email, "UQ_Instructors_Email")
                      .IsUnique();


            builder.Property(e => e.Email).
                HasMaxLength(150)
                .IsRequired();

            builder.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsRequired();


            // Default = active instructor
            builder.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .IsRequired();


            builder.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsRequired();


            builder.Property(e => e.Salary)
                      .HasColumnType("decimal(10, 2)");


            // One manager can manage many instructors
            builder.HasOne(d => d.Manager)
                      .WithMany(p => p.InverseManager)
                      .HasForeignKey(d => d.ManagerId)
                      .HasConstraintName("FK_Instructors_Manager");

        }
    }
}
