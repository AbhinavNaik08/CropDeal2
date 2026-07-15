using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);

        Task<List<Transaction>> GetTransactionsByDealerAsync(int dealerId);

        Task<Transaction?> GetTransactionByIdAsync(int transactionId);

        Task<bool> DealerExistsAsync(int dealerId);

        Task<Transaction?> GetTransactionWithDetailsAsync(int transactionId);

        Task AddPaymentEventAsync(PaymentEvent paymentEvent);
    }
}