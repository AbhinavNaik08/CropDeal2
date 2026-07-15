namespace CropDeal.Interfaces
{
    public interface IPaymentService
    {
        Task<string> ProcessPaymentAsync(int transactionId, int? callerDealerId);
        Task<int?> GetDealerIdByUserIdAsync(string userId);
    }
}