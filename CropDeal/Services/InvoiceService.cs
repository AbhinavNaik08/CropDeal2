using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.Exceptions;

namespace CropDeal.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDealerRepository _dealerRepository;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            ITransactionRepository transactionRepository,
            IDealerRepository dealerRepository)
        {
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _dealerRepository = dealerRepository;
        }

        public async Task<Invoice> CreateInvoiceAsync(int transactionId, int? callerDealerId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                throw new NotFoundException("Transaction not found");

            if (callerDealerId != null && transaction.DealerId != callerDealerId)
                throw new ForbiddenException("You do not have permission to invoice this transaction.");    

            var existingInvoice= await _invoiceRepository.GetInvoiceByTransactionIdAsync(transactionId);
            if (existingInvoice != null)
                throw new BadRequestException("An invoice for this transaction already exists.");
            

            var invoice = new Invoice
            {
                TransactionId = transactionId,
                Date = DateTime.UtcNow
            };

            return await _invoiceRepository.AddInvoiceAsync(invoice);
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
        }

        public async Task<List<Invoice>> GetInvoicesByDealerAsync(int dealerId)
        {
            return await _invoiceRepository.GetInvoicesByDealerAsync(dealerId);
        }

        public async Task<int?> GetDealerIdByUserIdAsync(string userId)
        {
            return await _dealerRepository.GetDealerIdByUserIdAsync(userId);
        }
    }
}