using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;

namespace TodoApp.Infrastructure.Persistence.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository Tasks { get; }
        IUserRepository DomainUsers { get; }
        IGroupRepository Groups { get; }
        ISubTaskRepository SubTasks { get; }
        IApplicationUserRepository ApplicationUsers { get; }

        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task BeginTransactionAsync();
        System.Threading.Tasks.Task CommitTransactionAsync();
        System.Threading.Tasks.Task RollbackTransactionAsync();
    }
} 