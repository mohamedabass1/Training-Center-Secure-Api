using FluentValidation;
using TrainingCenter.Application.DTOs.StudentProfiles;

namespace TrainingCenter.Application.Validators.StudentProfiles
{
    public class UpdateStudentProfileDtoValidator
       : AbstractValidator<UpdateStudentProfileDto>
    {
        public UpdateStudentProfileDtoValidator()
        {

            RuleFor(x => x.Address)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.City)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.City));

            RuleFor(x => x.Country)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Country));

            RuleFor(x => x.Bio)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));

            RuleFor(x => x.LinkedInUrl)
                .MaximumLength(200)
                .Must(url =>
                    Uri.TryCreate(url, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl))
                .WithMessage("LinkedIn URL must be a valid URL.");
        }

    }
}
