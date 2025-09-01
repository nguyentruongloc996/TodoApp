using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Register;

namespace TodoApp.Application.Abstraction.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> ValidateTokenAsync(string token);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<RegisterRequestDto> RegisterAsync(RegisterCommand command);
    }
} 