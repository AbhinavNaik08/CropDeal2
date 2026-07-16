using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;
using System.Transactions;
using CropDeal.DTOs.Invoice;
namespace CropDeal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        private async Task<int?> ResolveCallerDealerIdAsync()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _invoiceService.GetDealerIdByUserIdAsync(userId);
        }

        [Authorize(Roles = "Dealer,Admin")]
        [HttpPost("{transactionId}")]
        public async Task<IActionResult> CreateInvoice(int transactionId)
        {
            int? callerDealerId = null;

            if (!User.IsInRole("Admin"))
            {
                callerDealerId = await ResolveCallerDealerIdAsync();

                if (callerDealerId == null)
                    return BadRequest("No dealer profile found for this account.");
            }

            var invoice = await _invoiceService.CreateInvoiceAsync(transactionId, callerDealerId);

            var result= new InvoiceDto
            {
                Id=invoice.Id,
                TransactionId=invoice.TransactionId,
                Date=invoice.Date,
                DealerId=invoice.Transaction?.DealerId,
                Amount=invoice.Transaction?.Amount
            };


            return Ok(result);
        }

        [Authorize(Roles = "Dealer,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
                return NotFound();

            var result= new InvoiceDto
            {
                Id=invoice.Id,
                TransactionId=invoice.TransactionId,
                Date=invoice.Date,
                DealerId=invoice.Transaction?.DealerId,
                Amount=invoice.Transaction?.Amount
            };
            return Ok(result);
        }

        [Authorize(Roles = "Dealer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyInvoices()
        {
            var dealerId = await ResolveCallerDealerIdAsync();

            if (dealerId == null)
                return BadRequest("No dealer profile found for this account.");

            var invoices = await _invoiceService.GetInvoicesByDealerAsync(dealerId.Value);

            var result= invoices.Select(invoice => new InvoiceDto
            {
                Id = invoice.Id,
                TransactionId = invoice.TransactionId,
                Date = invoice.Date,
                DealerId = invoice.Transaction?.DealerId,
                Amount = invoice.Transaction?.Amount
            });

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> GetInvoicesByDealer(int dealerId)
        {
            var invoices = await _invoiceService.GetInvoicesByDealerAsync(dealerId);

            var result= invoices.Select(invoice => new InvoiceDto
            {
                Id = invoice.Id,
                TransactionId = invoice.TransactionId,
                Date = invoice.Date,
                DealerId = invoice.Transaction?.DealerId,
                Amount = invoice.Transaction?.Amount
            });

            return Ok(result);
        }
    }
}