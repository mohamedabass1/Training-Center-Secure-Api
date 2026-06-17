using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Students;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.StudentControllers
{
    [Authorize]
    [Route("api/students")]
    [Tags("06. Student Status")]
    [ApiController]
    public class StudentStatusController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentStatusController(StudentService studentService)
        {
            _studentService = studentService;
        }



        [HttpGet("active")]
        [EndpointSummary("Retrieves all active students.")]
        [ProducesResponseType(typeof(List<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<StudentDto>>> GetActive()
        {
            var students = await _studentService.GetActiveStudentsAsync();

            return Ok(students);
        }


        [HttpGet("suspended")]
        [EndpointSummary("Retrieves all suspended students.")]
        [ProducesResponseType(typeof(List<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<StudentDto>>> GetSuspended()
        {
            var students = await _studentService.GetSuspendedStudentsAsync();

            return Ok(students);
        }


        [HttpGet("graduated")]
        [EndpointSummary("Retrieves all graduated students.")]
        [ProducesResponseType(typeof(List<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<List<StudentDto>>> GetGraduated()
        {
            var students = await _studentService.GetGraduatedStudentsAsync();

            return Ok(students);
        }



        [HttpPatch("{id:int:min(1)}/activate")]
        [EndpointSummary("Activates a student.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Activate(int id)
        {
            await _studentService.ActivateAsync(id);

            return NoContent();
        }


        [HttpPatch("{id:int:min(1)}/suspend")]
        [EndpointSummary("Suspends a student.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Suspend(int id)
        {
            await _studentService.SuspendAsync(id);

            return NoContent();
        }


        [HttpPatch("{id:int:min(1)}/graduate")]
        [EndpointSummary("Graduates a student.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Graduate(int id)
        {
            await _studentService.GraduateAsync(id);

            return NoContent();
        }




    }
}
