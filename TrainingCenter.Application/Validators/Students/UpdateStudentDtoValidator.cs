using FluentValidation;
using TrainingCenter.Application.DTOs.Students;

namespace TrainingCenter.Application.Validators.Students
{
    public class UpdateStudentDtoValidator
    : AbstractValidator<UpdateStudentDto>
    {
        public UpdateStudentDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage(
                    "Status must be Active, Suspended, or Graduated.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(30)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
