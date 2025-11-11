using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Common.Result;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Auth.Register;

public sealed record RegisterCommand(RegisterRequestDto Request) : ICommand<Result<RegisterRequestDto>>{}