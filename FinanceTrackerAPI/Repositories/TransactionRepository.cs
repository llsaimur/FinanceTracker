using FinanceTrackerAPI.Config;
using FinanceTrackerAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FinanceTrackerAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IMongoCollection<Transaction> _collection;

        public TransactionRepository(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Transaction>("Transactions");
        }

        public async Task<List<Transaction>> GetTransactionsByUserId(string userId)
        {
            return await _collection.Find(t => t.UserId == userId).ToListAsync();
        }

        public async Task<Transaction?> GetById(string id)
        {
            return await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Transaction transaction)
        {
            await _collection.InsertOneAsync(transaction);
        }

        public async Task Update(Transaction transaction)
        {
            await _collection.ReplaceOneAsync(t => t.Id == transaction.Id, transaction);
        }

        public async Task Delete(string id)
        {
            await _collection.DeleteOneAsync(t => t.Id == id);
        }
    }
}
