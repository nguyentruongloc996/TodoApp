using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TodoApp.Application.Abstraction;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.Common.Result;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Register;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Abstraction.Services;
using TodoApp.Infrastructure.Persistence.Auth;
using TodoApp.Infrastructure.Persistence.Interfaces;

namespace TodoApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IInfrastructureUnitOfWork _unitOfWork;
        private readonly IUserIdentityService _userIdentityService;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenProvider _tokenProvider;

        public AuthService(IInfrastructureUnitOfWork unitOfWork, IUserIdentityService userIdentityService, IOptions<JwtSettings> jwtSettings, ITokenProvider tokenProvider)
        {
            _unitOfWork = unitOfWork;
            _userIdentityService = userIdentityService;
            _jwtSettings = jwtSettings.Value;
            _tokenProvider = tokenProvider;
        }

        public async Task<Result<RegisterRequestDto>> RegisterAsync(RegisterCommand command)
        {
            // Validate email format.
            // This should be changed to use a more robust validation in production.
            var email = new Domain.ValueObjects.Email(command.Request.Email);
            if (email.Errors.Any())
            {
                return Error.Validation(
                    "Auth.InvalidEmail",
                    email.Errors.First());
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = command.Request.Name
            };

            var identityUserId = await _userIdentityService.CreateUserAsync(email.Value, command.Request.Password, user);
            if (identityUserId.IsFailure)
                return identityUserId.Error;

            return new RegisterRequestDto
            {
                Email = email.Value,
                Name = user.DisplayName,
                Password = command.Request.Password
            };
        }

        public async System.Threading.Tasks.Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            // Single authentication call that handles both user lookup and password verification
            var authResult = await _userIdentityService.AuthenticateAsync(request.Email, request.Password);

            if (!authResult.IsSuccess || !authResult.IdentityUserId.HasValue)
            {
                if (string.IsNullOrEmpty(authResult.ErrorMessage))
                {
                    return DomainErrors.Auth.InvalidCredentials;
                }

                return Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    string.IsNullOrEmpty(authResult.ErrorMessage) ? string.Empty : authResult.ErrorMessage);
            }

            var identityUser = await _unitOfWork.ApplicationUsers.GetByIdWithDomainUserAsync(authResult.IdentityUserId.Value);

            // Use Identity-based JWT token generation
            var jwtToken = await _tokenProvider.GenerateJwtToken(identityUser);
            var refreshToken = _tokenProvider.GenerateRefreshToken();

            refreshToken.UserId = identityUser.Id;
            _unitOfWork.RefreshTokens.Add(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
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

            var jwtToken = await _tokenProvider.GenerateJwtToken(existingUser);
            var refreshToken = _tokenProvider.GenerateRefreshToken();
            refreshToken.UserId = existingUser.Id;

            return new LoginResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }

        public async Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            // Find the refresh token in the database
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);
            if (!_tokenProvider.IsRefreshTokenValid(refreshToken))
                return DomainErrors.Auth.InvalidRefreshToken;

            // Revoke the used refresh token
            refreshToken.Revoked = DateTime.UtcNow;

            var applicationUser = await _unitOfWork.ApplicationUsers.GetByIdAsync(refreshToken.UserId);
            if (applicationUser == null)
               return DomainErrors.User.NotFound;

            // Generate new tokens
            var newJwtToken = await _tokenProvider.GenerateJwtToken(applicationUser);
            var newRefreshToken = _tokenProvider.GenerateRefreshToken();

            newRefreshToken.UserId = applicationUser.Id;
            _unitOfWork.RefreshTokens.Add(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
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