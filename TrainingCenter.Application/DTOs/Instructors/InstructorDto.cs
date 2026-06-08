namespace TrainingCenter.Application.DTOs.Instructors
{
    public class InstructorDto
    {
        public int InstructorId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateOnly HireDate { get; set; }

        public decimal Salary { get; set; }

        public bool IsActive { get; set; }

        public int? ManagerId { get; set; }

        public string? ManagerName { get; set; }
    }



}
