using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
        private readonly ILogger<AuthService> _logger;
        public AuthService(AppDbContext context, IOptions<JwtSettings> jwtOptions, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
            _logger = logger;
        }

        private string GenerateAccessToken(User user)
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

        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            User? user = await _context.Users
                            .Include(u => u.Student)
                            .Include(u => u.Instructor)
                            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);


            if (user is null)
            {
                _logger.LogWarning("Login failed. Email={Email}", dto.Email);

                throw new UnauthorizedException("Invalid credentials");
            }


            bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                _logger.LogWarning("Login failed (bad password). UserId={UserId}, Email={Email}", user.UserId, user.Email);

                throw new UnauthorizedException("Invalid credentials");
            }

            string accessToken = GenerateAccessToken(user);

            string refreshToken = GenerateRefreshToken();

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
            user.RefreshTokenRevokedAt = null;

            await _context.SaveChangesAsync();


            _logger.LogInformation("Login succeeded. UserId={UserId}, Role={Role}", user.UserId, user.Role);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }



        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshRequest request)
        {
            User? user = await _context.Users
                      .Include(u => u.Student)
                      .Include(u => u.Instructor)
                      .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (user is null)
                throw new BadRequestException("Invalid refresh request");

            if (user.RefreshTokenRevokedAt != null)
                throw new UnauthorizedException("Refresh token is revoked");

            if (user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token is Expired");


            if (string.IsNullOrEmpty(user.RefreshTokenHash))
                throw new BadRequestException("Invalid refresh token");


            bool refreshValid = BCrypt.Net.BCrypt.
                Verify(request.RefreshToken, user.RefreshTokenHash);


            if (!refreshValid)
            {
                _logger.LogWarning("Invalid refresh token attempt. UserId={UserId}", user.UserId);

                throw new UnauthorizedException("Invalid refresh token");
            }


            string newAccessToken = GenerateAccessToken(user);

            string newRefreshToken = GenerateRefreshToken();

            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
            user.RefreshTokenRevokedAt = null;

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Email = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };

        }


        public async Task LogoutAsync(LogoutRequest request)
        {
            User? user = await _context.Users
                     .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
                return;

            if (string.IsNullOrEmpty(user.RefreshTokenHash))
                return;

            bool refreshValid =
                BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash);

            if (!refreshValid)
                return;

            user.RefreshTokenRevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();


            _logger.LogInformation("Logout succeeded. UserId={UserId}", user.UserId);
        }

    }
}
