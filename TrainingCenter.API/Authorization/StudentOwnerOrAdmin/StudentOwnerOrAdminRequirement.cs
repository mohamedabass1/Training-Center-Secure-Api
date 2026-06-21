using Microsoft.AspNetCore.Authorization;

namespace TrainingCenter.API.Authorization
{
    // Student Owner OR Admin can access the specific resource.
    public class StudentOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }



}
