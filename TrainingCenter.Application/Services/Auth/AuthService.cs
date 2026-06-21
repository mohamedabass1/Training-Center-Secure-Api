

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrainingCenter.Application.DTOs.Authentication;
using TrainingCenter.Application.Exceptions;
using TrainingCenter.Application.Settings;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Enums;
using TrainingCenter.Infrastructure.Context;
namespace TrainingCenter.Application.Services.Auth
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AppDbContext context, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
        }

        private string GenerateToken(User user)
        {
            List<Claim> claims =
            [
                // Unique identifier for the user
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),


                new Claim(ClaimTypes.Email, user.Email),


                // Role (Student or Instructor or Admin) used later for authorization
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ];

            if (user.Role == UserRole.Instructor && user.Instructor != null)
            {
                claims.Add(new Claim(CustomClaimTypes.InstructorId,
                    user.Instructor.InstructorId.ToString()));

            }

            if (user.Role == UserRole.Student && user.Student != null)
            {
                claims.Add(new Claim(CustomClaimTypes.StudentId,
                    user.Student.StudentId.ToString()));

            }



            var key = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(_jwtSettings.Key));

            // This specifies the algorithm used to sign the token.
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            DateTime expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: credentials
                    );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            User? user = await _context.Users
                            .AsNoTracking()
                            .Include(u => u.Student)
                            .Include(u => u.Instructor)
                            .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null)
                throw new UnauthorizedException("Invalid credentials");

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedException("Invalid credentials");


            string token = GenerateToken(user);


            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }
    }
}
