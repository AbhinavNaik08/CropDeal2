using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> SubscribeAsync(Subscription subscription);
        Task<List<Subscription>> GetSubscriptionsByDealerAsync(int dealerId);
        Task UnsubscribeAsync(int subscriptionId, int? callerDealerId);
        Task<int?> GetDealerIdByUserIdAsync(string userId);
    }
}