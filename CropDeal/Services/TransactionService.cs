using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.Exceptions;

namespace CropDeal.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICropRepository _cropRepository;
        private readonly IDealerRepository _dealerRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ICropRepository cropRepository,
            IDealerRepository dealerRepository)
        {
            _transactionRepository = transactionRepository;
            _cropRepository = cropRepository;
            _dealerRepository = dealerRepository;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            var crop = await _cropRepository.GetCropByIdAsync(transaction.CropId);
            if (crop == null)
                throw new NotFoundException("Crop not found");

            var dealerExists = await _transactionRepository.DealerExistsAsync(transaction.DealerId);
            if (!dealerExists)
                throw new NotFoundException("Dealer not found");

            if (transaction.Quantity <= 0)
                throw new BadRequestException("Quantity must be greater than zero");

            if (transaction.Quantity > crop.Quantity)
                throw new BadRequestException("Requested quantity exceeds available crop quantity");

            crop.Quantity -= transaction.Quantity;
            await _cropRepository.UpdateCropAsync(crop);

            transaction.Amount = crop.ExpectedPrice * transaction.Quantity;

            return await _transactionRepository.AddTransactionAsync(transaction);
        }

        public async Task<List<Transaction>> GetTransactionsByDealerAsync(int dealerId)
        {
            return await _transactionRepository.GetTransactionsByDealerAsync(dealerId);
        }

        public async Task<int?> GetDealerIdByUserIdAsync(string userId)
        {
            return await _dealerRepository.GetDealerIdByUserIdAsync(userId);
        }
    }
}