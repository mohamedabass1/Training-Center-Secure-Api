using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Students
{
    public class StudentDto
    {
        public int StudentId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateOnly DateOfBirth { get; set; }

        public StudentStatus Status { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime RegisteredAt { get; set; }
    }

}
