
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Enrollments
{
    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }

        public int StudentId { get; set; }

        public string StudentName { get; set; } = null!;

        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = null!;

        public DateTime EnrollmentDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public decimal ProgressPercent { get; set; }

        public decimal? FinalGrade { get; set; }

        public EnrollmentStatus Status { get; set; }
    }
}
