using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.Common.Result;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Auth.Register
{
    public class RegisterCommandHandle(IAuthService authService) : ICommandHandle<RegisterCommand, Result<RegisterRequestDto>>
    {
        public async Task<Result<RegisterRequestDto>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            return await authService.RegisterAsync(command);
        }
    }
}