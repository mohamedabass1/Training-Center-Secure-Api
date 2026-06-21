using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrainingCenter.Application.Services.Auth;

namespace TrainingCenter.API.Authorization
{
    public class InstructorOwnerOrAdminHandler : AuthorizationHandler<InstructorOwnerOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        InstructorOwnerOrAdminRequirement requirement, int instructorId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Ownership check
            var InstructorIdClaim = context.User.FindFirstValue(CustomClaimTypes.InstructorId);

            if (int.TryParse(InstructorIdClaim, out int authenticatedInstructorId) &&
                authenticatedInstructorId == instructorId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }


    }
}
