
namespace TrainingCenter.Application.DTOs.Authentication
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
