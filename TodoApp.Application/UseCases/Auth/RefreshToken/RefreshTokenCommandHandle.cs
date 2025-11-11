using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.Common.Result;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Auth.RefreshToken
{
    public sealed class RefreshTokenCommandHandle(IAuthService authService) : ICommandHandle<RefreshTokenCommand, Result<LoginResponseDto>>
    {
        public async Task<Result<LoginResponseDto>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            return await authService.RefreshTokenAsync(command.RefreshTokenRequest);
        }
    }
}
