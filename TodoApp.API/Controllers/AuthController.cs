using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Login;
using TodoApp.Application.UseCases.Auth.GoogleLogin;
using TodoApp.Application.UseCases.Auth.Register;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandle<LoginCommand, LoginResponseDto> _loginCommandHandle;
        private readonly ICommandHandle<GoogleLoginCommand, LoginResponseDto> _googleLoginCommandHandle;
        private readonly ICommandHandle<RegisterCommand, RegisterRequestDto> _registerCommandHandle;

        public AuthController(
            ICommandHandle<LoginCommand, LoginResponseDto> loginCommandHandle,
            ICommandHandle<GoogleLoginCommand, LoginResponseDto> googleLoginCommandHandle,
            ICommandHandle<RegisterCommand, RegisterRequestDto> registerCommandHandle)
        {
            _loginCommandHandle = loginCommandHandle;
            _googleLoginCommandHandle = googleLoginCommandHandle;
            _registerCommandHandle = registerCommandHandle;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request);
            var result = await _loginCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequestDto request, CancellationToken cancellationToken)
        {
            var command = new GoogleLoginCommand(request);
            var result = await _googleLoginCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request);
            var result = await _registerCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            // TODO: Implement refresh token command
            return Ok(new { message = "Refresh token endpoint - to be implemented" });
        }
    }
} 