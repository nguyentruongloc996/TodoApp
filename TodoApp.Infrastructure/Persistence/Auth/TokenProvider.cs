using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoApp.Infrastructure.Persistence.Auth.Interfaces;
using TodoApp.Infrastructure.Persistence.Auth.Models;

namespace TodoApp.Infrastructure.Persistence.Auth
{
    public class TokenProvider : ITokenProvider
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public TokenProvider(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Get all claims using Identity's built-in claim management
            var claims = await GetUserClaimsAsync(user);

            // Add JWT specific claims
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
        }

        public bool IsRefreshTokenValid(RefreshToken? refreshToken)
        {
            return refreshToken != null && refreshToken.IsActive;
        }

        private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser applicationUser)
        {
            var claims = new List<Claim>();
            // Custom claims from domain user
            if (applicationUser.DomainUser != null)
            {
                claims.Add(new Claim("domainUserId", applicationUser.DomainUser.Id.ToString()));
                claims.Add(new Claim("displayName", applicationUser.DomainUser.DisplayName));
            }

            // Get user claims from Identity
            var userClaims = await _userManager.GetClaimsAsync(applicationUser);
            claims.AddRange(userClaims);

            // Get role claims
            var roles = await _userManager.GetRolesAsync(applicationUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Get role claims
                var identityRole = await _roleManager.FindByNameAsync(role);
                if (identityRole != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
                    claims.AddRange(roleClaims);
                }
            }

            return claims;
        }
    }
}
