using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Register;

namespace TodoApp.Application.Abstraction.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<RegisterRequestDto> RegisterAsync(RegisterCommand command);
    }
} 