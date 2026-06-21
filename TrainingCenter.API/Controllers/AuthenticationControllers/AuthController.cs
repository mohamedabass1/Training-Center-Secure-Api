using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrainingCenter.API.Common;
using TrainingCenter.Application.DTOs.Authentication;
using TrainingCenter.Application.Services.Auth;

namespace TrainingCenter.API.Controllers.AuthenticationControllers
{
    [Route("api/auth")]
    [ApiController]
    [Tags("01. Authentication")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        [EndpointSummary("Authenticates a user and returns a JWT token.")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
        {
            AuthResponseDto response = await _authService.LoginAsync(dto);

            return Ok(response);
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        [EndpointSummary("Generates a new access token using a valid refresh token.")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshRequest dto)
        {
            AuthResponseDto response = await _authService.RefreshTokenAsync(dto);

            return Ok(response);
        }


        [HttpPost("logout")]
        [EndpointSummary("Revokes the current refresh token and logs the user out.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(LogoutRequest dto)
        {
            await _authService.LogoutAsync(dto);

            return NoContent();
        }
    }
}
