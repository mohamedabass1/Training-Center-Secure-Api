using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Instructors;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers
{
    [ApiController]
    [Route("api/Instructors")]
    public class InstructorsController : ControllerBase
    {
        private readonly InstructorService _instructorService;

        public InstructorsController(InstructorService instructorService)
        {
            _instructorService = instructorService;
        }


        // ============================================
        //              CRUD Operations
        // ============================================

        [HttpGet]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Retrieves all instructors.")]
        public async Task<ActionResult<List<InstructorDto>>> GetAll()
        {
            var instructors = await _instructorService.GetAllAsync();

            return Ok(instructors);
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Retrieves an instructor by ID.")]
        public async Task<ActionResult<InstructorDto>> GetById(int id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);

            return Ok(instructor);
        }


        [HttpPost]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Creates a new instructor.")]
        public async Task<ActionResult<InstructorDto>> Create(CreateInstructorDto dto)
        {
            var instructor = await _instructorService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = instructor.InstructorId },
                instructor);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]


        [EndpointSummary("Updates an existing instructor.")]
        public async Task<ActionResult<InstructorDto>> Update(int id, UpdateInstructorDto dto)
        {
            var instructor = await _instructorService.UpdateAsync(id, dto);

            return Ok(instructor);
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Deletes an instructor by ID.")]
        public async Task<IActionResult> Delete(int id)
        {
            await _instructorService.DeleteAsync(id);

            return NoContent();
        }


        // ============================================
        //             Status Operations
        // ============================================

        [HttpGet("active")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Retrieves all active instructors.")]
        public async Task<ActionResult<List<InstructorDto>>> GetActive()
        {
            var instructors = await _instructorService.GetActiveInstructorsAsync();

            return Ok(instructors);
        }


        [HttpGet("inactive")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Retrieves all inactive instructors.")]
        public async Task<ActionResult<List<InstructorDto>>> GetInactive()
        {
            var instructors = await _instructorService.GetInactiveInstructorsAsync();

            return Ok(instructors);
        }


        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Activates an instructor.")]
        public async Task<IActionResult> Activate(int id)
        {
            await _instructorService.ActivateAsync(id);

            return NoContent();
        }


        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Deactivates an instructor.")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _instructorService.DeactivateAsync(id);

            return NoContent();
        }


        // ============================================
        //          Manager Operations
        // ============================================

        [HttpGet("{id:int}/manager")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Retrieves the manager assigned to an instructor.")]
        public async Task<ActionResult<InstructorDto>> GetManager(int id)
        {
            var manager = await _instructorService.GetManagerAsync(id);

            return Ok(manager);
        }


        [HttpPatch("{id:int}/manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Assigns or removes a manager for an instructor.")]
        public async Task<IActionResult> AssignManager(int id, [FromBody] int? managerId)
        {
            await _instructorService.AssignManagerAsync(id, managerId);

            return NoContent();
        }

        [HttpGet("{id:int}/subordinates")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        [EndpointSummary("Retrieves all instructors managed by the specified instructor.")]
        public async Task<ActionResult<List<InstructorDto>>> GetSubordinates(int id)
        {
            var subordinates = await _instructorService.GetSubordinatesAsync(id);

            return Ok(subordinates);
        }
    }
}