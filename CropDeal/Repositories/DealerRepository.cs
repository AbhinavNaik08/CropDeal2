using CropDeal.Data;
using CropDeal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CropDeal.Repositories
{
    public class DealerRepository : IDealerRepository
    {
        private readonly ApplicationDbContext _context;

        public DealerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DealerExistsAsync(int dealerId)
        {
            return await _context.Dealers.AnyAsync(d => d.Id == dealerId);
        }

        public async Task<int?> GetDealerIdByUserIdAsync(string userId)
        {
            var dealer = await _context.Dealers.FirstOrDefaultAsync(d => d.UserId == userId);
            return dealer?.Id;
        }

        public async Task<string?> GetDealerEmailAsync(int dealerId)
        {
            var dealer = await _context.Dealers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == dealerId);

            return dealer?.User?.Email;
        }

    }
}