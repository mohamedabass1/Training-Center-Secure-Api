using FluentValidation;
using TrainingCenter.Application.DTOs.Students;

namespace TrainingCenter.Application.Validators.Students
{
    public class CreateStudentDtoValidator
         : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty();


            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);


            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .LessThan(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Date of birth must be in the past.");


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
