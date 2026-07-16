using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.DTOs.Transaction;
using Microsoft.AspNetCore.Authorization;

namespace CropDeal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        private async Task<int?> ResolveCallerDealerIdAsync()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _transactionService.GetDealerIdByUserIdAsync(userId);
        }

        [Authorize(Roles = "Dealer")]
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
        {
            var dealerId = await ResolveCallerDealerIdAsync();

            if (dealerId == null)
                return BadRequest("No dealer profile found for this account.");

            var transaction = new Transaction
            {
                CropId = dto.CropId,
                DealerId = dealerId.Value,
                Quantity = dto.Quantity
            };

            var created = await _transactionService.CreateTransactionAsync(transaction);

            var result= new TransactionDto
            {
                Id=created.Id,
                CropId=created.CropId,
                CropName=created.Crop?.CropName,
                DealerId=created.DealerId,
                Quantity=created.Quantity,
                Amount=created.Amount
            };

            return Ok(result);
        }

        [Authorize(Roles = "Dealer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var dealerId = await ResolveCallerDealerIdAsync();

            if (dealerId == null)
                return BadRequest("No dealer profile found for this account.");

            var transactions = await _transactionService.GetTransactionsByDealerAsync(dealerId.Value);

            var result= transactions.Select(t=> new TransactionDto
            {
                Id=t.Id,
                CropId=t.CropId,
                CropName=t.Crop?.CropName,
                DealerId=t.DealerId,
                Quantity=t.Quantity,
                Amount=t.Amount
            });

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> GetTransactionsByDealer(int dealerId)
        {
            var transactions = await _transactionService.GetTransactionsByDealerAsync(dealerId);

            var result= transactions.Select(t=> new TransactionDto
            {
                Id=t.Id,
                CropId=t.CropId,
                CropName=t.Crop?.CropName,
                DealerId=t.DealerId,
                Quantity=t.Quantity,
                Amount=t.Amount
            });

            return Ok(result);
        }
    }
}