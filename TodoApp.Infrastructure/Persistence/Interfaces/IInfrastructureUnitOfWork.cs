using TodoApp.Application.Abstraction;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;

namespace TodoApp.Infrastructure.Persistence.Interfaces
{
    public interface IInfrastructureUnitOfWork : IUnitOfWork
    {
        IApplicationUserRepository ApplicationUsers { get; }
    }
} 