using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Auth;

namespace TodoApp.Infrastructure.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserIdentityService(UserManager<ApplicationUser> userManager,
                                   RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async System.Threading.Tasks.Task AddToRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.AddToRoleAsync(user, role);
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

        public Task<bool> CheckPasswordAsync(Guid userId, string password)
        {
            return _userManager.CheckPasswordAsync(
            _userManager.Users.First(u => u.Id == userId), password);
        }

        public async Task<Guid> CreateUserAsync(string email, string password, User domainUser)
        {
            var user = new ApplicationUser { UserName = email, Email = email, DomainUser = domainUser };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.Id; // return IdentityUserId
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return await _userManager.IsInRoleAsync(user, role);
        }
    }
}
