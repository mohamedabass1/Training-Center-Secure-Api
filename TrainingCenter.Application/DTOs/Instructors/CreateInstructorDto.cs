namespace TrainingCenter.Application.DTOs.Instructors
{

    public class CreateInstructorDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateOnly HireDate { get; set; }
        public decimal Salary { get; set; }
        public int? ManagerId { get; set; }
        public bool IsActive { get; set; }
    }


    public class AssignManagerDto
    {
        public int? ManagerId { get; set; }
    }


}
