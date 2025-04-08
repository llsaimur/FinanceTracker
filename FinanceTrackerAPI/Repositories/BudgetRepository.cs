using FinanceTrackerAPI.Config;
using FinanceTrackerAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FinanceTrackerAPI.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly IMongoCollection<Budget> _collection;

        public BudgetRepository(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Budget>("Budgets");
        }

        public async Task<List<Budget>> GetBudgetsByUserId(string userId)
        {
            return await _collection.Find(b => b.UserId == userId).ToListAsync();
        }

        public async Task<Budget?> GetById(string id)
        {
            return await _collection.Find(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Budget budget)
        {
            await _collection.InsertOneAsync(budget);
        }

        public async Task Update(Budget budget)
        {
            await _collection.ReplaceOneAsync(b => b.Id == budget.Id, budget);
        }

        public async Task Delete(string id)
        {
            await _collection.DeleteOneAsync(b => b.Id == id);
        }
    }
}
