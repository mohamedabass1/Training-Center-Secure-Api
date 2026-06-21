using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Enrollments;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.EnrollmentControllers
{
    [Authorize]
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService;
        public EnrollmentsController(EnrollmentService enrollmentService)
            => _enrollmentService = enrollmentService;


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [EndpointSummary("Retrieves all Enrollments.")]
        [ProducesResponseType(typeof(List<EnrollmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EnrollmentDto>>> GetAll()
        {
            var enrollments = await _enrollmentService.GetAllAsync();

            return Ok(enrollments);
        }



        // Owner or Admin
        [HttpGet("{id:int:min(1)}")]
        [EndpointSummary("Retrieves an enrollment by ID.")]
        [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EnrollmentDto>> GetById(int id,
            [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(User, id,
                "EnrollmentOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403


            var enrollment = await _enrollmentService.GetByIdAsync(id);

            return Ok(enrollment);
        }



        // Owner or Admin
        [HttpPost]
        [EndpointSummary("Creates a new enrollment.")]
        [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]

        public async Task<ActionResult<EnrollmentDto>> EnrollStudent(EnrollStudentDto dto,
               [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(User, dto.StudentId,
                "StudentOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403

            var enrollment = await _enrollmentService.EnrollStudentAsync(dto.StudentId, dto.CourseId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = enrollment.EnrollmentId },
                enrollment);
        }


        [HttpPatch("{id:int:min(1)}/complete")]
        [EndpointSummary("Marks an enrollment as completed and assigns the final grade.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Complete(int id, CompleteEnrollmentDto dto,
                 [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(User, id,
                "EnrollmentCourseInstructorOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403

            await _enrollmentService.CompleteAsync(id, dto.FinalGrade);

            return NoContent();
        }



        // Owner or Admin
        [HttpPatch("{id:int:min(1)}/drop")]
        [EndpointSummary("Drops a student from a course.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Drop(int id,
                [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(User, id,
                "EnrollmentOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403

            await _enrollmentService.DropAsync(id);

            return NoContent();
        }


        [HttpPatch("{id:int:min(1)}/progress")]
        [EndpointSummary("Updates the progress percentage of an active enrollment.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProgress(int id, UpdateEnrollmentProgressDto dto,
              [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(User, id,
                "EnrollmentCourseInstructorOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403
            await _enrollmentService.UpdateProgressAsync(id, dto.ProgressPercent);

            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("active")]
        [EndpointSummary("Retrieves all active enrollments.")]
        [ProducesResponseType(typeof(List<EnrollmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EnrollmentDto>>> GetActive()
        {
            var enrollments = await _enrollmentService.GetActiveEnrollmentsAsync();

            return Ok(enrollments);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("completed")]
        [EndpointSummary("Retrieves all completed enrollments.")]
        [ProducesResponseType(typeof(List<EnrollmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EnrollmentDto>>> GetCompleted()
        {
            var enrollments = await _enrollmentService.GetCompletedEnrollmentsAsync();

            return Ok(enrollments);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("dropped")]
        [EndpointSummary("Retrieves all dropped enrollments.")]
        [ProducesResponseType(typeof(List<EnrollmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EnrollmentDto>>> GetDropped()
        {
            var enrollments = await _enrollmentService.GetDroppedEnrollmentsAsync();

            return Ok(enrollments);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("statistics")]
        [EndpointSummary("Retrieves enrollment statistics.")]
        [ProducesResponseType(typeof(EnrollmentStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EnrollmentStatisticsDto>> GetStatistics()
        {
            var statistics =
                await _enrollmentService.GetEnrollmentStatisticsAsync();

            return Ok(statistics);
        }
    }
}
