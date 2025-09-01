using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.User.CreateUser;

public sealed class CreateUserCommandHandle(IUserService userService) : ICommandHandle<CreateUserCommand, UserDto> {
    public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken cancellationToken) {
        return await userService.CreateUserAsync(command.UserDto);
    }
} 