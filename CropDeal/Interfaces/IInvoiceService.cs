using CropDeal.Models;

namespace CropDeal.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateInvoiceAsync(int transactionId, int? callerDealerId);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<List<Invoice>> GetInvoicesByDealerAsync(int dealerId);
        Task<int?> GetDealerIdByUserIdAsync(string userId);
    }
}