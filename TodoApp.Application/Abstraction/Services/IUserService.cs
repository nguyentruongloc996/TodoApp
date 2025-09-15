using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Abstraction.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(UserDto userDto);
        Task<UserDto> UpdateUserAsync(Guid userId, UserDto userDto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<List<UserDto>> SearchUsersAsync(string searchTerm);
        Task<bool> DeleteUserAsync(Guid userId);
    }
} 