using Microsoft.EntityFrameworkCore;
using ReposirotyPatternWithUnitOfWork.Data;
using ReposirotyPatternWithUnitOfWork.Models;

namespace ReposirotyPatternWithUnitOfWork.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _context;
        public ProductRepository(ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _context.Products.Include(p => p.Category)
                .AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == productId);
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }
        public void Update(Product product)
        {
            _context.Products.Update(product);
        }
        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(c => c.ProductId == id);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
