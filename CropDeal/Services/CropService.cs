using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.Exceptions;

namespace CropDeal.Services
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _cropRepository;
        private readonly IEmailService _emailService;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IFarmerRepository _farmerRepository;

        public CropService(
            ICropRepository cropRepository,
            IEmailService emailService,
            ISubscriptionRepository subscriptionRepository,
            IFarmerRepository farmerRepository)
        {
            _cropRepository = cropRepository;
            _emailService = emailService;
            _subscriptionRepository = subscriptionRepository;
            _farmerRepository = farmerRepository;
        }

        public async Task<Crop> PublishCropAsync(Crop crop)
        {
            if (crop.Quantity <= 0)
                throw new BadRequestException("Crop quantity must be greater than zero");

            if (crop.ExpectedPrice <= 0)
                throw new BadRequestException("Expected price must be greater than zero");

            var farmerExists = await _farmerRepository.FarmerExistsAsync(crop.FarmerId);

            if (!farmerExists)
                throw new NotFoundException("Farmer not found");

            var savedCrop = await _cropRepository.AddCropAsync(crop);

            // var subscriptions = await _subscriptionRepository
            //     .GetSubscriptionsByCropAsync(savedCrop.Id);

            // foreach (var subscription in subscriptions)
            // {
            //     var dealerUser = subscription.Dealer?.User;

            //     if (dealerUser != null && !string.IsNullOrEmpty(dealerUser.Email))
            //     {
            //         await _emailService.SendEmailAsync(
            //             dealerUser.Email,
            //             "New Crop Available",
            //             $"A new crop '{crop.CropName}' has been published.");
            //     }
            // }

            return savedCrop;
        }

        public async Task<List<Crop>> GetAllCropsAsync()
        {
            return await _cropRepository.GetAllCropsAsync();
        }

        public async Task<List<Crop>> GetCropsByFarmerAsync(int farmerId)
        {
            return await _cropRepository.GetCropsByFarmerAsync(farmerId);
        }

        public async Task<Crop?> GetCropByIdAsync(int cropId)
        {
            return await _cropRepository.GetCropByIdAsync(cropId);
        }

        public async Task<Crop?> UpdateCropAsync(Crop crop)
        {
            var existingCrop = await _cropRepository.GetCropByIdAsync(crop.Id);

            if (existingCrop == null)
                return null;

            return await _cropRepository.UpdateCropAsync(crop);
        }

        public async Task<int?> GetFarmerIdByUserIdAsync(string userId)
        {
            return await _farmerRepository.GetFarmerIdByUserIdAsync(userId);
        }
    }
}