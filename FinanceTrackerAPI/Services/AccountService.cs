using FinanceTrackerAPI.Models;
using FinanceTrackerAPI.Repositories;

namespace FinanceTrackerAPI.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Account>> GetUserAccounts(string userId)
        {
            return await _repo.GetAccountsByUserId(userId);
        }

        public async Task<Account?> GetAccount(string id)
        {
            return await _repo.GetById(id);
        }

        public async Task CreateAccount(Account account)
        {
            var existingAccounts = await _repo.GetAccountsByUserId(account.UserId);

            if (existingAccounts.Any(a => a.Name.Equals(account.Name, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("An account with that name already exists.");

            await _repo.Create(account);
        }


        public async Task UpdateAccount(Account account)
        {
            var existingAccounts = await _repo.GetAccountsByUserId(account.UserId);

            if (existingAccounts.Any(a =>
                a.Id != account.Id &&
                a.Name.Equals(account.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("An account with that name already exists.");
            }

            await _repo.Update(account);
        }


        public async Task DeleteAccount(string id)
        {
            await _repo.Delete(id);
        }
    }
}
