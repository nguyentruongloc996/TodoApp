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
                DisplayName = userDto.Name
            };

            var createdUser = await _unitOfWork.DomainUsers.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = createdUser.Id,
                Name = createdUser.DisplayName
            };
        }

        public async System.Threading.Tasks.Task<UserDto> UpdateUserAsync(Guid userId, UserDto userDto)
        {
            var user = await _unitOfWork.DomainUsers.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            user.DisplayName = userDto.Name;

            var updatedUser = await _unitOfWork.DomainUsers.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = updatedUser.Id,
                Name = updatedUser.DisplayName
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
                Name = user.DisplayName
            };
        }

        public async System.Threading.Tasks.Task<List<UserDto>> SearchUsersAsync(string searchTerm)
        {
            var users = await _unitOfWork.DomainUsers.SearchByNameAsync(searchTerm);
            
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.DisplayName
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

        // Update methods to work with the new relationship
        public async Task<UserDto> GetUserByIdentityIdAsync(Guid identityUserId)
        {
            var user = await _unitOfWork.DomainUsers.GetByIdAsync(identityUserId);
            
            if (user == null)
                throw new ArgumentException("Domain user not found");

            return new UserDto
            {
                Id = user.Id,
                Name = user.DisplayName
            };
        }
    }
}