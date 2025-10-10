using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Infrastructure.Persistence.Auth.Models;

namespace TodoApp.Infrastructure.Persistence.Auth.Interfaces
{
    public interface ITokenProvider
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
        public RefreshToken GenerateRefreshToken();
        bool IsRefreshTokenValid(RefreshToken? refreshToken);
    }
}
