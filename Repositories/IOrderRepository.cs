using ReposirotyPatternWithUnitOfWork.Models;
using ReposirotyPatternWithUnitOfWork.Repositories.GenericRepository;

namespace ReposirotyPatternWithUnitOfWork.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Declare an asynchronous method that retrieves all orders
        // Includes related Customer and OrderItems entities for comprehensive order details
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
        // Declare an asynchronous method to get all orders placed by a specific customer
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
        // Declare an asynchronous method to retrieve orders placed within a specified date range
        // Takes two DateTime parameters: startDate and endDate to filter orders
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
