
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Enrollments
{
    public class UpdateEnrollmentDto
    {
        public decimal ProgressPercent { get; set; }

        public decimal? FinalGrade { get; set; }

        public EnrollmentStatus Status { get; set; }

        public DateTime? CompletionDate { get; set; }
    }
}
