using Microsoft.AspNetCore.Authorization;

namespace TrainingCenter.API.Authorization
{
    public class EnrollmentOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
