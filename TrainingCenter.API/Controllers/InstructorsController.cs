using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Application.DTOs.Instructors;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorsController : ControllerBase
    {
        private readonly InstructorService _instructorService;

        public InstructorsController(InstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InstructorDto>>> GetAll()
        {
            var instructors = await _instructorService.GetAllAsync();

            return Ok(instructors);
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<InstructorDto>>> GetActive()
        {
            var instructors = await _instructorService.GetActiveAsync();

            return Ok(instructors);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InstructorDto>> GetById(int id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);

            if (instructor is null)
                return NotFound();

            return Ok(instructor);
        }

        [HttpPost]
        public async Task<ActionResult<InstructorDto>> Create(CreateInstructorDto dto)
        {
            var instructor = await _instructorService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = instructor.InstructorId },
                instructor);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<InstructorDto>> Update(int id, UpdateInstructorDto dto)
        {
            var instructor = await _instructorService.UpdateAsync(id, dto);

            return Ok(instructor);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _instructorService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            bool result = await _instructorService.ActivateAsync(id);

            return Ok(new
            {
                Success = result,
                Message = "Instructor activated successfully."
            });
        }

        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            bool result = await _instructorService.DeactivateAsync(id);

            return Ok(new
            {
                Success = result,
                Message = "Instructor deactivated successfully."
            });
        }
    }
}