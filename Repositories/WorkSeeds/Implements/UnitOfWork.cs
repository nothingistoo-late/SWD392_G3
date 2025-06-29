using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories.WorkSeeds.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        //private readonly SWD392_G3DBcontext _context;
        //private readonly IUserRepository _userRepository;
        //private bool _disposed;

        //public UnitOfWork(SWD392_G3DBcontext context, IUserRepository userRepository)
        //{
        //    _context = context ?? throw new ArgumentNullException(nameof(context));
        //    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        //}

        //public IUserRepository UserRepository => _userRepository;

        //public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    return await _context.SaveChangesAsync(cancellationToken);
        //}

        //// Triển khai BeginTransactionAsync
        //public async Task<IDbContextTransaction> BeginTransactionAsync(
        //    IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        //    CancellationToken cancellationToken = default)
        //{
        //    // Gọi DatabaseFacade.BeginTransactionAsync với IsolationLevel
        //    return await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        //}
        //public async ValueTask DisposeAsync()
        //{
        //    if (!_disposed)
        //    {
        //        await _context.DisposeAsync();
        //        _disposed = true;
        //    }
        //    GC.SuppressFinalize(this);
        //}
        private readonly SWD392_G3DBcontext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private IDbContextTransaction? _transaction;

        // Specific repositories
        private IUserRepository? _userRepository;
        public UnitOfWork(SWD392_G3DBcontext context, IRepositoryFactory repositoryFactory)
        {
            _context = context;
            _repositoryFactory = repositoryFactory;
        }

        public IUserRepository UserRepository =>
            _userRepository ??= _repositoryFactory.GetCustomRepository<IUserRepository>();
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class
        {
            return _repositoryFactory.GetRepository<TEntity, TKey>();
        }

        public bool HasActiveTransaction => _transaction != null;

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already active.");

            _transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
            return _transaction;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to rollback.");

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();

            if (_context != null)
                await _context.DisposeAsync();
        }
    }
}
