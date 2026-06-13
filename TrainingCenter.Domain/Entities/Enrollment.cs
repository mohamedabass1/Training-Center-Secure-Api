using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Domain.Entities
{
    /// <summary>
    /// Represents a student's enrollment in a course, including progress and grading information.
    /// </summary>
    /// <remarks>Includes enrollment and completion dates, progress percentage, final grade, and navigation
    /// properties to the associated student and course.</remarks>
    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        // Foreign Key → Student
        // Indicates which student owns this enrollment.
        public int StudentId { get; set; }

        // Foreign Key → Course
        // Indicates which course the student joined.
        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public decimal ProgressPercent { get; set; }
        public decimal? FinalGrade { get; set; }

        public EnrollmentStatus Status { get; set; }


        // This enrollment belongs to one course.
        public Course Course { get; set; } = null!;

        // This enrollment belongs to one student.
        public Student Student { get; set; } = null!;
    }

}
