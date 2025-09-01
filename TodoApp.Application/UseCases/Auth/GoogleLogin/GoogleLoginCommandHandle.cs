using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.Auth.GoogleLogin;

public sealed class GoogleLoginCommandHandle(IAuthService authService) : ICommandHandle<GoogleLoginCommand, LoginResponseDto> {
    public async Task<LoginResponseDto> Handle(GoogleLoginCommand command, CancellationToken cancellationToken) {
        return await authService.GoogleLoginAsync(command.GoogleAuthRequest);
    }
} 