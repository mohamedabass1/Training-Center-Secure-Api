using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Domain.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;


        public DateOnly DateOfBirth { get; set; }
        public DateTime RegisteredAt { get; set; }

        public StudentStatus Status { get; set; }
        public string? PhoneNumber { get; set; }


        public int UserId { get; set; }

        public User User { get; set; } = null!;


        // One student has one profile.
        public StudentProfile? StudentProfile { get; set; }

        // One student can enroll in many courses.
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

}
