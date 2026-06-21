using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrainingCenter.Application.Services;
using TrainingCenter.Application.Services.Auth;

namespace TrainingCenter.API.Authorization
{
    public class CourseOwnerOrAdminHandler : AuthorizationHandler<CourseOwnerOrAdminRequirement, int>
    {
        private readonly CourseService _courseService;
        // Inject CourseService
        public CourseOwnerOrAdminHandler(CourseService courseService)
        {
            _courseService = courseService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CourseOwnerOrAdminRequirement requirement, int courseId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Ownership check
            var InstructorIdClaim = context.User.FindFirstValue(CustomClaimTypes.InstructorId);

            if (!int.TryParse(InstructorIdClaim, out int authenticatedInstructorId))
            {
                return;
            }

            bool isOwner = await _courseService.IsCourseOwnedByInstructorAsync(courseId, authenticatedInstructorId);

            if (isOwner)
                context.Succeed(requirement);
        }



    }
}
