
namespace TrainingCenter.Application.DTOs.Authentication
{
    public class LogoutRequest
    {
        public string Email { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
