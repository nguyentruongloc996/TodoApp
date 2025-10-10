using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        void Add(Auth.Models.RefreshToken refreshToken);
        Task<Auth.Models.RefreshToken?> GetByTokenAsync(string token);
    }
}
