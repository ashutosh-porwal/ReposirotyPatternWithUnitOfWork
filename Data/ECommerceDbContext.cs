using Microsoft.EntityFrameworkCore;

namespace ReposirotyPatternWithUnitOfWork.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Product> Products { get; set; } = null!;
        public DbSet<Models.Customer> Customers { get; set; } = null!;
        public DbSet<Models.Order> Orders { get; set; } = null!;
        public DbSet<Models.Category> Categories { get; set; } = null!;
    }
}
