using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.DTOs.Subscription;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;

namespace CropDeal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        private async Task<int?> ResolveCallerDealerIdAsync()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _subscriptionService.GetDealerIdByUserIdAsync(userId);
        }

        [Authorize(Roles = "Dealer")]
        [HttpPost]
        public async Task<IActionResult> Subscribe(CreateSubscriptionDto dto)
        {
            var dealerId = await ResolveCallerDealerIdAsync();

            if (dealerId == null)
                return BadRequest("No dealer profile found for this account.");

            var subscription = new Subscription
            {
                DealerId = dealerId.Value,
                CropId = dto.CropId
            };

            var result = await _subscriptionService.SubscribeAsync(subscription);

            return Ok(new SubscriptionDto
            {
                Id=result.Id,
                DealerId=result.DealerId,
                CropId=result.CropId
            });
        }

        [Authorize(Roles = "Dealer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var dealerId = await ResolveCallerDealerIdAsync();

            if (dealerId == null)
                return BadRequest("No dealer profile found for this account.");

            var subscriptions = await _subscriptionService.GetSubscriptionsByDealerAsync(dealerId.Value);

            var result = subscriptions.Select(s=> new SubscriptionDto
                    {
                        Id=s.Id,
                        DealerId=s.DealerId,
                        CropId=s.CropId
                    });

            return Ok(result);

           
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dealer/{dealerId}")]
        public async Task<IActionResult> GetSubscriptionsByDealer(int dealerId)
        {
            var subscriptions = await _subscriptionService.GetSubscriptionsByDealerAsync(dealerId);

            return Ok(subscriptions);
        }

        [Authorize(Roles = "Dealer,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            int? callerDealerId = null;

            if (!User.IsInRole("Admin"))
            {
                callerDealerId = await ResolveCallerDealerIdAsync();

                if (callerDealerId == null)
                    return BadRequest("No dealer profile found for this account.");
            }

            await _subscriptionService.UnsubscribeAsync(id, callerDealerId);

            return Ok("Unsubscribed successfully");
        }
    }
}