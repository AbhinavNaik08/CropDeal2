using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> AddSubscriptionAsync(Subscription subscription);

        Task<List<Subscription>> GetSubscriptionsByDealerAsync(int dealerId);

        Task DeleteSubscriptionAsync(int subscriptionId);

        Task<List<Subscription>> GetSubscriptionsByCropAsync(int cropId);

        Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId);
    }
}