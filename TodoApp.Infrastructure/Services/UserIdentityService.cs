using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Auth;

namespace TodoApp.Infrastructure.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public UserIdentityService(UserManager<ApplicationUser> userManager,
                                   RoleManager<IdentityRole<Guid>> roleManager,
                                   IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async System.Threading.Tasks.Task AddToRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid credentials"
                    };
                }

                bool isValid = await _userManager.CheckPasswordAsync(user, password);
                if (!isValid)
                {
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid credentials"
                    };
                }

                return new AuthenticationResult
                {
                    IsSuccess = true,
                    IdentityUserId = user.Id,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Authentication service error"
                };
            }
        }

        public async Task<bool> CheckPasswordAsync(Guid userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Guid> CreateUserAsync(string email, string password, User domainUser)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DomainUser = domainUser,
                DomainUserId = domainUser.Id
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.Id;
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, role);
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

        public async Task<string> GenerateJwtTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ArgumentException("User not found");

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

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ArgumentException("User not found");

            // Generate refresh token using Identity's token provider
            return await _userManager.GenerateUserTokenAsync(user, "RefreshTokenProvider", "RefreshToken");
        }

        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            // Validate refresh token using Identity's token provider
            return await _userManager.VerifyUserTokenAsync(user, "RefreshTokenProvider", "RefreshToken", refreshToken);
        }
    }
}
