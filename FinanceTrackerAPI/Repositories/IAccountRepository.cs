using FinanceTrackerAPI.Models;

namespace FinanceTrackerAPI.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAccountsByUserId(string userId);
        Task<Account?> GetById(string id);
        Task Create(Account account);
        Task Update(Account account);
        Task Delete(string id);
    }
}
