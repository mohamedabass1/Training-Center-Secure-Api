using FluentValidation;
using TrainingCenter.Application.DTOs.Courses;

namespace TrainingCenter.Application.Validators.Courses
{
    public class CreateCourseDtoValidator
        : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Level)
                .IsInEnum()
                .WithMessage(
                    "Level must be Beginner, Intermediate, or Advanced.");

            RuleFor(x => x.DurationHours)
                .GreaterThan(0)
                .WithMessage(
                    "Duration hours must be greater than zero.");

            RuleFor(x => x.InstructorId)
                .GreaterThan(0)
                .WithMessage(
                    "Instructor ID must be greater than zero.");
        }
    }
}