using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrainingCenter.Application.Services.Auth;

namespace TrainingCenter.API.Authorization
{
    public class StudentOwnerOrAdminHandler : AuthorizationHandler<StudentOwnerOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        StudentOwnerOrAdminRequirement requirement, int studentId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Ownership check
            var studentIdClaim = context.User.FindFirstValue(CustomClaimTypes.StudentId);

            if (int.TryParse(studentIdClaim, out int authenticatedStudentId) &&
                authenticatedStudentId == studentId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }



}
