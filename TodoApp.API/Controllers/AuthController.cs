using Microsoft.AspNetCore.Mvc;
using TodoApp.API.Extensions;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Login;
using TodoApp.Application.UseCases.Auth.GoogleLogin;
using TodoApp.Application.UseCases.Auth.Register;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Application.UseCases.Auth.RefreshToken;
using TodoApp.Application.Common.Result;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandle<LoginCommand, Result<LoginResponseDto>> _loginCommandHandle;
        private readonly ICommandHandle<GoogleLoginCommand, LoginResponseDto> _googleLoginCommandHandle;
        private readonly ICommandHandle<RegisterCommand, Result<RegisterRequestDto>> _registerCommandHandle;
        private readonly ICommandHandle<RefreshTokenCommand, Result<LoginResponseDto>> _refreshTokenCommandHandle;
        public AuthController(
            ICommandHandle<LoginCommand, Result<LoginResponseDto>> loginCommandHandle,
            ICommandHandle<GoogleLoginCommand, LoginResponseDto> googleLoginCommandHandle,
            ICommandHandle<RegisterCommand, Result<RegisterRequestDto>> registerCommandHandle,
            ICommandHandle<RefreshTokenCommand, Result<LoginResponseDto>> refreshTokenCommandHandle)
        {
            _loginCommandHandle = loginCommandHandle;
            _googleLoginCommandHandle = googleLoginCommandHandle;
            _registerCommandHandle = registerCommandHandle;
            _refreshTokenCommandHandle = refreshTokenCommandHandle;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request);
            var result = await _loginCommandHandle.Handle(command, cancellationToken);
            return this.FromResult(result);
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
            return result.IsSuccess ? Created(nameof(Register), result.Value) : this.FromResult(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            // TODO: Implement refresh token command
            var command = new RefreshTokenCommand(request);
            var result = await _refreshTokenCommandHandle.Handle(command, cancellationToken);

            return this.FromResult(result);
        }
    }
} 