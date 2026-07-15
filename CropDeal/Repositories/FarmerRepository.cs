using CropDeal.Data;
using CropDeal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeal.Repositories
{
    public class FarmerRepository : IFarmerRepository
    {
        private readonly ApplicationDbContext _context;

        public FarmerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> FarmerExistsAsync(int farmerId)
        {
            return await _context.Farmers.AnyAsync(f => f.Id == farmerId);
        }

        public async Task<int?> GetFarmerIdByUserIdAsync(string userId)
        {
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);
            return farmer?.Id;
        }
    }
}