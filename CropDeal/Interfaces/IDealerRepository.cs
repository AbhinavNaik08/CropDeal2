namespace CropDeal.Interfaces
{
    public interface IDealerRepository
    {
        Task<bool> DealerExistsAsync(int dealerId);
        Task<int?> GetDealerIdByUserIdAsync(string userId);

        Task<string?> GetDealerEmailAsync(int dealerId);
    }
}