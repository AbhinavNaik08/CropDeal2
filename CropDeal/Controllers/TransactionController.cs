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

            var result = await _transactionService.CreateTransactionAsync(transaction);

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

            return Ok(transactions);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> GetTransactionsByDealer(int dealerId)
        {
            var transactions = await _transactionService.GetTransactionsByDealerAsync(dealerId);

            return Ok(transactions);
        }
    }
}