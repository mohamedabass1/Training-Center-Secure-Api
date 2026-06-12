
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Courses
{
    public class CourseDto
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public CourseLevel Level { get; set; }
        public CourseStatus Status { get; set; }

        public int DurationHours { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public int InstructorId { get; set; }

    }

}
