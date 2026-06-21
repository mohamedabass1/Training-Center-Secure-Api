using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrainingCenter.Application.Services;
using TrainingCenter.Application.Services.Auth;

namespace TrainingCenter.API.Authorization
{
    public partial class EnrollmentOwnerOrAdminHandler : AuthorizationHandler<EnrollmentOwnerOrAdminRequirement, int>
    {
        private readonly EnrollmentService _EnrollmentService;
        // Inject CourseService
        public EnrollmentOwnerOrAdminHandler(EnrollmentService enrollmentService)
        {
            _EnrollmentService = enrollmentService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        EnrollmentOwnerOrAdminRequirement requirement, int enrollmentId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Ownership check
            var studentIdClaim = context.User.FindFirstValue(CustomClaimTypes.StudentId);

            if (!int.TryParse(studentIdClaim, out int authenticatedStudentId))
            {
                return;
            }

            bool isOwner = await _EnrollmentService.IsEnrollmentBelongsToStudentAsync(enrollmentId, authenticatedStudentId);

            if (isOwner)
                context.Succeed(requirement);
        }


    }
}
