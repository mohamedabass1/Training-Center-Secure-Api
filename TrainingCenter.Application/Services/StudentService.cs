using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrainingCenter.Application.DTOs.Enrollments;
using TrainingCenter.Application.DTOs.StudentProfiles;
using TrainingCenter.Application.DTOs.Students;
using TrainingCenter.Application.Exceptions;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Infrastructure.Context;

namespace TrainingCenter.Application.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;
        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        /// --------------------
        private StudentDto ToDto(Student student)
        {
            return new StudentDto
            {
                StudentId = student.StudentId,
                FullName = $"{student.FirstName} {student.LastName}",
                Email = student.User.Email,
                DateOfBirth = student.DateOfBirth,
                Status = student.Status,
                PhoneNumber = student.PhoneNumber,
                RegisteredAt = student.RegisteredAt
            };
        }

        private static readonly Expression<Func<Student, StudentDto>>
            StudentSelector = s => new StudentDto
            {
                StudentId = s.StudentId,
                FullName = s.FirstName + " " + s.LastName,
                Email = s.User.Email,
                DateOfBirth = s.DateOfBirth,
                Status = s.Status,
                PhoneNumber = s.PhoneNumber,
                RegisteredAt = s.RegisteredAt
            };
        private async Task<List<StudentDto>> GetStudentsByStatusAsync(StudentStatus status)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.Status == status)
                .OrderBy(s => s.StudentId)
                .Select(StudentSelector)
                .ToListAsync();
        }


        private async Task UpdateStudentStatusAsync(int studentId, StudentStatus status)
        {
            var student = await _context.Students
                .FindAsync(studentId);

            if (student is null)
                throw new NotFoundException($"Student with id {studentId} not found");



            student.Status = status;

            await _context.SaveChangesAsync();
        }

        // ============================================
        //              CRUD Operations
        // ============================================

        public async Task<List<StudentDto>> GetAllAsync()
        {
            return await _context.Students
                .AsNoTracking()
                .OrderByDescending(s => s.StudentId)
                .Select(StudentSelector)
                .ToListAsync();
        }

        public async Task<StudentDto> GetByIdAsync(int studentId)
        {
            StudentDto? student = await _context.Students
                .AsNoTracking()
                .Where(s => s.StudentId == studentId)
                .Select(StudentSelector)
                .FirstOrDefaultAsync();

            if (student is null)
                throw new NotFoundException($"Student with id {studentId} not found");


            return student;
        }

        public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
        {
            bool emailExists = await _context.Users
                 .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
                throw new ConflictException("Email already exists.");

            User user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Student
            };

            Student student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Status = dto.Status,
                PhoneNumber = dto.PhoneNumber,
                User = user
            };

            _context.Students.Add(student);

            await _context.SaveChangesAsync();

            return ToDto(student);

        }

        public async Task<StudentDto> UpdateAsync(int studentId, UpdateStudentDto dto)
        {
            Student? student = await _context.Students
                                         .Include(s => s.User)
                                         .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student is null)
                throw new NotFoundException($"Student with id {studentId} not found");


            // if the user dose not update the password
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                student.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Status = dto.Status;
            student.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return ToDto(student); ;
        }

        public async Task DeleteAsync(int studentId)
        {
            Student? student = await _context.Students
                                        .Include(s => s.User)
                                        .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student is null)
                throw new NotFoundException($"Student with id {studentId} not found");


            bool hasEnrollments = await _context.Enrollments
                 .AnyAsync(e => e.StudentId == studentId);

            if (hasEnrollments)
                throw new BadRequestException("Cannot delete student with assigned enrollments.");

            _context.Users.Remove(student.User);
            _context.Students.Remove(student);

            await _context.SaveChangesAsync();
        }

        // ============================================
        //          Status Operations
        // ============================================


        public async Task<List<StudentDto>> GetActiveStudentsAsync()
        {
            return await GetStudentsByStatusAsync(StudentStatus.Active);
        }

        public async Task<List<StudentDto>> GetSuspendedStudentsAsync()
        {
            return await GetStudentsByStatusAsync(StudentStatus.Suspended);
        }

        public async Task<List<StudentDto>> GetGraduatedStudentsAsync()
        {
            return await GetStudentsByStatusAsync(StudentStatus.Graduated);
        }


        public async Task ActivateAsync(int studentId)
        {
            await UpdateStudentStatusAsync(studentId, StudentStatus.Active);
        }

        public async Task SuspendAsync(int studentId)
        {
            await UpdateStudentStatusAsync(studentId, StudentStatus.Suspended);
        }

        public async Task GraduateAsync(int studentId)
        {
            await UpdateStudentStatusAsync(studentId, StudentStatus.Graduated);
        }




        // ============================================
        //           Profile Operations
        // ============================================


        public async Task<StudentProfileDto> GetProfileAsync(int studentId)
        {
            var profile = await _context.StudentProfiles
                .AsNoTracking()
                .Where(p => p.StudentId == studentId)
                .Select(p => new StudentProfileDto
                {
                    StudentId = p.StudentId,
                    Address = p.Address,
                    City = p.City,
                    Country = p.Country,
                    Bio = p.Bio,
                    LinkedInUrl = p.LinkedInUrl
                })
                .FirstOrDefaultAsync();

            if (profile is null)
                throw new NotFoundException("Student profile not found.");

            return profile;
        }

        public async Task<StudentProfileDto> CreateProfileAsync(int studentId, CreateStudentProfileDto dto)
        {
            bool studentExists = await _context.Students
                  .AnyAsync(s => s.StudentId == studentId);

            if (!studentExists)
                throw new NotFoundException($"Student with id {studentId} not found");

            bool hasProfile = await _context.StudentProfiles
                     .AnyAsync(p => p.StudentId == studentId);

            if (hasProfile)
                throw new ConflictException("Student already has a profile");


            var profile = new StudentProfile
            {
                StudentId = studentId,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                Bio = dto.Bio,
                LinkedInUrl = dto.LinkedInUrl
            };

            _context.StudentProfiles.Add(profile);

            await _context.SaveChangesAsync();


            return new StudentProfileDto
            {
                StudentId = studentId,
                Address = profile.Address,
                City = profile.City,
                Country = profile.Country,
                Bio = profile.Bio,
                LinkedInUrl = profile.LinkedInUrl
            };

        }

        public async Task<StudentProfileDto> UpdateProfileAsync(int studentId, UpdateStudentProfileDto dto)
        {

            StudentProfile? profile = await _context.StudentProfiles
                     .FindAsync(studentId);

            if (profile is null)
                throw new NotFoundException("Student profile not found.");

            profile.Address = dto.Address;
            profile.City = dto.City;
            profile.Country = dto.Country;
            profile.Bio = dto.Bio;
            profile.LinkedInUrl = dto.LinkedInUrl;

            await _context.SaveChangesAsync();


            return new StudentProfileDto
            {
                StudentId = studentId,
                Address = profile.Address,
                City = profile.City,
                Country = profile.Country,
                Bio = profile.Bio,
                LinkedInUrl = profile.LinkedInUrl
            };

        }


        public async Task DeleteProfileAsync(int studentId)
        {
            StudentProfile? profile = await _context.StudentProfiles
                .FindAsync(studentId);

            if (profile is null)
                throw new NotFoundException($"Student Profile not found");

            _context.StudentProfiles.Remove(profile);

            await _context.SaveChangesAsync();
        }


        //////
        ///
        public async Task<List<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId)
        {
            bool studentExists = await _context.Students
                .AsNoTracking()
                .AnyAsync(s => s.StudentId == studentId);

            if (!studentExists)
                throw new NotFoundException(
                    $"Student with id {studentId} not found.");


            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
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
                }
                 ).ToListAsync();

            return enrollments;


        }

    }
}
