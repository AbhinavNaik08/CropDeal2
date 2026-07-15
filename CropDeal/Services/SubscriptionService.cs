using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.Exceptions;

namespace CropDeal.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICropRepository _cropRepository;
        private readonly IDealerRepository _dealerRepository;
        private readonly IEmailService _emailService;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            ITransactionRepository transactionRepository,
            ICropRepository cropRepository,
            IDealerRepository dealerRepository,
            IEmailService emailService)
        {
            _subscriptionRepository = subscriptionRepository;
            _transactionRepository = transactionRepository;
            _cropRepository = cropRepository;
            _dealerRepository = dealerRepository;
            _emailService = emailService;
        }

        public async Task<Subscription> SubscribeAsync(Subscription subscription)
        {
            if (subscription.DealerId <= 0)
                throw new BadRequestException("Invalid dealer");

            if (subscription.CropId <= 0)
                throw new BadRequestException("Invalid crop");

            var dealerExists = await _transactionRepository.DealerExistsAsync(subscription.DealerId);
            if (!dealerExists)
                throw new NotFoundException("Dealer not found");

            var crop = await _cropRepository.GetCropByIdAsync(subscription.CropId);
            if (crop == null)
                throw new NotFoundException("Crop not found");

            var savedSubscription = await _subscriptionRepository.AddSubscriptionAsync(subscription);

            // Best-effort confirmation email — a failed email should never fail the subscription itself.
            var dealerEmail = await _dealerRepository.GetDealerEmailAsync(subscription.DealerId);

            if (!string.IsNullOrEmpty(dealerEmail))
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        dealerEmail,
                        "Subscription Confirmed",
                        $"You've subscribed to updates for '{crop.CropName}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send subscription confirmation email to {dealerEmail}: {ex.Message}");
                }
            }

            return savedSubscription;
        }

        public async Task<List<Subscription>> GetSubscriptionsByDealerAsync(int dealerId)
        {
            return await _subscriptionRepository.GetSubscriptionsByDealerAsync(dealerId);
        }

        public async Task UnsubscribeAsync(int subscriptionId, int? callerDealerId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);

            if (subscription == null)
                throw new NotFoundException("Subscription not found");

            if (callerDealerId != null && subscription.DealerId != callerDealerId)
                throw new ForbiddenException("You do not have permission to remove this subscription.");

            await _subscriptionRepository.DeleteSubscriptionAsync(subscriptionId);
        }

        public async Task<int?> GetDealerIdByUserIdAsync(string userId)
        {
            return await _dealerRepository.GetDealerIdByUserIdAsync(userId);
        }
    }
}