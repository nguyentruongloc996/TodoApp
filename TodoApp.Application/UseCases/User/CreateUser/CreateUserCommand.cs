using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.User.CreateUser;
public sealed record CreateUserCommand(UserDto UserDto) : ICommand<UserDto> {} 