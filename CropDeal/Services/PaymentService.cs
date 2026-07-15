using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.Exceptions;

namespace CropDeal.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEmailService _emailService;
        private readonly IDealerRepository _dealerRepository;

        public PaymentService(
            ITransactionRepository transactionRepository,
            IEmailService emailService,
            IDealerRepository dealerRepository)
        {
            _transactionRepository = transactionRepository;
            _emailService = emailService;
            _dealerRepository = dealerRepository;
        }

        public async Task<string> ProcessPaymentAsync(int transactionId, int? callerDealerId)
        {
            var transaction = await _transactionRepository.GetTransactionWithDetailsAsync(transactionId);

            if (transaction == null)
                throw new NotFoundException("Transaction not found.");

            if (callerDealerId != null && transaction.DealerId != callerDealerId)
                throw new ForbiddenException("You do not have permission to pay for this transaction.");

            if (transaction.Dealer?.User == null)
                throw new NotFoundException("Dealer information not found.");

            if (transaction.Crop?.Farmer?.User == null)
                throw new NotFoundException("Farmer information not found.");

            await _transactionRepository.AddPaymentEventAsync(new PaymentEvent
            {
                TransactionId = transactionId,
                EventType = "PaymentInitiated",
                Timestamp = DateTime.UtcNow
            });

            bool paymentSuccess = true;

            if (paymentSuccess)
            {
                await _transactionRepository.AddPaymentEventAsync(new PaymentEvent
                {
                    TransactionId = transactionId,
                    EventType = "PaymentCompleted",
                    Timestamp = DateTime.UtcNow
                });

                var dealerSubject = "Payment Successful";
                var dealerBody =
                    $"Hello {transaction.Dealer.User.FullName},\n\n" +
                    $"Your payment for Transaction #{transaction.Id} was processed successfully.\n\n" +
                    $"Crop: {transaction.Crop.CropName}\n" +
                    $"Amount Paid: \u20b9{transaction.Amount}\n\n" +
                    $"Thank you for using CropDeal!\n\n" +
                    $"Regards,\nCropDeal Team";

                try
                {
                    await _emailService.SendEmailAsync(transaction.Dealer.User.Email!, dealerSubject, dealerBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send payment email to dealer {transaction.Dealer.User.Email}: {ex.Message}");
                }

                var farmerSubject = "Payment Received";
                var farmerBody =
                    $"Hello {transaction.Crop.Farmer.User.FullName},\n\n" +
                    $"Good news! A dealer has successfully paid for your crop.\n\n" +
                    $"Crop: {transaction.Crop.CropName}\n" +
                    $"Transaction ID: {transaction.Id}\n" +
                    $"Amount Received: \u20b9{transaction.Amount}\n\n" +
                    $"Please prepare your crop for further processing.\n\n" +
                    $"Thank you for using CropDeal!\n\nRegards,\nCropDeal Team";

                try
                {
                    await _emailService.SendEmailAsync(transaction.Crop.Farmer.User.Email!, farmerSubject, farmerBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send payment email to farmer {transaction.Crop.Farmer.User.Email}: {ex.Message}");
                }

                return "Payment Successful";
            }
            else
            {
                await _transactionRepository.AddPaymentEventAsync(new PaymentEvent
                {
                    TransactionId = transactionId,
                    EventType = "PaymentFailed",
                    Timestamp = DateTime.UtcNow
                });

                return "Payment Failed";
            }
        }

        public async Task<int?> GetDealerIdByUserIdAsync(string userId)
        {
            return await _dealerRepository.GetDealerIdByUserIdAsync(userId);
        }
    }
}