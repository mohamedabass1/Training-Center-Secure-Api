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
    [Tags("04. Instructor Managers")]
    public class InstructorManagersController : ControllerBase
    {

        private readonly InstructorService _instructorService;

        public InstructorManagersController(InstructorService instructorService)
        {
            _instructorService = instructorService;
        }


        // --- Owner or Admin
        [HttpGet("{id:int:min(1)}/manager")]
        [EndpointSummary("Retrieves the manager assigned to an instructor.")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InstructorDto>> GetManager(int id,
            [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(base.User, id,
              "InstructorOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403


            var manager = await _instructorService.GetManagerAsync(id);

            return Ok(manager);
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int:min(1)}/manager")]
        [EndpointSummary("Assigns or removes a manager for an instructor.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AssignManager(int id, [FromBody] AssignManagerDto assignManagerDto)
        {
            await _instructorService.AssignManagerAsync(id, assignManagerDto);

            return NoContent();
        }



        // --- Owner or Admin
        [HttpGet("{id:int:min(1)}/subordinates")]
        [EndpointSummary("Retrieves all instructors managed by the specified instructor.")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<InstructorDto>>> GetSubordinates(int id,
            [FromServices] IAuthorizationService authorizationService)
        {

            var authResult = await authorizationService.AuthorizeAsync(base.User, id,
              "InstructorOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403

            var subordinates = await _instructorService.GetSubordinatesAsync(id);

            return Ok(subordinates);
        }
    }

}