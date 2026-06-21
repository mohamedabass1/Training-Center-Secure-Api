
namespace TrainingCenter.Application.DTOs.Authentication
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
