using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.Common.Result;

namespace TodoApp.Application.UseCases.Auth.Login;

public sealed class LoginCommandHandle(IAuthService authService) : ICommandHandle<LoginCommand, Result<LoginResponseDto>> {
    public async Task<Result<LoginResponseDto>> Handle(LoginCommand command, CancellationToken cancellationToken) {
        return await authService.LoginAsync(command.LoginRequest);
    }
} 