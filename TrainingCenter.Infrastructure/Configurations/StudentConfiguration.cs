using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {

        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(e => e.StudentId);

            builder.HasIndex(e => e.Status, "IX_Students_Status");



            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.PhoneNumber).HasMaxLength(30);

            builder.Property(e => e.DateOfBirth)
                .HasColumnType("date")
                .IsRequired();

            // Registration date defaults to now
            builder.Property(e => e.RegisteredAt)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime")
                  .IsRequired();


            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Students_Status",
                    "[Status] IN ('Active','Suspended','Graduated')");
            });


            builder.HasOne(x => x.User)
                .WithOne(x => x.Student)
                .HasForeignKey<Student>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
