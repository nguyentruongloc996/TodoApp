using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public ApplicationUserDto User { get; set; }
    }

    public class GoogleAuthRequestDto
    {
        [Required]
        public string IdToken { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    public class InviteUserRequestDto
    {
        [Required]
        public string Email { get; set; }
        public string? Message { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class ApplicationUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? ProfilePicture { get; set; }
    }
} 