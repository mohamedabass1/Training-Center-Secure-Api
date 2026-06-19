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


        // --- Instructor or Admin

        [HttpGet("{id:int:min(1)}/manager")]
        [EndpointSummary("Retrieves the manager assigned to an instructor.")]
        [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InstructorDto>> GetManager(int id)
        {
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

        public async Task<IActionResult> AssignManager(int id, [FromBody] int? managerId)
        {
            await _instructorService.AssignManagerAsync(id, managerId);

            return NoContent();
        }



        // --- Instructor or Admin
        [HttpGet("{id:int:min(1)}/subordinates")]
        [EndpointSummary("Retrieves all instructors managed by the specified instructor.")]
        [ProducesResponseType(typeof(List<InstructorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<InstructorDto>>> GetSubordinates(int id)
        {
            var subordinates = await _instructorService.GetSubordinatesAsync(id);

            return Ok(subordinates);
        }
    }

}