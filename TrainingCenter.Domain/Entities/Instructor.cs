namespace TrainingCenter.Domain.Entities
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public DateOnly HireDate { get; set; }
        public decimal Salary { get; set; }
        public int? ManagerId { get; set; } // Points to another instructor who is the manager.
        public bool IsActive { get; set; }


        public int UserId { get; set; }

        public User User { get; set; }


        // One instructor can teach many courses.
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        // Collection of instructors managed by this instructor.
        public ICollection<Instructor> Subordinates { get; set; } = new List<Instructor>();

        // Points to this instructor's manager.
        public Instructor? Manager { get; set; }
    }

}
