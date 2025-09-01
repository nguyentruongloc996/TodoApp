using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandle(IAuthService authService) : ICommandHandle<LoginCommand, LoginResponseDto> {
    public async Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken) {
        return await authService.LoginAsync(command.LoginRequest);
    }
} 