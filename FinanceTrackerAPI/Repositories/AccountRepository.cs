using FinanceTrackerAPI.Config;
using FinanceTrackerAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FinanceTrackerAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account> _accounts;

        public AccountRepository(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _accounts = database.GetCollection<Account>("Accounts");
        }

        public async Task<List<Account>> GetAccountsByUserId(string userId)
        {
            return await _accounts.Find(a => a.UserId == userId).ToListAsync();
        }

        public async Task<Account?> GetById(string id)
        {
            return await _accounts.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Account account)
        {
            await _accounts.InsertOneAsync(account);
        }

        public async Task Update(Account account)
        {
            await _accounts.ReplaceOneAsync(a => a.Id == account.Id, account);
        }

        public async Task Delete(string id)
        {
            await _accounts.DeleteOneAsync(a => a.Id == id);
        }
    }
}
