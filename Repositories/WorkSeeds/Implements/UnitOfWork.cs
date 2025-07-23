using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories.WorkSeeds.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SWD392_G3DBcontext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private IDbContextTransaction? _transaction;

        // Specific repositories
        private IUserRepository? _userRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IOrderRepository _orderRepository;
        private IStaffRepository _staffRepository;
        private IStaffScheduleRepository _staffScheduleRepository;
        private IServiceRepository _serviceRepository;
        private ICustomerRepository? _customerRepository;
        private IRatingRepository _ratingRepository;
        private IMembershipRepository _MembershipRepositorysitory;
        private ICustomerMemberShipRepository _customerMemberShipRepository;
        private IOrderMembershipRepository _orderMembershipRepository;
        public UnitOfWork(SWD392_G3DBcontext context, IRepositoryFactory repositoryFactory)
        {
            _context = context;
            _repositoryFactory = repositoryFactory;
        }

        public IUserRepository UserRepository =>
            _userRepository ??= _repositoryFactory.GetCustomRepository<IUserRepository>();

        public IOrderRepository OrderRepository =>
            _orderRepository ??= _repositoryFactory.GetCustomRepository<IOrderRepository>();

        public IOrderDetailRepository OrderDetailRepository =>
            _orderDetailRepository ??= _repositoryFactory.GetCustomRepository<IOrderDetailRepository>();

        public IStaffRepository StaffRepository =>
            _staffRepository ??= _repositoryFactory.GetCustomRepository<IStaffRepository>();
        public IStaffScheduleRepository StaffScheduleRepository =>
            _staffScheduleRepository ??= _repositoryFactory.GetCustomRepository<IStaffScheduleRepository>();
        public IServiceRepository ServiceRepository =>
            _serviceRepository ??= _repositoryFactory.GetCustomRepository<IServiceRepository>();
        public ICustomerRepository CustomerRepository =>
            _customerRepository ??= _repositoryFactory.GetCustomRepository<ICustomerRepository>();
        public IRatingRepository RatingRepository => 
            _ratingRepository ??= _repositoryFactory.GetCustomRepository<IRatingRepository>();
        public IMembershipRepository MembershipRepository =>
            _MembershipRepositorysitory ??= _repositoryFactory.GetCustomRepository<IMembershipRepository>();
        public ICustomerMemberShipRepository CustomerMemberShipRepository => 
            _customerMemberShipRepository ??= _repositoryFactory.GetCustomRepository<ICustomerMemberShipRepository>();
        public IOrderMembershipRepository OrderMembershipRepository =>
            _orderMembershipRepository ??= _repositoryFactory.GetCustomRepository<IOrderMembershipRepository>();
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
            {
                Console.WriteLine("⚠️ RollbackTransactionAsync was called but no active transaction.");
                return; // Không ném lỗi nữa, tránh crash app
            }

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
