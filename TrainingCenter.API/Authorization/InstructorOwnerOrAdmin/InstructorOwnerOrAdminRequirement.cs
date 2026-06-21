using Microsoft.AspNetCore.Authorization;

namespace TrainingCenter.API.Authorization
{
    // Instructor Owner OR Admin can access the specific resource.
    public class InstructorOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }

}
