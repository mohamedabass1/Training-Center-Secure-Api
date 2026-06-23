

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrainingCenter.Application.DTOs.Courses;
using TrainingCenter.Application.DTOs.Instructors;
using TrainingCenter.Application.Exceptions;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Infrastructure.Context;

namespace TrainingCenter.Application.Services
{
    public class InstructorService
    {
        private readonly AppDbContext _context;
        public InstructorService(AppDbContext context)
        {
            _context = context;
        }

        private static readonly Expression<Func<Instructor, InstructorDto>>
                                InstructorSelector = i => new InstructorDto
                                {
                                    InstructorId = i.InstructorId,
                                    FullName = i.FirstName + " " + i.LastName,
                                    Email = i.User.Email,
                                    HireDate = i.HireDate,
                                    Salary = i.Salary,
                                    IsActive = i.IsActive,
                                    ManagerId = i.ManagerId
                                };
        private InstructorDto ToDto(Instructor instructor)
        {
            return new InstructorDto
            {
                InstructorId = instructor.InstructorId,
                FullName = $"{instructor.FirstName} {instructor.LastName}",
                Email = instructor.User.Email,
                HireDate = instructor.HireDate,
                Salary = instructor.Salary,
                IsActive = instructor.IsActive,
                ManagerId = instructor?.ManagerId,
            };

        }

        private async Task<bool> IsManagerExists(int? ManagerId)
        {
            if (!ManagerId.HasValue || ManagerId <= 0)
                return false;

            return await _context.Instructors
                  .AnyAsync(i => i.InstructorId == ManagerId);
        }

        // ============================================
        //              CRUD Operations
        // ============================================
        public async Task<InstructorDto> CreateAsync(CreateInstructorDto dto)
        {

            bool emailExists = await _context.Users
               .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
                throw new ConflictException("Email already exists.");


            if (dto.ManagerId.HasValue)
            {
                bool managerExists = await IsManagerExists(dto.ManagerId);

                if (!managerExists)
                    throw new NotFoundException("Manager not found.");
            }

            User user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Instructor
            };


            Instructor instructor = new Instructor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                HireDate = dto.HireDate,
                Salary = dto.Salary,
                ManagerId = dto.ManagerId,
                IsActive = dto.IsActive,
                User = user

            };

            _context.Instructors.Add(instructor);

            await _context.SaveChangesAsync();

            return ToDto(instructor);
        }

        public async Task<List<InstructorDto>> GetAllAsync()
        {
            return await _context.Instructors
                .AsNoTracking()
                .OrderByDescending(i => i.InstructorId)
                .Select(InstructorSelector)
                .ToListAsync();
        }


        public async Task<InstructorDto> UpdateAsync(int instructorId, UpdateInstructorDto dto)
        {
            Instructor? instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");


            if (dto.ManagerId == instructorId)
                throw new BadRequestException("Instructor cannot be his own manager.");

            if (dto.ManagerId.HasValue)
            {

                bool managerExists = await IsManagerExists(dto.ManagerId);

                if (!managerExists)
                    throw new NotFoundException("Manager not found.");
            }

            // if the user dose not update the password
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                instructor.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }



            instructor.FirstName = dto.FirstName;
            instructor.LastName = dto.LastName;
            instructor.Salary = dto.Salary;
            instructor.ManagerId = dto.ManagerId;
            instructor.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return ToDto(instructor);

        }

        public async Task<InstructorDto> GetByIdAsync(int instructorId)
        {
            InstructorDto? instructor = await _context.Instructors
                .AsNoTracking()
                .Where(i => i.InstructorId == instructorId)
                .Select(InstructorSelector)
                .FirstOrDefaultAsync();

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");


            return instructor;
        }

        public async Task DeleteAsync(int instructorId)
        {
            Instructor? instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");


            bool hasCourses = await _context.Courses
                 .AnyAsync(c => c.InstructorId == instructorId);

            if (hasCourses)
                throw new BadRequestException("Cannot delete instructor with assigned courses.");

            _context.Users.Remove(instructor.User);
            _context.Instructors.Remove(instructor);

            await _context.SaveChangesAsync();
        }

        public async Task<List<CourseDto>> GetInstructorCoursesAsync(int instructorId)
        {
            var exists = await _context.Instructors
                .AsNoTracking()
                .AnyAsync(i => i.InstructorId == instructorId);

            if (!exists)
                throw new NotFoundException($"Instructor with id {instructorId} not found.");

            var courses = await _context.Courses
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

            return courses;
        }

        // ============================================
        //             Status Operations
        // ============================================
        public async Task<List<InstructorDto>> GetInactiveInstructorsAsync()
        {

            return await _context.Instructors
                .AsNoTracking()
                .Where(i => !i.IsActive)
                .OrderByDescending(i => i.InstructorId)
                .Select(InstructorSelector)
                .ToListAsync();
        }
        public async Task<List<InstructorDto>> GetActiveInstructorsAsync()
        {
            return await _context.Instructors
                .AsNoTracking()
                .Where(i => i.IsActive)
                .OrderByDescending(i => i.InstructorId)
                .Select(InstructorSelector)
                .ToListAsync();
        }

        private async Task SetInstructorStatusAsync(int instructorId, bool isActive)
        {
            var instructor = await _context.Instructors
              .FindAsync(instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");


            instructor.IsActive = isActive;

            await _context.SaveChangesAsync();
        }
        public async Task ActivateAsync(int instructorId)
        {
            await SetInstructorStatusAsync(instructorId, true);
        }

        public async Task DeactivateAsync(int instructorId)
        {
            await SetInstructorStatusAsync(instructorId, false);
        }

        // ============================================
        //          Manager Operations
        // ============================================

        public async Task AssignManagerAsync(int instructorId, AssignManagerDto assignManagerDto)
        {

            int? managerId = assignManagerDto.ManagerId;

            if (instructorId == managerId)
            {
                throw new BadRequestException("Instructor cannot be his own manager.");
            }


            if (managerId.HasValue)
            {
                bool managerExists = await IsManagerExists(managerId);

                if (!managerExists)
                    throw new NotFoundException("Manager not found.");
            }


            Instructor? instructor = await _context.Instructors
                            .FindAsync(instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");


            instructor.ManagerId = managerId;

            await _context.SaveChangesAsync();
        }

        public async Task<InstructorDto> GetManagerAsync(int instructorId)
        {
            Instructor? instructor = await _context.Instructors
                                .Include(i => i.Manager)
                                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);

            if (instructor is null)
                throw new NotFoundException($"Instructor with ID {instructorId} was not found.");

            if (instructor.Manager is null)
                throw new NotFoundException($"Instructor with ID {instructorId} does not have a manager.");

            return ToDto(instructor.Manager);
        }

        public async Task<List<InstructorDto>> GetSubordinatesAsync(int managerId)
        {
            bool managerExists = await _context.Instructors.
                        AnyAsync(i => i.InstructorId == managerId);

            if (!managerExists)
                throw new NotFoundException("Manager not found.");


            return await _context.Instructors
                            .Where(i => i.ManagerId == managerId)
                            .Select(InstructorSelector)
                            .ToListAsync();
        }

    }
}
