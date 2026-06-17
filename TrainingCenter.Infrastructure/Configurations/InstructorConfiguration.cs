using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.ToTable("Instructors");

            builder.HasKey(e => e.InstructorId);

            // Self-reference manager relationship
            builder.HasIndex(e => e.ManagerId, "IX_Instructors_ManagerId");

            builder.HasIndex(x => x.UserId)
              .IsUnique();

            builder.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.HireDate)
                .HasColumnType("date")
                .IsRequired();

            // Default = active instructor
            builder.Property(e => e.IsActive)
                      .HasDefaultValue(true)
                      .IsRequired();


            builder.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsRequired();


            builder.Property(e => e.Salary)
                .HasColumnType("decimal(10,2)")
                .IsRequired();


            // One manager can manage many instructors
            builder.HasOne(d => d.Manager)
                 .WithMany(p => p.Subordinates)
                 .HasForeignKey(d => d.ManagerId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .HasConstraintName("FK_Instructors_Manager");

            builder.HasOne(x => x.User)
                .WithOne(x => x.Instructor)
                .HasForeignKey<Instructor>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
