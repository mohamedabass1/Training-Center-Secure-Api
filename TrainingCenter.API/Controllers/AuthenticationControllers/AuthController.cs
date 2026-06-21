using Microsoft.AspNetCore.Mvc;
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
        [EndpointSummary("Authenticates a user and returns a JWT token.")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
        {
            AuthResponseDto response =
                await _authService.LoginAsync(dto);

            return Ok(response);
        }
    }
}
