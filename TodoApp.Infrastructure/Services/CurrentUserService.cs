using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? _httpContextAccessor.HttpContext?.User?
                    .FindFirst("sub")?.Value;

                return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public Guid? DomainUserId
        {
            get
            {
                var domainUserIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("domainUserId")?.Value;

                return Guid.TryParse(domainUserIdClaim, out var domainUserId) ? domainUserId : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}