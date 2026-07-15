using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using Microsoft.AspNetCore.Authorization;

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

            return Ok(invoice);
        }

        [Authorize(Roles = "Dealer,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        [Authorize(Roles = "Dealer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyInvoices()
        {
            var dealerId = await ResolveCallerDealerIdAsync();

            if (dealerId == null)
                return BadRequest("No dealer profile found for this account.");

            var invoices = await _invoiceService.GetInvoicesByDealerAsync(dealerId.Value);

            return Ok(invoices);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> GetInvoicesByDealer(int dealerId)
        {
            var invoices = await _invoiceService.GetInvoicesByDealerAsync(dealerId);

            return Ok(invoices);
        }
    }
}