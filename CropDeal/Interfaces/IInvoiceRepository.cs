using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> AddInvoiceAsync(Invoice invoice);

        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);

        Task<Invoice?> GetInvoiceByTransactionIdAsync(int transactionId);

        Task<List<Invoice>> GetInvoicesByDealerAsync(int dealerId);
        Task<IEnumerable<Invoice?>> GetAllInvoiceAsync();
    }
}