using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CropDeal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        //no need of admins to do payment
        [Authorize(Roles = "Dealer")]
        [HttpPost("{transactionId}")]
        public async Task<IActionResult> ProcessPayment(int transactionId)
        {
            int? callerDealerId = null;

            var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                callerDealerId = await _paymentService.GetDealerIdByUserIdAsync(userId);

                if (callerDealerId == null)
                    return BadRequest("No dealer profile found for this account.");

            var result = await _paymentService.ProcessPaymentAsync(transactionId, callerDealerId);

            return Ok(new { message = result });
        }
    }
}