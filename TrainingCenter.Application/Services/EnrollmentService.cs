using Microsoft.EntityFrameworkCore;
using TrainingCenter.Application.DTOs.Enrollments;
using TrainingCenter.Application.Exceptions;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Infrastructure.Context;

namespace TrainingCenter.Application.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _context;

        public EnrollmentService(AppDbContext context)
        {
            _context = context;
        }


        // ============================================
        //             Private Helpers
        // ============================================

        private EnrollmentDto ToDto(Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate,
                CompletionDate = enrollment.CompletionDate,
                ProgressPercent = enrollment.ProgressPercent,
                FinalGrade = enrollment.FinalGrade,
                Status = enrollment.Status
            };
        }

        private async Task<bool> IsStudentExists(int studentId)
        {
            return await _context.Students
                .AnyAsync(s => s.StudentId == studentId);
        }

        private async Task<bool> IsCourseExists(int courseId)
        {
            return await _context.Courses
                .AnyAsync(c => c.CourseId == courseId);
        }

        private async Task<bool> IsEnrollmentExists(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e =>
                    e.StudentId == studentId &&
                    e.CourseId == courseId);
        }


        // ============================================
        //           Enrollment Operations
        // ============================================

        public async Task<List<EnrollmentDto>> GetAllAsync()
        {
            return await _context.Enrollments
                .AsNoTracking()
                .OrderByDescending(e => e.EnrollmentId)
                .Select(e => new EnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    StudentName = e.Student.FirstName + " " + e.Student.LastName,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    EnrollmentDate = e.EnrollmentDate,
                    CompletionDate = e.CompletionDate,
                    ProgressPercent = e.ProgressPercent,
                    FinalGrade = e.FinalGrade,
                    Status = e.Status
                })
                .ToListAsync();
        }

        public async Task<EnrollmentDto> GetByIdAsync(int enrollmentId)
        {
            EnrollmentDto? enrollment = await _context.Enrollments
               .AsNoTracking()
               .Where(e => e.EnrollmentId == enrollmentId)
               .Select(e => new EnrollmentDto
               {
                   EnrollmentId = e.EnrollmentId,
                   StudentId = e.StudentId,
                   StudentName = e.Student.FirstName + " " + e.Student.LastName,
                   CourseId = e.CourseId,
                   CourseTitle = e.Course.Title,
                   EnrollmentDate = e.EnrollmentDate,
                   CompletionDate = e.CompletionDate,
                   ProgressPercent = e.ProgressPercent,
                   FinalGrade = e.FinalGrade,
                   Status = e.Status
               }).FirstOrDefaultAsync();


            if (enrollment is null)
                throw new NotFoundException(
                    $"Enrollment with id {enrollmentId} not found.");

            return enrollment;
        }

        public async Task<EnrollmentDto> EnrollStudentAsync(int studentId, int courseId)
        {
            bool enrollmentExists = await IsEnrollmentExists(
             studentId,
             courseId);

            if (enrollmentExists)
                throw new ConflictException(
                    "Student is already enrolled in this course.");



            bool studentExists = await IsStudentExists(studentId);

            if (!studentExists)
                throw new NotFoundException(
                    $"Student with id {studentId} not found.");


            bool courseExists = await IsCourseExists(courseId);

            if (!courseExists)
                throw new NotFoundException(
                    $"Course with id {courseId} not found.");




            Enrollment enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = EnrollmentStatus.Active
            };

            _context.Enrollments.Add(enrollment);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(enrollment.EnrollmentId);
        }

        public async Task DeleteAsync(int enrollmentId)
        {
            Enrollment? enrollment = await _context.Enrollments
                .FindAsync(enrollmentId);

            if (enrollment is null)
                throw new NotFoundException(
                    $"Enrollment with id {enrollmentId} not found.");


            _context.Enrollments.Remove(enrollment);

            await _context.SaveChangesAsync();
        }

        public async Task CompleteAsync(int enrollmentId, decimal finalGrade)
        {

            Enrollment? enrollment = await _context.Enrollments
                .FindAsync(enrollmentId);


            if (enrollment is null)
                throw new NotFoundException(
                    $"Enrollment with id {enrollmentId} not found.");



            if (enrollment.Status == EnrollmentStatus.Completed)
                throw new ConflictException("Enrollment already completed");


            if (enrollment.Status == EnrollmentStatus.Dropped)
                throw new BadRequestException("Dropped enrollment cannot be completed.");


            if (finalGrade < 0 || finalGrade > 100)
                throw new BadRequestException("Final grade must be between 0 and 100.");

            enrollment.FinalGrade = finalGrade;
            enrollment.ProgressPercent = 100;
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletionDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

        }

        public async Task DropAsync(int enrollmentId)
        {
            Enrollment? enrollment = await _context.Enrollments
                .FindAsync(enrollmentId);

            if (enrollment is null)
                throw new NotFoundException($"Enrollment with id {enrollmentId} not found.");


            if (enrollment.Status == EnrollmentStatus.Dropped)
                throw new ConflictException("Enrollment already dropped.");


            if (enrollment.Status == EnrollmentStatus.Completed)
                throw new BadRequestException("Completed enrollment cannot be dropped.");


            enrollment.Status = EnrollmentStatus.Dropped;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateProgressAsync(int enrollmentId, decimal progress)
        {
            Enrollment? enrollment = await _context.Enrollments
                .FindAsync(enrollmentId);

            if (enrollment is null)
                throw new NotFoundException(
                    $"Enrollment with id {enrollmentId} not found.");


            if (enrollment.Status == EnrollmentStatus.Completed)
                throw new BadRequestException(
                    "Completed enrollment progress cannot be updated.");


            if (enrollment.Status == EnrollmentStatus.Dropped)
                throw new BadRequestException(
                    "Dropped enrollment progress cannot be updated.");


            if (progress < 0 || progress >= 100)
                throw new BadRequestException(
                    "Progress must be between 0 and less than 100.");


            enrollment.ProgressPercent = progress;

            await _context.SaveChangesAsync();
        }


        private async Task<List<EnrollmentDto>> GetEnrollmentsByStatusAsync(EnrollmentStatus status)
        {
            return await _context.Enrollments
               .AsNoTracking()
               .Where(e => e.Status == status)
               .OrderByDescending(e => e.EnrollmentId)
               .Select(e => new EnrollmentDto
               {
                   EnrollmentId = e.EnrollmentId,
                   StudentId = e.StudentId,
                   StudentName = e.Student.FirstName + " " + e.Student.LastName,
                   CourseId = e.CourseId,
                   CourseTitle = e.Course.Title,
                   EnrollmentDate = e.EnrollmentDate,
                   CompletionDate = e.CompletionDate,
                   ProgressPercent = e.ProgressPercent,
                   FinalGrade = e.FinalGrade,
                   Status = e.Status
               })
               .ToListAsync();
        }
        public async Task<List<EnrollmentDto>> GetActiveEnrollmentsAsync()
        {
            return await GetEnrollmentsByStatusAsync(EnrollmentStatus.Active);
        }
        public async Task<List<EnrollmentDto>> GetCompletedEnrollmentsAsync()
        {
            return await GetEnrollmentsByStatusAsync(EnrollmentStatus.Completed);
        }

        public async Task<List<EnrollmentDto>> GetDroppedEnrollmentsAsync()
        {
            return await GetEnrollmentsByStatusAsync(EnrollmentStatus.Dropped);
        }

        /// <summary>
        /// Asynchronously retrieves enrollment statistics, including total, active, completed, and dropped enrollments.
        /// </summary>
        /// <returns>An EnrollmentStatisticsDto containing the aggregated enrollment statistics.</returns>
        public async Task<EnrollmentStatisticsDto> GetEnrollmentStatisticsAsync()
        {
            var statistics = await _context.Enrollments
                                .GroupBy(e => 1)
                                .Select(g => new EnrollmentStatisticsDto
                                {
                                    TotalEnrollments = g.Count(),

                                    ActiveEnrollments =
                                        g.Count(e => e.Status == EnrollmentStatus.Active),

                                    CompletedEnrollments =
                                        g.Count(e => e.Status == EnrollmentStatus.Completed),

                                    DroppedEnrollments =
                                        g.Count(e => e.Status == EnrollmentStatus.Dropped)
                                })
                                .FirstAsync();

            return new EnrollmentStatisticsDto
            {
                TotalEnrollments = statistics.TotalEnrollments,
                ActiveEnrollments = statistics.ActiveEnrollments,
                CompletedEnrollments = statistics.CompletedEnrollments,
                DroppedEnrollments = statistics.DroppedEnrollments,
            };
        }

        public async Task<bool> IsEnrollmentBelongsToStudentAsync(int enrollmentId, int studentId)
        {
            return await _context.Enrollments.
                AnyAsync(e => e.EnrollmentId == enrollmentId && e.StudentId == studentId);
        }
        public async Task<bool> IsEnrollmentBelongsToInstructorAsync(int enrollmentId, int InstructorId)
        {
            return await _context.Enrollments.
                AnyAsync(e => e.EnrollmentId == enrollmentId && e.Course.InstructorId == InstructorId);
        }

    }
}