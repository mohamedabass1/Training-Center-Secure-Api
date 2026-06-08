using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Students
{
    public class CreateStudentDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public StudentStatus Status { get; set; }
        public string? PhoneNumber { get; set; }
    }




}
