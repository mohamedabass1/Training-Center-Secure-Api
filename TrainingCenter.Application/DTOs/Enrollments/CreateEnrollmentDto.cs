
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Application.DTOs.Enrollments
{
    public class CreateEnrollmentDto
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public EnrollmentStatus Status { get; set; }
    }
}
