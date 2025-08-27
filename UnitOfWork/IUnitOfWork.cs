using Microsoft.EntityFrameworkCore.Storage;
using ReposirotyPatternWithUnitOfWork.Repositories;

namespace ReposirotyPatternWithUnitOfWork.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        ICustomerRepository Customers { get; }
        IOrderRepository Orders { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();

        Task RollbackAsync();

    }
}
