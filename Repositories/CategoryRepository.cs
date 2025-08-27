using Microsoft.EntityFrameworkCore;
using ReposirotyPatternWithUnitOfWork.Data;
using ReposirotyPatternWithUnitOfWork.Models;

namespace ReposirotyPatternWithUnitOfWork.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ECommerceDbContext _context;
        public CategoryRepository(ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }
        public async Task<Category?> GetByIdAsync(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }
        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }
        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }
        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
