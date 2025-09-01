using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.User.CreateUser;
using TodoApp.Application.UseCases.User.SearchUsers;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ICommandHandle<CreateUserCommand, UserDto> _createUserCommandHandle;
        private readonly IQueryHandle<SearchUsersQuery, List<UserDto>> _searchUsersQueryHandle;

        public UserController(
            ICommandHandle<CreateUserCommand, UserDto> createUserCommandHandle,
            IQueryHandle<SearchUsersQuery, List<UserDto>> searchUsersQueryHandle)
        {
            _createUserCommandHandle = createUserCommandHandle;
            _searchUsersQueryHandle = searchUsersQueryHandle;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand(userDto);
            var result = await _createUserCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm, CancellationToken cancellationToken)
        {
            var query = new SearchUsersQuery(searchTerm);
            var result = await _searchUsersQueryHandle.Handle(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancellationToken)
        {
            // TODO: Implement get user by ID query
            return Ok(new { message = "Get user endpoint - to be implemented" });
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDto userDto, CancellationToken cancellationToken)
        {
            // TODO: Implement update user command
            return Ok(new { message = "Update user endpoint - to be implemented" });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            // TODO: Implement delete user command
            return Ok(new { message = "Delete user endpoint - to be implemented" });
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteUser([FromBody] InviteUserRequestDto request, CancellationToken cancellationToken)
        {
            // TODO: Implement invite user command
            return Ok(new { message = "Invite user endpoint - to be implemented" });
        }
    }
} 