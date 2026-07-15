using CropDeal.Data;
using CropDeal.Interfaces;
using CropDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CropDeal.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<Transaction>> GetTransactionsByDealerAsync(int dealerId)
        {
            return await _context.Transactions
                .Include(t => t.Crop)
                .Where(t => t.DealerId == dealerId)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<bool> DealerExistsAsync(int dealerId)
        {
            return await _context.Dealers.AnyAsync(d => d.Id == dealerId);
        }

        public async Task<Transaction?> GetTransactionWithDetailsAsync(int transactionId)
        {
            return await _context.Transactions
                .Include(t => t.Dealer)
                    .ThenInclude(d => d.User)
                .Include(t => t.Crop)
                    .ThenInclude(c => c.Farmer)
                        .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task AddPaymentEventAsync(PaymentEvent paymentEvent)
        {
            _context.PaymentEvents.Add(paymentEvent);
            await _context.SaveChangesAsync();
        }
    }
}