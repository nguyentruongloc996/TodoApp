using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Auth.GoogleLogin;
public sealed record GoogleLoginCommand(GoogleAuthRequestDto GoogleAuthRequest) : ICommand<LoginResponseDto> {} 