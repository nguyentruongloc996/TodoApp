using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Interfaces;

namespace TodoApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new TodoApp.Domain.ValueObjects.Email(userDto.Email),
                DisplayName = userDto.Name
            };

            var createdUser = await _unitOfWork.DomainUsers.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email.Value,
                Name = createdUser.DisplayName,
                ProfilePicture = userDto.ProfilePicture
            };
        }

        public async System.Threading.Tasks.Task<UserDto> UpdateUserAsync(Guid userId, UserDto userDto)
        {
            var user = await _unitOfWork.DomainUsers.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            user.Email = new TodoApp.Domain.ValueObjects.Email(userDto.Email);
            user.DisplayName = userDto.Name;

            var updatedUser = await _unitOfWork.DomainUsers.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = updatedUser.Id,
                Email = updatedUser.Email.Value,
                Name = updatedUser.DisplayName,
                ProfilePicture = userDto.ProfilePicture
            };
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

        public async System.Threading.Tasks.Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.DomainUsers.GetByEmailAsync(email);
            
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

        public async System.Threading.Tasks.Task<List<UserDto>> SearchUsersAsync(string searchTerm)
        {
            var users = await _unitOfWork.DomainUsers.SearchByNameAsync(searchTerm);
            
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                Name = user.DisplayName,
                ProfilePicture = null
            }).ToList();
        }

        public async System.Threading.Tasks.Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _unitOfWork.DomainUsers.GetByIdAsync(userId);
            if (user == null)
                return false;

            await _unitOfWork.DomainUsers.DeleteAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async System.Threading.Tasks.Task<bool> InviteUserAsync(Guid inviterId, string email, string message)
        {
            // In a real implementation, you would send an invitation email here
            // For now, we'll just check if the user exists
            var existingUser = await _unitOfWork.DomainUsers.GetByEmailAsync(email);
            return existingUser != null;
        }

        // Update methods to work with the new relationship
        public async Task<UserDto> GetUserByIdentityIdAsync(Guid identityUserId)
        {
            var applicationUser = await _unitOfWork.ApplicationUsers.GetByIdWithDomainUserAsync(identityUserId);
            
            if (applicationUser?.DomainUser == null)
                throw new ArgumentException("User not found");

            return new UserDto
            {
                Id = applicationUser.DomainUser.Id,
                Email = applicationUser.DomainUser.Email.Value,
                Name = applicationUser.DomainUser.DisplayName,
                ProfilePicture = null
            };
        }
    }
}