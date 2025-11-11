using TodoApp.Application.Common.Result;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Register;

namespace TodoApp.Application.Abstraction.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request);
        Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<Result<RegisterRequestDto>> RegisterAsync(RegisterCommand command);
    }
} 