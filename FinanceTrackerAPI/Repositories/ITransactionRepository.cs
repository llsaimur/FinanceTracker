using FinanceTrackerAPI.Models;

namespace FinanceTrackerAPI.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetTransactionsByUserId(string userId);
        Task<Transaction?> GetById(string id);
        Task Create(Transaction transaction);
        Task Update(Transaction transaction);
        Task Delete(string id);
    }
}
