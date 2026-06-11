using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.StudentProfiles;
using TrainingCenter.Application.DTOs.Students;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers
{
    [Route("api/Students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }


        // ============================================
        //              CRUD Operations
        // ============================================


        [HttpGet]
        [EndpointSummary("Retrieves all students.")]
        [ProducesResponseType(typeof(List<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<StudentDto>>> GetAll()
        {
            var students = await _studentService.GetAllAsync();

            return Ok(students);
        }

        [HttpGet("{id:int:min(1)}")]
        [EndpointSummary("Retrieves a student by ID.")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<StudentDto>> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);

            return Ok(student);
        }


        [HttpPost]
        [EndpointSummary("Creates a new student.")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<StudentDto>> Create(CreateStudentDto dto)
        {
            StudentDto student = await _studentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = student.StudentId },
                student);
        }

        [HttpPut("{id:int:min(1)}")]
        [EndpointSummary("Updates an existing student.")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> Update(int id, UpdateStudentDto dto)
        {
            var student = await _studentService.UpdateAsync(id, dto);

            return Ok(student);
        }


        [HttpDelete("{id:int:min(1)}")]
        [EndpointSummary("Deletes a student by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Delete(int id)
        {
            await _studentService.DeleteAsync(id);

            return NoContent();
        }


        // ============================================
        //          Status Operations
        // ============================================


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



        // ============================================
        //           Profile Operations
        // ============================================

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

        [HttpPost("{id:int:min(1)}/profile")]
        [EndpointSummary("Creates a new student profile.")]
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


        [HttpDelete("{id:int:min(1)}/profile")]
        [EndpointSummary("Deletes a student profile by ID.")]
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
