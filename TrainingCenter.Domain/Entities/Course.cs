using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Domain.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        // Difficulty level of the course.
        // Beginner
        // Intermediate
        // Advanced
        public CourseLevel Level { get; set; }

        public int DurationHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }


        public CourseStatus Status { get; set; }

        // Foreign Key
        // Links this course to its instructor.
        public int InstructorId { get; set; }


        // One course can have many enrollments.
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // Each course belongs to one instructor.
        public Instructor Instructor { get; set; } = null!;

    }

}
