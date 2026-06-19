using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.StudentProfiles;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.StudentControllers
{
    [Authorize]
    [Route("api/students")]
    [Tags("07. Student Profiles")]
    [ApiController]
    public class StudentProfilesController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentProfilesController(StudentService studentService)
        {
            _studentService = studentService;
        }

        // --- Owner or Admin
        [HttpGet("{id:int:min(1)}/profile")]
        [EndpointSummary("Retrieves the profile of a student.")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentProfileDto>> GetProfile(int id)
        {
            var profile = await _studentService.GetProfileAsync(id);

            return Ok(profile);
        }


        // --- Owner or Admin
        [HttpPost("{id:int:min(1)}/profile")]
        [EndpointSummary("Creates a profile for a student.")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentProfileDto>> CreateProfile(int id, CreateStudentProfileDto dto)
        {
            var profile = await _studentService.CreateProfileAsync(id, dto);

            return CreatedAtAction(
                          nameof(GetProfile),
                          new { id = profile.StudentId },
                          profile);
        }


        // --- Owner or Admin
        [HttpPut("{id:int:min(1)}/profile")]
        [EndpointSummary("Updates a student profile.")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentProfileDto>> UpdateProfile(int id, UpdateStudentProfileDto dto)
        {
            var profile = await _studentService.UpdateProfileAsync(id, dto);

            return Ok(profile);
        }


        // --- Owner or Admin
        [HttpDelete("{id:int:min(1)}/profile")]
        [EndpointSummary("Deletes the profile of a student.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteProfile(int id)
        {
            await _studentService.DeleteProfileAsync(id);

            return NoContent();
        }

    }
}
