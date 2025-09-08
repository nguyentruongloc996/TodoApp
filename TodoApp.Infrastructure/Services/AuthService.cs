using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

            return new LoginResponseDto
            {
                Token = GenerateJwtToken(user),
                RefreshToken = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddHours(24),
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

            return new LoginResponseDto
            {
                Token = GenerateJwtToken(existingUser),
                RefreshToken = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddHours(24),
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
            // In a real implementation, you would validate the refresh token here


            // For now, we'll return a mock response
            throw new NotImplementedException("Refresh token functionality not implemented yet");
        }

        public async System.Threading.Tasks.Task<bool> ValidateTokenAsync(string token)
        {
            // In a real implementation, you would validate the JWT token here
            // For now, we'll just check if it's not empty
            return !string.IsNullOrEmpty(token);
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

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim("userId", user.Id.ToString()),
                new System.Security.Claims.Claim("email", user.Email.Value),
                new System.Security.Claims.Claim("name", user.DisplayName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            // This is a placeholder implementation
            // In a real application, you would generate a secure refresh token
            return Guid.NewGuid().ToString();
        }
    }
} 