using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.User.SearchUsers;
public sealed record SearchUsersQuery(string SearchTerm) : IQuery<List<UserDto>> {} 