
using TrainingCenter.Domain.Enums;

namespace TrainingCenter.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }

        public Student? Student { get; set; }

        public Instructor? Instructor { get; set; }
    }
}
