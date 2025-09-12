using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Register;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Auth;
using TodoApp.Infrastructure.Persistence.Interfaces;

namespace TodoApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentityService _userIdentityService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUnitOfWork unitOfWork, IUserIdentityService userIdentityService, JwtSettings jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _userIdentityService = userIdentityService;
            _jwtSettings = jwtSettings;
        }

        public async System.Threading.Tasks.Task<RegisterRequestDto> RegisterAsync(RegisterCommand command)
        {
            // Validate email format
            var email = new Domain.ValueObjects.Email(command.Request.Email);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                DisplayName = command.Request.Name
            };
            var identityUserId = await _userIdentityService.CreateUserAsync(email.Value, command.Request.Password, user);

            await _unitOfWork.DomainUsers.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new RegisterRequestDto
            {
                Email = user.Email.Value,
                Name = user.DisplayName,
                Password = command.Request.Password
            };
        }

        public async System.Threading.Tasks.Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Single authentication call that handles both user lookup and password verification
            var authResult = await _userIdentityService.AuthenticateAsync(request.Email, request.Password);

            if (!authResult.IsSuccess || !authResult.IdentityUserId.HasValue)
                throw new UnauthorizedAccessException(string.IsNullOrEmpty(authResult.ErrorMessage) ? string.Empty : authResult.ErrorMessage);

            var identityUser = await _unitOfWork.ApplicationUsers.GetByIdAsync(authResult.IdentityUserId.Value);
            // Get domain user by IdentityUserId
            var user = identityUser?.DomainUser;
            if (user == null)
            {
                // Handle data inconsistency - user exists in Identity but not in domain
                throw new InvalidOperationException("User data inconsistency detected");
            }

            // Use Identity-based JWT token generation
            var jwtToken = await _userIdentityService.GenerateJwtTokenAsync(authResult.IdentityUserId.Value);
            var refreshToken = await _userIdentityService.GenerateRefreshTokenAsync(authResult.IdentityUserId.Value);

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    Name = user.DisplayName,
                    ProfilePicture = null
                }
            };
        }

        public async System.Threading.Tasks.Task<LoginResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request)
        {
            // In a real implementation, you would validate the Google ID token here
            // For now, we'll create a mock user
            
            var mockEmail = "google.user@example.com";
            var existingUser = await _unitOfWork.DomainUsers.GetByEmailAsync(mockEmail);
            
            if (existingUser == null)
            {
                // Create new user
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = new TodoApp.Domain.ValueObjects.Email(mockEmail),
                    DisplayName = "Google User"
                };

                existingUser = await _unitOfWork.DomainUsers.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();
            }

            // Find the corresponding Identity user
            var identityUser = await _unitOfWork.ApplicationUsers.GetByDomainUserIdAsync(existingUser.Id);
            if (identityUser == null)
                throw new InvalidOperationException("Identity user not found for domain user");

            var jwtToken = await _userIdentityService.GenerateJwtTokenAsync(identityUser.Id);
            var refreshToken = await _userIdentityService.GenerateRefreshTokenAsync(identityUser.Id);

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserDto
                {
                    Id = existingUser.Id,
                    Email = existingUser.Email.Value,
                    Name = existingUser.DisplayName,
                    ProfilePicture = null
                }
            };
        }

        public async System.Threading.Tasks.Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            // Extract user ID from the JWT token or implement a mapping mechanism
            var userId = ExtractUserIdFromToken(request.RefreshToken);
            
            // Validate the refresh token using Identity
            var isValid = await _userIdentityService.ValidateRefreshTokenAsync(userId, request.RefreshToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            // Generate new tokens
            var newJwtToken = await _userIdentityService.GenerateJwtTokenAsync(userId);
            var newRefreshToken = await _userIdentityService.GenerateRefreshTokenAsync(userId);

            // Get user info
            var identityUser = await _unitOfWork.ApplicationUsers.GetByIdAsync(userId);
            var user = identityUser?.DomainUser;
            if (user == null)
                throw new InvalidOperationException("User not found");

            return new LoginResponseDto
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    Name = user.DisplayName,
                    ProfilePicture = null
                }
            };
        }

        public async System.Threading.Tasks.Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Secret));
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async System.Threading.Tasks.Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.DomainUsers.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                Name = user.DisplayName,
                ProfilePicture = null
            };
        }

        private Guid ExtractUserIdFromToken(string token)
        {
            // This is a simplified implementation
            // You might want to decode the JWT or implement a proper mapping
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
            }
            catch
            {
                // Handle token parsing errors
            }
            
            throw new UnauthorizedAccessException("Invalid token format");
        }
    }
}