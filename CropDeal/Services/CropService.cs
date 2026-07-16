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

            var updatedCrop= await _cropRepository.UpdateCropAsync(crop);

            var subscriptions = await _subscriptionRepository.GetSubscriptionsByCropAsync(crop.Id);

            foreach(var sub in subscriptions)
            {
                var email= sub.Dealer?.User?.Email;

                if(string.IsNullOrEmpty(email))
                    continue;

                try
                {
                    await _emailService.SendEmailAsync(email,"Crop updates", $"'{updatedCrop.CropName}' has been updated, check the latest quantity and price.");
                }

                catch(Exception ex)
                {
                    Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
                }
            }

            return await _cropRepository.UpdateCropAsync(crop);
        }

        public async Task<int?> GetFarmerIdByUserIdAsync(string userId)
        {
            return await _farmerRepository.GetFarmerIdByUserIdAsync(userId);
        }
    }
}