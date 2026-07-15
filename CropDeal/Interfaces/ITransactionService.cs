using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(Transaction transaction);

        Task<List<Transaction>> GetTransactionsByDealerAsync(int dealerId);

        Task<int?> GetDealerIdByUserIdAsync(string userId);
    }
}