using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Auth.Register;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Interfaces;

namespace TodoApp.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task<RegisterRequestDto> RegisterAsync(RegisterCommand command)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new TodoApp.Domain.ValueObjects.Email(command.Request.Email),
                DisplayName = command.Request.Name
            };

            await _unitOfWork.Users.AddAsync(user);
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
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null)
                throw new ArgumentException("Invalid email or password");

            // In a real implementation, you would verify the password hash here
            // For now, we'll just check if the user exists

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
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(mockEmail);
            
            if (existingUser == null)
            {
                // Create new user
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = new TodoApp.Domain.ValueObjects.Email(mockEmail),
                    DisplayName = "Google User"
                };

                existingUser = await _unitOfWork.Users.AddAsync(newUser);
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
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
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
            // This is a placeholder implementation
            // In a real application, you would use a proper JWT library
            return $"jwt_token_for_{user.Id}";
        }

        private string GenerateRefreshToken()
        {
            // This is a placeholder implementation
            // In a real application, you would generate a secure refresh token
            return Guid.NewGuid().ToString();
        }
    }
} 