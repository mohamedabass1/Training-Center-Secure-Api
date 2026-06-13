using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Courses;
using TrainingCenter.Application.DTOs.Enrollments;
using TrainingCenter.Application.DTOs.Instructors;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.CourseControllers
{
    [ApiController]
    [Route("api/courses")]
    [Tags("07. Courses")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CoursesController(CourseService courseService)
        {
            _courseService = courseService;
        }


        [HttpGet]
        [EndpointSummary("Retrieves all courses.")]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CourseDto>>> GetAll()
        {
            var courses = await _courseService.GetAllAsync();

            return Ok(courses);
        }


        [HttpGet("{id:int:min(1)}")]
        [EndpointSummary("Retrieves a course by ID.")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> GetById(int id)
        {
            var course = await _courseService.GetByIdAsync(id);

            return Ok(course);
        }


        [HttpGet("code/{code}")]
        [EndpointSummary("Retrieves a course by code.")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> GetByCode(string code)
        {
            var course = await _courseService.GetByCodeAsync(code);

            return Ok(course);
        }


        [HttpPost]
        [EndpointSummary("Creates a new course.")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> Create(CreateCourseDto dto)
        {
            var course = await _courseService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = course.CourseId },
                course);
        }


        [HttpPut("{id:int:min(1)}")]
        [EndpointSummary("Updates an existing course.")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseDto>> Update(int id, UpdateCourseDto dto)
        {
            var course = await _courseService.UpdateAsync(id, dto);

            return Ok(course);
        }


        [HttpDelete("{id:int:min(1)}")]
        [EndpointSummary("Deletes a course by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteAsync(id);

            return NoContent();
        }


        [HttpGet("{id:int:min(1)}/instructor")]
        [EndpointSummary("Retrieves the instructor assigned to a course.")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InstructorDto>> GetInstructor(int id)
        {
            var instructor = await _courseService.GetCourseInstructorAsync(id);

            return Ok(instructor);
        }


        [HttpPatch("{id:int:min(1)}/instructor")]
        [EndpointSummary("Changes the instructor assigned to a course.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeInstructor(int id, ChangeCourseInstructorDto dto)
        {
            await _courseService.ChangeInstructorAsync(id, dto.InstructorId);

            return NoContent();
        }



        [HttpGet("courses/{id:int:min(1)}/enrollments")]
        [EndpointSummary("Retrieves all enrollments for a specific course.")]
        [ProducesResponseType(typeof(List<EnrollmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EnrollmentDto>>> GetCourseEnrollments(int id)
        {
            var enrollments = await _courseService
                .GetCourseEnrollmentsAsync(id);

            return Ok(enrollments);
        }



    }
}