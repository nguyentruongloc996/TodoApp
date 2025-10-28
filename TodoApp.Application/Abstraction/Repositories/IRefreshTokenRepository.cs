using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstraction.Repositories
{
    public interface IRefreshTokenRepository
    {
        void Add(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}