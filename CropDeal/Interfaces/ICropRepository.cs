using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface ICropRepository
    {
        Task<Crop> AddCropAsync(Crop crop);

        Task<List<Crop>> GetAllCropsAsync();

        Task<List<Crop>> GetCropsByFarmerAsync(int farmerId);

        Task<Crop?> GetCropByIdAsync(int cropId);

        Task<Crop?> UpdateCropAsync(Crop crop);
    }
}