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
                DisplayName = command.Request.Name
            };
            var identityUserId = await _userIdentityService.CreateUserAsync(email.Value, command.Request.Password, user);

            await _unitOfWork.DomainUsers.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new RegisterRequestDto
            {
                Email = email.Value,
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

            var identityUser = await _unitOfWork.ApplicationUsers.GetByIdWithDomainUserAsync(authResult.IdentityUserId.Value);
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
                    IdentityId = identityUser.Id,
                    UserId = user.Id,
                    Email = identityUser.Email ?? string.Empty,
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
            var existingUser = await _unitOfWork.ApplicationUsers.GetByEmailAsync(mockEmail);

            if (existingUser == null)
                throw new InvalidOperationException("Identity user not found for domain user");

            var jwtToken = await _userIdentityService.GenerateJwtTokenAsync(existingUser.Id);
            var refreshToken = await _userIdentityService.GenerateRefreshTokenAsync(existingUser.Id);

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserDto
                {
                    IdentityId = existingUser.Id,
                    UserId = existingUser.DomainUser.Id,
                    Email = existingUser.Email ?? string.Empty,
                    Name = existingUser.DomainUser.DisplayName,
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
            var identityUser = await _unitOfWork.ApplicationUsers.GetByIdWithDomainUserAsync(userId);

            if (identityUser == null)
                throw new InvalidOperationException("User not found");

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
                    IdentityId = identityUser.Id,
                    UserId = user.Id,
                    Email = identityUser.Email,
                    Name = user.DisplayName,
                    ProfilePicture = null
                }
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