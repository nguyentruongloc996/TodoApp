using Microsoft.EntityFrameworkCore.Storage;
using TodoApp.Infrastructure.Persistence.Interfaces;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;
using TodoApp.Infrastructure.Persistence.Repositories;
using TodoApp.Application.Abstraction.Repositories;

namespace TodoApp.Infrastructure.Persistence
{
    public class UnitOfWork : IInfrastructureUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        private ITaskRepository? _taskRepository;
        private IUserRepository? _userRepository;
        private IGroupRepository? _groupRepository;
        private ISubTaskRepository? _subTaskRepository;
        private IApplicationUserRepository? _applicationUserRepository;
        private IRefreshTokenRepository? _refreshTokenRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITaskRepository Tasks => _taskRepository ??= new TaskRepository(_context);
        public IUserRepository DomainUsers => _userRepository ??= new UserRepository(_context);
        public IGroupRepository Groups => _groupRepository ??= new GroupRepository(_context);
        public ISubTaskRepository SubTasks => _subTaskRepository ??= new SubTaskRepository(_context);
        public IApplicationUserRepository ApplicationUsers => _applicationUserRepository ??= new ApplicationUserRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public async System.Threading.Tasks.Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async System.Threading.Tasks.Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async System.Threading.Tasks.Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }
} 