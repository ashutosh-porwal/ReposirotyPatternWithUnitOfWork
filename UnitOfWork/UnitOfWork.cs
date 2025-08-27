using Microsoft.EntityFrameworkCore.Storage;
using ReposirotyPatternWithUnitOfWork.Data;
using ReposirotyPatternWithUnitOfWork.Repositories;
using System.Threading.Tasks;

namespace ReposirotyPatternWithUnitOfWork.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _context;

        private IDbContextTransaction? _transaction;
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public ICustomerRepository Customers { get; }
        public IOrderRepository Orders { get; }

        public UnitOfWork(ECommerceDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Customers = new CustomerRepository(_context);
            Orders = new OrderRepository(_context);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_transaction != null)
                return _transaction;

            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitAsync()
        {
            if(_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            await _context.SaveChangesAsync();

            await _transaction.CommitAsync();

            await DisposeTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to rollback.");

            await _transaction.RollbackAsync();

            await DisposeTransactionAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
