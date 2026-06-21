using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrainingCenter.Application.Services;
using TrainingCenter.Application.Services.Auth;

namespace TrainingCenter.API.Authorization
{
    public class EnrollmentCourseInstructorOrAdminHandler : AuthorizationHandler<
          EnrollmentCourseInstructorOrAdminRequirement, int>
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentCourseInstructorOrAdminHandler(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EnrollmentCourseInstructorOrAdminRequirement requirement, int enrollmentId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            var instructorIdClaim =
                context.User.FindFirstValue(CustomClaimTypes.InstructorId);

            if (!int.TryParse(instructorIdClaim, out var instructorId))
            {
                return;
            }

            var isInstructor =
                await _enrollmentService.IsEnrollmentBelongsToInstructorAsync(enrollmentId, instructorId);

            if (isInstructor)
            {
                context.Succeed(requirement);
            }
        }



    }
}
