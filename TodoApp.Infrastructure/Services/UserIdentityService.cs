using Microsoft.AspNetCore.Identity;
using TodoApp.Infrastructure.Abstraction.Services;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Auth;
using TodoApp.Application.Common.Result;

namespace TodoApp.Infrastructure.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserIdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

        public async Task<Result<Guid>> CreateUserAsync(string email, string password, User domainUser)
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
                return Error.Validation("Auth.CreateUserError", string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.Id;
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, role);
        }
    }
}
