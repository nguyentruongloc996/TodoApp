using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Application.Abstraction.Repositories;

namespace TodoApp.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext _context) : IRefreshTokenRepository
    {
        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var existingToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            return existingToken;
        }
    }
}
