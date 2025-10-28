using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstraction
{
    public interface ITokenProvider
    {
        Task<string> GenerateJwtToken(object userObj);
        RefreshToken GenerateRefreshToken();
        bool IsRefreshTokenValid(RefreshToken? refreshToken);
    }
}