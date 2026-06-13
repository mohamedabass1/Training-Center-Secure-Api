using Microsoft.AspNetCore.Mvc;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Enrollments;
using TrainingCenter.Application.DTOs.Students;
using TrainingCenter.Application.Services;

namespace TrainingCenter.API.Controllers.StudentControllers
{
    [Route("api/students")]
    [Tags("04. Students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }


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



        [HttpGet("{id:int:min(1)}/enrollments")]
        [EndpointSummary("Retrieves all enrollments for a specific student.")]
        [ProducesResponseType(typeof(List<EnrollmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EnrollmentDto>>> GetEnrollments(int id)
        {
            var enrollments = await _studentService
                .GetStudentEnrollmentsAsync(id);

            return Ok(enrollments);
        }

    }
}
