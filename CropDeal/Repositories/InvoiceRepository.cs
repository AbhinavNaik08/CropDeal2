using CropDeal.Data;
using CropDeal.Interfaces;
using CropDeal.Models;
using Microsoft.EntityFrameworkCore;

namespace CropDeal.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _context.Invoices
                .Include(i => i.Transaction)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }

        public async Task<Invoice?> GetInvoiceByTransactionIdAsync(int transactionId)
        {
            return await _context.Invoices.Include(i=>i.Transaction)
                .FirstOrDefaultAsync(i => i.TransactionId == transactionId);
        }
        public async Task<IEnumerable<Invoice?>> GetAllInvoiceAsync()
        {
            return await _context.Invoices
                .Include(i => i.Transaction).ToListAsync();
        }

        public async Task<List<Invoice>> GetInvoicesByDealerAsync(int dealerId)
        {
            return await _context.Invoices
                .Include(i => i.Transaction)
                .Where(i => i.Transaction != null && i.Transaction.DealerId == dealerId)
                .ToListAsync();
        }

        
    }
}