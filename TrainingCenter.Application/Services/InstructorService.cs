

using Microsoft.EntityFrameworkCore;
using TrainingCenter.Application.DTOs.Instructors;
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
                    i.Email == email &&
                    i.InstructorId != excludedInstructorId);
        }
        private async Task<bool> IsManagerExists(int? ManagerId)
        {
            if (!ManagerId.HasValue || ManagerId <= 0)
                return false;

            return await _context.Instructors
                  .AnyAsync(i => i.InstructorId == ManagerId);
        }
        public async Task<InstructorDto> CreateAsync(CreateInstructorDto dto)
        {


            bool emailExists = await IsEmailExists(dto.Email);

            if (emailExists)
                throw new InvalidOperationException("Email already exists.");


            if (dto.ManagerId.HasValue)
            {

                bool managerExists = await IsManagerExists(dto.ManagerId);

                if (!managerExists)
                    throw new InvalidOperationException("Manager not found.");
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

        public async Task<List<InstructorDto>> GetActiveAsync()
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


        public async Task<InstructorDto> UpdateAsync(int instructorId, UpdateInstructorDto dto)
        {
            Instructor? instructor = await _context.Instructors.FindAsync(instructorId);

            if (instructor is null)
                throw new InvalidOperationException("Instructor not found.");


            if (dto.ManagerId == instructorId)
                throw new InvalidOperationException(
                    "Instructor cannot be his own manager.");

            if (dto.ManagerId.HasValue)
            {

                bool managerExists = await IsManagerExists(dto.ManagerId);

                if (!managerExists)
                    throw new InvalidOperationException("Manager not found.");
            }

            bool emailExists = await IsEmailExists(dto.Email, instructorId);

            if (emailExists)
                throw new InvalidOperationException("Email already exists.");


            instructor.FirstName = dto.FirstName;
            instructor.LastName = dto.LastName;
            instructor.Email = dto.Email;
            instructor.Salary = dto.Salary;
            instructor.ManagerId = dto.ManagerId;
            instructor.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return ToDto(instructor);

        }

        public async Task<InstructorDto?> GetByIdAsync(int instructorId)
        {
            Instructor? instructor = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);

            if (instructor is null)
                return null;


            return ToDto(instructor);
        }

        public async Task<bool> DeleteAsync(int instructorId)
        {
            Instructor? instructor = await _context.Instructors
                .FindAsync(instructorId);

            if (instructor is null)
                return false;

            bool hasCourses = await _context.Courses
                 .AnyAsync(c => c.InstructorId == instructorId);

            if (hasCourses)
                throw new InvalidOperationException(
                    "Cannot delete instructor with assigned courses.");

            _context.Instructors.Remove(instructor);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> ActivateAsync(int instructorId)
        {
            var instructor = await _context.Instructors
                .FindAsync(instructorId);

            if (instructor is null)
                throw new InvalidOperationException(
                    "Instructor not found.");

            if (instructor.IsActive)
                return true;

            instructor.IsActive = true;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeactivateAsync(int instructorId)
        {
            var instructor = await _context.Instructors
                .FindAsync(instructorId);

            if (instructor is null)
                throw new InvalidOperationException(
                    "Instructor not found.");

            if (!instructor.IsActive)
                return true;

            instructor.IsActive = false;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
