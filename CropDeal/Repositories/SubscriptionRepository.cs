using Microsoft.EntityFrameworkCore;
using CropDeal.Data;
using CropDeal.Interfaces;
using CropDeal.Models;

namespace CropDeal.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription> AddSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<List<Subscription>> GetSubscriptionsByDealerAsync(int dealerId)
        {
            return await _context.Subscriptions
                .Include(s => s.Crop)
                .Where(s => s.DealerId == dealerId)
                .ToListAsync();
        }

        public async Task DeleteSubscriptionAsync(int subscriptionId)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Subscription>> GetSubscriptionsByCropAsync(int cropId)
        {
            return await _context.Subscriptions
                .Include(s => s.Dealer)
                    .ThenInclude(d => d.User)
                .Where(s => s.CropId == cropId)
                .ToListAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);
        }
    }
}