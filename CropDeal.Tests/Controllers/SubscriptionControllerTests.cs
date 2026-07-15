using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.DTOs.Subscription;
using CropDeal.Exceptions;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private Mock<ISubscriptionService> _subscriptionServiceMock;
        private SubscriptionController _controller;

        private void SetUser(string userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [SetUp]
        public void Setup()
        {
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _controller = new SubscriptionController(_subscriptionServiceMock.Object);
            SetUser("dealer-user-1", "Dealer");
        }

       [Test]
        public async Task Subscribe_Success()
        {
            var dto = new CreateSubscriptionDto { CropId = 1 };
            var subscription = new Subscription { Id = 1, DealerId = 1, CropId = 1 };

            _subscriptionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _subscriptionServiceMock.Setup(s => s.SubscribeAsync(It.IsAny<Subscription>())).ReturnsAsync(subscription);

            var result = await _controller.Subscribe(dto);

            var ok = result as OkObjectResult;
            var returned = ok!.Value as SubscriptionDto;

            Assert.That(returned, Is.Not.Null);
            Assert.That(returned!.Id, Is.EqualTo(1));
            Assert.That(returned.CropId, Is.EqualTo(1));
        }

        [Test]
        public async Task Subscribe_NoProfile_BadRequest()
        {
            var dto = new CreateSubscriptionDto { CropId = 1 };

            _subscriptionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync((int?)null);

            var result = await _controller.Subscribe(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetMySubscriptions_Success()
        {
            var subscriptions = new List<Subscription>
            {
                new Subscription { Id = 1, DealerId = 1, CropId = 1 },
                new Subscription { Id = 2, DealerId = 1, CropId = 2 }
            };

            _subscriptionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _subscriptionServiceMock.Setup(s => s.GetSubscriptionsByDealerAsync(1)).ReturnsAsync(subscriptions);

            var result = await _controller.GetMySubscriptions();

            var ok = result as OkObjectResult;
            var returned = ok!.Value as IEnumerable<SubscriptionDto>;

            Assert.That(returned, Is.Not.Null);
            Assert.That(returned!.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Unsubscribe_WrongOwner_Forbidden()
        {
            _subscriptionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _subscriptionServiceMock
                .Setup(s => s.UnsubscribeAsync(1, 1))
                .ThrowsAsync(new ForbiddenException("You do not have permission to remove this subscription."));

            Assert.ThrowsAsync<ForbiddenException>(() => _controller.Unsubscribe(1));
        }

        [Test]
        public async Task Unsubscribe_Success()
        {
            _subscriptionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _subscriptionServiceMock.Setup(s => s.UnsubscribeAsync(1, 1)).Returns(Task.CompletedTask);

            var result = await _controller.Unsubscribe(1);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Value, Is.EqualTo("Unsubscribed successfully"));
        }
    }
}