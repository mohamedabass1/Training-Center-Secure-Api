using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Students
{
    public class UpdateStudentDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Password { get; set; }
        public StudentStatus Status { get; set; }
        public string? PhoneNumber { get; set; }
    }




}
