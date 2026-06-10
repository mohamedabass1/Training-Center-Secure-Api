

using Microsoft.EntityFrameworkCore;
using TrainingCenter.Application.DTOs.Instructors;
using TrainingCenter.Application.Exceptions;
using TrainingCenter.Domain.Entities;
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

        private InstructorDto ToDto(Instructor instructor)
        {
            return new InstructorDto
            {
                InstructorId = instructor.InstructorId,
                FullName = $"{instructor.FirstName} {instructor.LastName}",
                Email = instructor.Email,
                HireDate = instructor.HireDate,
                Salary = instructor.Salary,
                IsActive = instructor.IsActive,
                ManagerId = instructor.ManagerId,
            };

        }

        private async Task<bool> IsEmailExists(string email, int? excludedInstructorId = null)
        {
            return await _context.Instructors
                .AnyAsync(i =>
                    i.Email.ToLower() == email.ToLower() &&
                    i.InstructorId != excludedInstructorId);
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


            bool emailExists = await IsEmailExists(dto.Email);

            if (emailExists)
                throw new ConflictException("Email already exists.");


            if (dto.ManagerId.HasValue)
            {

                bool managerExists = await IsManagerExists(dto.ManagerId);

                if (!managerExists)
                    throw new NotFoundException("Manager not found.");
            }


            Instructor instructor = new Instructor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                HireDate = dto.HireDate,
                Salary = dto.Salary,
                ManagerId = dto.ManagerId,
                IsActive = dto.IsActive
            };


            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return ToDto(instructor);

        }

        public async Task<List<InstructorDto>> GetAllAsync()
        {
            return await _context.Instructors
                .AsNoTracking()
                .OrderBy(i => i.InstructorId)
                .Select(i => new InstructorDto
                {
                    InstructorId = i.InstructorId,
                    FullName = i.FirstName + " " + i.LastName,
                    Email = i.Email,
                    HireDate = i.HireDate,
                    Salary = i.Salary,
                    IsActive = i.IsActive,
                    ManagerId = i.ManagerId,
                })
                .ToListAsync();
        }


        public async Task<InstructorDto> UpdateAsync(int instructorId, UpdateInstructorDto dto)
        {
            Instructor? instructor = await _context.Instructors.FindAsync(instructorId);

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

            bool emailExists = await IsEmailExists(dto.Email, instructorId);

            if (emailExists)
                throw new ConflictException("Email already exists.");


            instructor.FirstName = dto.FirstName;
            instructor.LastName = dto.LastName;
            instructor.Email = dto.Email;
            instructor.Salary = dto.Salary;
            instructor.ManagerId = dto.ManagerId;
            instructor.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return ToDto(instructor);

        }

        public async Task<InstructorDto> GetByIdAsync(int instructorId)
        {
            Instructor? instructor = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");


            return ToDto(instructor);
        }

        public async Task DeleteAsync(int instructorId)
        {
            Instructor? instructor = await _context.Instructors
                .FindAsync(instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");


            bool hasCourses = await _context.Courses
                 .AnyAsync(c => c.InstructorId == instructorId);

            if (hasCourses)
                throw new BadRequestException("Cannot delete instructor with assigned courses.");

            _context.Instructors.Remove(instructor);

            await _context.SaveChangesAsync();
        }

        // ============================================
        //             Status Operations
        // ============================================
        public async Task<List<InstructorDto>> GetInactiveInstructorsAsync()
        {
            return await _context.Instructors
                .AsNoTracking()
                .Where(i => !i.IsActive)
                .OrderBy(i => i.InstructorId)
                .Select(i => new InstructorDto
                {
                    InstructorId = i.InstructorId,
                    FullName = i.FirstName + " " + i.LastName,
                    Email = i.Email,
                    HireDate = i.HireDate,
                    Salary = i.Salary,
                    IsActive = i.IsActive,
                    ManagerId = i.ManagerId,
                })
                .ToListAsync();
        }
        public async Task<List<InstructorDto>> GetActiveInstructorsAsync()
        {
            return await _context.Instructors
                .AsNoTracking()
                .Where(i => i.IsActive)
                .OrderBy(i => i.InstructorId)
                .Select(i => new InstructorDto
                {
                    InstructorId = i.InstructorId,
                    FullName = i.FirstName + " " + i.LastName,
                    Email = i.Email,
                    HireDate = i.HireDate,
                    Salary = i.Salary,
                    IsActive = i.IsActive,
                    ManagerId = i.ManagerId,
                })
                .ToListAsync();
        }

        public async Task ActivateAsync(int instructorId)
        {
            var instructor = await _context.Instructors
                .FindAsync(instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");



            instructor.IsActive = true;

            await _context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int instructorId)
        {
            var instructor = await _context.Instructors
                .FindAsync(instructorId);

            if (instructor is null)
                throw new NotFoundException("Instructor not found.");



            instructor.IsActive = false;

            await _context.SaveChangesAsync();
        }

        // ============================================
        //          Manager Operations
        // ============================================

        public async Task AssignManagerAsync(int instructorId, int? managerId)
        {

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
                            .Select(i => new InstructorDto
                            {
                                InstructorId = i.InstructorId,
                                FullName = i.FirstName + " " + i.LastName,
                                Email = i.Email,
                                HireDate = i.HireDate,
                                Salary = i.Salary,
                                IsActive = i.IsActive,
                                ManagerId = i.ManagerId,
                            })
                         .ToListAsync();
        }

    }
}
