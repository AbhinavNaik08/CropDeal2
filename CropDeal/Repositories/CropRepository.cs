using Microsoft.EntityFrameworkCore;
using CropDeal.Data;
using CropDeal.Models;
using CropDeal.Interfaces;

namespace CropDeal.Repositories
{
    public class CropRepository : ICropRepository
    {
        private readonly ApplicationDbContext _context;

        public CropRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Crop> AddCropAsync(Crop crop)
        {
            _context.Crops.Add(crop);
            await _context.SaveChangesAsync();
            return crop;
        }

        public async Task<List<Crop>> GetAllCropsAsync()
        {
            return await _context.Crops
                .Include(c => c.Farmer)
                .ToListAsync();
        }

        public async Task<List<Crop>> GetCropsByFarmerAsync(int farmerId)
        {
            return await _context.Crops
                .Where(c => c.FarmerId == farmerId)
                .ToListAsync();
        }

        public async Task<Crop?> GetCropByIdAsync(int cropId)
        {
            return await _context.Crops
                .FirstOrDefaultAsync(c => c.Id == cropId);
        }

        public async Task<Crop?> UpdateCropAsync(Crop crop)
        {
            var existingCrop = await _context.Crops.FindAsync(crop.Id);

            if (existingCrop == null)
                return null;

            existingCrop.CropName = crop.CropName;
            existingCrop.CropType = crop.CropType;
            existingCrop.Quantity = crop.Quantity;
            existingCrop.ExpectedPrice = crop.ExpectedPrice;
            existingCrop.Location = crop.Location; 
            

            await _context.SaveChangesAsync();

            return existingCrop;
}
    }
}