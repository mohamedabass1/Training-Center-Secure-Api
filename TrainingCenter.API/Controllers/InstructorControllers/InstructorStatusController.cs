using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Instructors;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.InstructorControllers
{
    [Authorize]
    [ApiController]
    [Route("api/instructors")]
    [Tags("03. Instructor Status")]
    public class InstructorStatusController : ControllerBase
    {

        private readonly InstructorService _instructorService;

        public InstructorStatusController(InstructorService instructorService)
        {
            _instructorService = instructorService;
        }



        [HttpGet("active")]
        [EndpointSummary("Retrieves all active instructors.")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<InstructorDto>>> GetActive()
        {
            var instructors = await _instructorService.GetActiveInstructorsAsync();

            return Ok(instructors);
        }


        [HttpGet("inactive")]
        [EndpointSummary("Retrieves all inactive instructors.")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<InstructorDto>>> GetInactive()
        {
            var instructors = await _instructorService.GetInactiveInstructorsAsync();

            return Ok(instructors);
        }


        [HttpPatch("{id:int:min(1)}/activate")]
        [EndpointSummary("Activates an instructor.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Activate(int id)
        {
            await _instructorService.ActivateAsync(id);

            return NoContent();
        }


        [HttpPatch("{id:int:min(1)}/deactivate")]
        [EndpointSummary("Deactivates an instructor.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Deactivate(int id)
        {
            await _instructorService.DeactivateAsync(id);

            return NoContent();
        }


    }

}