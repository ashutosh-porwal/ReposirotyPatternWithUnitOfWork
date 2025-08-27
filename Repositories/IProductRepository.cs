using ReposirotyPatternWithUnitOfWork.Models;
using ReposirotyPatternWithUnitOfWork.Repositories.GenericRepository;

namespace ReposirotyPatternWithUnitOfWork.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Declare an asynchronous method to get all products belonging to a specific category
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);

        // Declare an asynchronous method to get top selling products limited by count
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count);
    }
}
