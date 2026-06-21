
namespace TrainingCenter.Application.DTOs.Authentication
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
