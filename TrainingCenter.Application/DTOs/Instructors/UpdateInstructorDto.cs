namespace TrainingCenter.Application.DTOs.Instructors
{
    public class UpdateInstructorDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Salary { get; set; }
        public int? ManagerId { get; set; }
        public bool IsActive { get; set; }
    }




}
