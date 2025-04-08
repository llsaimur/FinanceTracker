using FinanceTrackerAPI.Models;
using FinanceTrackerAPI.Repositories;

namespace FinanceTrackerAPI.Services
{
    public class TransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountRepository _accountRepo;

        public TransactionService(ITransactionRepository transactionRepo, IAccountRepository accountRepo)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
        }

        public async Task<List<Transaction>> GetUserTransactions(string userId)
        {
            return await _transactionRepo.GetTransactionsByUserId(userId);
        }

        public async Task<Transaction?> GetTransaction(string id)
        {
            return await _transactionRepo.GetById(id);
        }

        public async Task CreateTransaction(Transaction transaction)
        {
            var account = await _accountRepo.GetById(transaction.AccountId);
            if (account == null)
                throw new Exception("Associated account not found.");

            if (account.Balance < transaction.Amount)
                throw new Exception("Insufficient funds.");

            account.Balance -= transaction.Amount;
            await _accountRepo.Update(account);

            await _transactionRepo.Create(transaction);
        }

        public async Task UpdateTransaction(Transaction updatedTransaction)
        {
            var existingTransaction = await _transactionRepo.GetById(updatedTransaction.Id);
            if (existingTransaction == null)
                throw new Exception("Transaction not found.");

            var account = await _accountRepo.GetById(updatedTransaction.AccountId);
            if (account == null)
                throw new Exception("Associated account not found.");

            // If account ID changes, undo from old account and apply to new
            if (existingTransaction.AccountId != updatedTransaction.AccountId)
            {
                var oldAccount = await _accountRepo.GetById(existingTransaction.AccountId);
                var newAccount = account;

                if (oldAccount == null || newAccount == null)
                    throw new Exception("Account(s) not found for transaction update.");

                oldAccount.Balance += existingTransaction.Amount;
                newAccount.Balance -= updatedTransaction.Amount;

                await _accountRepo.Update(oldAccount);
                await _accountRepo.Update(newAccount);
            }
            else
            {
                var diff = updatedTransaction.Amount - existingTransaction.Amount;
                account.Balance -= diff;
                await _accountRepo.Update(account);
            }

            await _transactionRepo.Update(updatedTransaction);
        }

        public async Task DeleteTransaction(string id)
        {
            var transaction = await _transactionRepo.GetById(id);
            if (transaction == null)
                throw new Exception("Transaction not found.");

            var account = await _accountRepo.GetById(transaction.AccountId);
            if (account != null)
            {
                account.Balance += transaction.Amount;
                await _accountRepo.Update(account);
            }

            await _transactionRepo.Delete(id);
        }
    }
}
