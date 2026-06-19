using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Courses;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.CourseControllers
{
    [ApiController]
    [Route("api/courses")]
    [Tags("09. Course Filters")]
    public class CourseFiltersController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseFiltersController(CourseService courseService)
        {
            _courseService = courseService;
        }


        [Authorize(Roles = "Admin,Instructor")]

        [HttpPatch("{id:int:min(1)}/publish")]
        [EndpointSummary("Publishes a course.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Publish(int id)
        {
            await _courseService.PublishAsync(id);
            return NoContent();
        }


        [Authorize(Roles = "Admin,Instructor")]

        [HttpPatch("{id:int:min(1)}/archive")]
        [EndpointSummary("Archives a course.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Archive(int id)
        {
            await _courseService.ArchiveAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin,Instructor")]

        [HttpPatch("{id:int:min(1)}/unpublish")]
        [EndpointSummary("Moves a course back to draft status.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _courseService.UnpublishAsync(id);
            return NoContent();
        }


        [AllowAnonymous]
        [HttpGet("published")]
        [EndpointSummary("Retrieves all published courses.")]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CourseDto>>> GetPublished()
            => Ok(await _courseService.GetPublishedCoursesAsync());


        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("draft")]
        [EndpointSummary("Retrieves all draft courses.")]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CourseDto>>> GetDraft()
            => Ok(await _courseService.GetDraftCoursesAsync());


        [Authorize(Roles = "Admin,Instructor")]

        [HttpGet("archived")]
        [EndpointSummary("Retrieves all archived courses.")]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CourseDto>>> GetArchived()
            => Ok(await _courseService.GetArchivedCoursesAsync());




        /// Level oration
        [AllowAnonymous]
        [HttpGet("beginner")]
        [EndpointSummary("Retrieves all beginner courses.")]
        public async Task<ActionResult<List<CourseDto>>> GetBeginner()
        => Ok(await _courseService.GetBeginnerCoursesAsync());


        [AllowAnonymous]
        [HttpGet("intermediate")]
        [EndpointSummary("Retrieves all intermediate courses.")]
        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<CourseDto>>> GetIntermediate()
            => Ok(await _courseService.GetIntermediateCoursesAsync());


        [AllowAnonymous]
        [HttpGet("advanced")]
        [EndpointSummary("Retrieves all advanced courses.")]

        [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CourseDto>>> GetAdvanced()
            => Ok(await _courseService.GetAdvancedCoursesAsync());


    }
}