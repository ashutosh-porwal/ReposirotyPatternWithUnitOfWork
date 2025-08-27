using Microsoft.EntityFrameworkCore;
using ReposirotyPatternWithUnitOfWork.Data;
using ReposirotyPatternWithUnitOfWork.Models;
using ReposirotyPatternWithUnitOfWork.Repositories.GenericRepository;

namespace ReposirotyPatternWithUnitOfWork.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ECommerceDbContext _context;

        // Fix CS7036: Pass context to base constructor
        public ProductRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count)
        {
            return await _context.Products
                .OrderByDescending(p => p.OrderItems.Sum(s=>s.Quantity))
                .Include(p=>p.Category)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
