using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Auth.Login;
public sealed record LoginCommand(LoginRequestDto LoginRequest) : ICommand<LoginResponseDto> {} 