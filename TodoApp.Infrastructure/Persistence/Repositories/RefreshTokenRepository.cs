using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Infrastructure.Persistence.Auth.Models;

namespace TodoApp.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext _context) : Interfaces.IRefreshTokenRepository
    {
        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);

            return existingToken;
        }
    }
}
