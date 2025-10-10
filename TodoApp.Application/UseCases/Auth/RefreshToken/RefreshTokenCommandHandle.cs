using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Auth.RefreshToken
{
    public sealed class RefreshTokenCommandHandle(IAuthService authService) : ICommandHandle <RefreshTokenCommand, LoginResponseDto>
    {
        public async Task<LoginResponseDto> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            return await authService.RefreshTokenAsync(command.RefreshTokenRequest);
        }
    }
}
