namespace CropDeal.Interfaces
{
    public interface IFarmerRepository
    {
        Task<bool> FarmerExistsAsync(int farmerId);

        Task<int?> GetFarmerIdByUserIdAsync(string userId);
    }
}