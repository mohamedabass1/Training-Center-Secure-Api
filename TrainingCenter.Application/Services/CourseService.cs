
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Application.DTOs.Courses;
using TrainingCenter.Application.DTOs.Enrollments;
using TrainingCenter.Application.Exceptions;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Infrastructure.Context;

namespace TrainingCenter.Application.Services
{

    public class CourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }


        // ============================================
        //             Private Helpers
        // ============================================
        private CourseDto ToDto(Course course)
        {
            return new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Code = course.Code,
                Description = course.Description,
                Price = course.Price,
                Level = course.Level,
                Status = course.Status,
                DurationHours = course.DurationHours,
                CreatedAt = course.CreatedAt,
                PublishedAt = course.PublishedAt,
                InstructorId = course.InstructorId
            };
        }

        private async Task<bool> IsCodeExists(string code, int? excludedCourseId = null)
        {
            return await _context.Courses
                .AnyAsync(c =>
                    c.Code.ToLower() == code.ToLower() &&
                    c.CourseId != excludedCourseId);
        }
        private async Task<bool> IsInstructorExists(int instructorId)
        {
            return await _context.Instructors
                 .AnyAsync(i => i.InstructorId == instructorId);
        }


        private async Task<List<CourseDto>> GetByStatusAsync(CourseStatus status)
        {
            return await _context.Courses
                .AsNoTracking()
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CourseId)
                .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Code = c.Code,
                    Description = c.Description,
                    Price = c.Price,
                    Level = c.Level,
                    Status = c.Status,
                    DurationHours = c.DurationHours,
                    CreatedAt = c.CreatedAt,
                    PublishedAt = c.PublishedAt,
                    InstructorId = c.InstructorId
                })
                .ToListAsync();
        }

        private async Task<List<CourseDto>> GetByLevelAsync(CourseLevel level)
        {
            return await _context.Courses
                .AsNoTracking()
                .Where(c => c.Level == level)
                .OrderByDescending(c => c.CourseId)
                .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Code = c.Code,
                    Description = c.Description,
                    Price = c.Price,
                    Level = c.Level,
                    Status = c.Status,
                    DurationHours = c.DurationHours,
                    CreatedAt = c.CreatedAt,
                    PublishedAt = c.PublishedAt,
                    InstructorId = c.InstructorId
                })
                .ToListAsync();
        }


        // ============================================
        //              CRUD Operations
        // ============================================

        public async Task<List<CourseDto>> GetAllAsync()
        {
            return await _context.Courses
                .AsNoTracking()
                .OrderByDescending(c => c.CourseId)
                .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Code = c.Code,
                    Description = c.Description,
                    Price = c.Price,
                    Level = c.Level,
                    Status = c.Status,
                    DurationHours = c.DurationHours,
                    CreatedAt = c.CreatedAt,
                    PublishedAt = c.PublishedAt,
                    InstructorId = c.InstructorId
                })
                .ToListAsync();
        }

        public async Task<CourseDto> GetByIdAsync(int courseId)
        {
            Course? course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course is null)
                throw new NotFoundException($"Course with id {courseId} not found.");



            return ToDto(course);
        }
        public async Task<CourseDto> GetByCodeAsync(string code)
        {
            Course? course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code);

            if (course is null)
                throw new NotFoundException($"Course with code {code} not found.");



            return ToDto(course);
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
        {
            // Check instructor 
            bool isInstructorExists = await IsInstructorExists(dto.InstructorId);

            if (!isInstructorExists)
                throw new NotFoundException($"Instructor with id {dto.InstructorId} not found");


            // Check course code 
            bool isCodeExists = await IsCodeExists(dto.Code);

            if (isCodeExists)
                throw new ConflictException("Course code already exists.");


            Course course = new Course
            {
                Title = dto.Title,
                Code = dto.Code,
                Description = dto.Description,
                Price = dto.Price,
                Level = dto.Level,
                Status = CourseStatus.Draft,
                DurationHours = dto.DurationHours,
                InstructorId = dto.InstructorId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return ToDto(course);

        }

        public async Task<CourseDto> UpdateAsync(int courseId, UpdateCourseDto dto)
        {
            Course? course = await _context.Courses
                .FindAsync(courseId);

            if (course is null)
                throw new NotFoundException($"Course with id {courseId} not found.");


            // Check instructor 
            bool isInstructorExists = await IsInstructorExists(dto.InstructorId);

            if (!isInstructorExists)
                throw new NotFoundException($"Instructor with id {dto.InstructorId} not found");


            // Check course code 
            bool isCodeExists = await IsCodeExists(dto.Code, courseId);

            if (isCodeExists)
                throw new ConflictException("Course code already exists.");


            course.Title = dto.Title;
            course.Code = dto.Code;
            course.Description = dto.Description;
            course.Price = dto.Price;
            course.Level = dto.Level;
            course.DurationHours = dto.DurationHours;
            course.InstructorId = dto.InstructorId;

            await _context.SaveChangesAsync();

            return ToDto(course);

        }

        public async Task DeleteAsync(int courseId)
        {
            Course? course = await _context.Courses.FindAsync(courseId);

            if (course is null)
                throw new NotFoundException($"Course with id {courseId} not found.");


            bool hasEnrollments = await _context.Enrollments
                 .AnyAsync(e => e.CourseId == courseId);

            if (hasEnrollments)
                throw new BadRequestException("Cannot delete course with assigned enrollments.");

            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
        }



        // ============================================
        //          Status Operations
        // ============================================

        private async Task UpdateCourseStatusAsync(int courseId, CourseStatus status)
        {
            Course? course = await _context.Courses
              .FindAsync(courseId);

            if (course is null)
                throw new NotFoundException($"Course with id {courseId} not found.");

            if (course.Status == status)
                throw new ConflictException($"Course already {status}");


            course.Status = status;
            if (status == CourseStatus.Published)
                course.PublishedAt = DateTime.UtcNow;

            else if (status == CourseStatus.Draft)
                course.PublishedAt = null;

            await _context.SaveChangesAsync();
        }

        public async Task PublishAsync(int courseId)
        {
            await UpdateCourseStatusAsync(courseId, CourseStatus.Published);
        }

        public async Task ArchiveAsync(int courseId)
        {
            await UpdateCourseStatusAsync(courseId, CourseStatus.Archived);
        }

        public async Task UnpublishAsync(int courseId)
        {
            await UpdateCourseStatusAsync(courseId, CourseStatus.Draft);
        }


        public async Task<List<CourseDto>> GetPublishedCoursesAsync()
        {
            return await GetByStatusAsync(CourseStatus.Published);
        }

        public async Task<List<CourseDto>> GetDraftCoursesAsync()
        {
            return await GetByStatusAsync(CourseStatus.Draft);
        }

        public async Task<List<CourseDto>> GetArchivedCoursesAsync()
        {
            return await GetByStatusAsync(CourseStatus.Archived);
        }




        public async Task<List<CourseDto>> GetBeginnerCoursesAsync()
        {
            return await GetByLevelAsync(CourseLevel.Beginner);
        }

        public async Task<List<CourseDto>> GetIntermediateCoursesAsync()
        {
            return await GetByLevelAsync(CourseLevel.Intermediate);
        }

        public async Task<List<CourseDto>> GetAdvancedCoursesAsync()
        {
            return await GetByLevelAsync(CourseLevel.Advanced);
        }


        // ============================================
        //          Instructor Operations
        // ============================================
        public async Task<List<CourseDto>> GetInstructorCoursesAsync(int instructorId)
        {
            bool isInstructorExists = await IsInstructorExists(instructorId);

            if (!isInstructorExists)
                throw new NotFoundException($"Instructor with id {instructorId} not found");


            return await _context.Courses
                .AsNoTracking()
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CourseId)
                .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Code = c.Code,
                    Description = c.Description,
                    Price = c.Price,
                    Level = c.Level,
                    Status = c.Status,
                    DurationHours = c.DurationHours,
                    CreatedAt = c.CreatedAt,
                    PublishedAt = c.PublishedAt,
                    InstructorId = c.InstructorId
                })
                .ToListAsync();
        }

        public async Task<CourseInstructorDto> GetCourseInstructorAsync(int courseId)
        {
            CourseInstructorDto? instructor = await _context.Courses
                .AsNoTracking()
                .Where(c => c.CourseId == courseId)
                .Select(c => new CourseInstructorDto
                {
                    InstructorId = c.Instructor.InstructorId,
                    FullName = c.Instructor.FirstName + " " + c.Instructor.LastName,
                    Email = c.Instructor.User.Email,

                })
                .FirstOrDefaultAsync();

            if (instructor is null)
                throw new NotFoundException(
                    $"Course with id {courseId} not found.");

            return instructor;
        }
        public async Task ChangeInstructorAsync(int courseId, int instructorId)
        {
            Course? course = await _context.Courses
              .FindAsync(courseId);

            if (course is null)
                throw new NotFoundException($"Course with id {courseId} not found.");


            // Check instructor 
            bool isInstructorExists = await IsInstructorExists(instructorId);

            if (!isInstructorExists)
                throw new NotFoundException($"Instructor with id {instructorId} not found");

            if (course.InstructorId == instructorId)
                throw new ConflictException("Instructor already assigned to this course.");

            course.InstructorId = instructorId;

            await _context.SaveChangesAsync();

        }

        public async Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId)
        {
            bool courseExists = await _context.Courses
                .AsNoTracking()
                .AnyAsync(c => c.CourseId == courseId);

            if (!courseExists)
                throw new NotFoundException(
                    $"Course with id {courseId} not found.");


            return await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
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

        public async Task<bool> IsCourseOwnedByInstructorAsync(int courseId, int instructorId)
        {
            return await _context.Courses
                .AnyAsync(c =>
                    c.CourseId == courseId &&
                    c.InstructorId == instructorId);
        }
    }
}
