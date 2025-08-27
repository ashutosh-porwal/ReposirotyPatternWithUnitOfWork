using ReposirotyPatternWithUnitOfWork.Models;

namespace ReposirotyPatternWithUnitOfWork.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
        Task<bool> ExistAsync(int id);
        Task SaveAsync();
    }
}
