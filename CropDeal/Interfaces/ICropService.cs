using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface ICropService
    {
        Task<Crop> PublishCropAsync(Crop crop);

        Task<List<Crop>> GetAllCropsAsync();

        Task<List<Crop>> GetCropsByFarmerAsync(int farmerId);

        Task<Crop?> GetCropByIdAsync(int cropId);

        Task<Crop?> UpdateCropAsync(Crop crop);

        Task<int?> GetFarmerIdByUserIdAsync(string userId);
    }
}