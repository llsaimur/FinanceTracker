using FinanceTrackerAPI.Models;
using FinanceTrackerAPI.Repositories;

namespace FinanceTrackerAPI.Services
{
    public class BudgetService
    {
        private readonly IBudgetRepository _repo;

        public BudgetService(IBudgetRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Budget>> GetUserBudgets(string userId)
        {
            return await _repo.GetBudgetsByUserId(userId);
        }

        public async Task<Budget?> GetBudget(string id)
        {
            return await _repo.GetById(id);
        }

        public async Task CreateBudget(Budget budget)
        {
            var existingBudgets = await _repo.GetBudgetsByUserId(budget.UserId);
            bool alreadyExists = existingBudgets.Any(b =>
                b.Category.Equals(budget.Category, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                throw new Exception($"A budget already exists for the category '{budget.Category}'.");
            }

            await _repo.Create(budget);
        }

        public async Task UpdateBudget(Budget budget)
        {
            var existingBudgets = await _repo.GetBudgetsByUserId(budget.UserId);

            bool duplicateExists = existingBudgets.Any(b =>
                b.Id != budget.Id && // Exclude the current budget
                b.Category.Equals(budget.Category, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
            {
                throw new Exception($"A budget already exists for the category '{budget.Category}'.");
            }

            await _repo.Update(budget);
        }


        public async Task DeleteBudget(string id)
        {
            await _repo.Delete(id);
        }
    }
}
