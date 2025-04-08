using FinanceTrackerAPI.Models;

namespace FinanceTrackerAPI.Repositories
{
    public interface IBudgetRepository
    {
        Task<List<Budget>> GetBudgetsByUserId(string userId);
        Task<Budget?> GetById(string id);
        Task Create(Budget budget);
        Task Update(Budget budget);
        Task Delete(string id);
    }
}
