using FluentValidation;
using TrainingCenter.Application.DTOs.Instructors;

namespace TrainingCenter.Application.Validators.Instructors
{
    /// <summary>
    /// Validates CreateInstructorDto properties to ensure they meet required business rules and data integrity
    /// constraints.
    /// </summary>
    public class CreateInstructorDtoValidator
         : AbstractValidator<CreateInstructorDto>
    {
        public CreateInstructorDtoValidator()
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

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.HireDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Hire date cannot be in the future.");

            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ManagerId)
                .GreaterThan(0)
                .When(x => x.ManagerId.HasValue)
                .WithMessage("ManagerId must be greater than zero.");
        }


    }
}
