using FluentValidation;
using TrainingCenter.Application.DTOs.Instructors;

namespace TrainingCenter.Application.Validators.Instructors
{
    /// <summary>
    /// Validates the properties of an UpdateInstructorDto object to ensure they meet specified criteria.
    /// </summary>
    public class UpdateInstructorDtoValidator
       : AbstractValidator<UpdateInstructorDto>
    {
        public UpdateInstructorDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ManagerId)
                .GreaterThan(0)
                .When(x => x.ManagerId.HasValue)
                .WithMessage("ManagerId must be greater than zero.");


        }
    }
}
