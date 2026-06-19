using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Courses;
using TrainingCenter.Application.DTOs.Instructors;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.InstructorControllers
{
    [Authorize]
    [ApiController]
    [Route("api/instructors")]
    [Tags("02. Instructors")]
    public class InstructorsController : ControllerBase
    {
        private readonly InstructorService _instructorService;

        public InstructorsController(InstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [EndpointSummary("Retrieves all instructors.")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<InstructorDto>>> GetAll()
        {
            var instructors = await _instructorService.GetAllAsync();

            return Ok(instructors);
        }


        // --- Owner or Admin
        [HttpGet("{id:int:min(1)}")]
        [EndpointSummary("Retrieves an instructor by ID.")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InstructorDto>> GetById(int id)
        {

            var instructor = await _instructorService.GetByIdAsync(id);

            return Ok(instructor);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        [EndpointSummary("Creates a new instructor.")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InstructorDto>> Create(CreateInstructorDto dto)
        {
            var instructor = await _instructorService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = instructor.InstructorId },
                instructor);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int:min(1)}")]
        [EndpointSummary("Updates an existing instructor.")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InstructorDto>> Update(int id, UpdateInstructorDto dto)
        {
            var instructor = await _instructorService.UpdateAsync(id, dto);

            return Ok(instructor);
        }



        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int:min(1)}")]
        [EndpointSummary("Deletes an instructor by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _instructorService.DeleteAsync(id);

            return NoContent();
        }



        // --- Instructor or Admin
        [HttpGet("{id:int:min(1)}/courses")]
        [EndpointSummary("Retrieves all courses assigned to an instructor.")]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CourseDto>>> GetCourses(int id)
        {
            var courses = await _instructorService.GetInstructorCoursesAsync(id);

            return Ok(courses);
        }
    }

}