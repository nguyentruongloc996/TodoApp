using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Common.Result;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Auth.RefreshToken
{
    public sealed record RefreshTokenCommand(RefreshTokenRequestDto RefreshTokenRequest) : ICommand<Result<LoginResponseDto>> { }
}
