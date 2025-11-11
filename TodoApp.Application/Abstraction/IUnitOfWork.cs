using TodoApp.Application.Abstraction.Repositories;

namespace TodoApp.Application.Abstraction
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository Tasks { get; }
        IUserRepository DomainUsers { get; }
        IGroupRepository Groups { get; }
        ISubTaskRepository SubTasks { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task BeginTransactionAsync();
        System.Threading.Tasks.Task CommitTransactionAsync();
        System.Threading.Tasks.Task RollbackTransactionAsync();
    }
}