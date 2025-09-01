using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.User.SearchUsers;

public sealed class SearchUsersQueryHandle(IUserService userService) : IQueryHandle<SearchUsersQuery, List<UserDto>> {
    public async Task<List<UserDto>> Handle(SearchUsersQuery query, CancellationToken cancellationToken) {
        return await userService.SearchUsersAsync(query.SearchTerm);
    }
} 