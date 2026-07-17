using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class SubscriptionControllerTests
    {
        private Mock<ISubscriptionService> _subscriptionServiceMock;
        private SubscriptionController _controller;

        [SetUp]
        public void Setup()
        {
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _controller = new SubscriptionController(_subscriptionServiceMock.Object);
        }

        [Test]
        public async Task GetSubscriptionsByDealer_ReturnsOk()
        {
            _subscriptionServiceMock
                .Setup(x => x.GetSubscriptionsByDealerAsync(1))
                .ReturnsAsync(new List<Subscription>());

            var result = await _controller.GetSubscriptionsByDealer(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSubscriptionsByDealer_CallsService()
        {
            _subscriptionServiceMock
                .Setup(x => x.GetSubscriptionsByDealerAsync(1))
                .ReturnsAsync(new List<Subscription>());

            await _controller.GetSubscriptionsByDealer(1);

            _subscriptionServiceMock.Verify(
                x => x.GetSubscriptionsByDealerAsync(1),
                Times.Once);
        }

        [Test]
        public async Task GetSubscriptionsByDealer_ReturnsResult()
        {
            _subscriptionServiceMock
                .Setup(x => x.GetSubscriptionsByDealerAsync(1))
                .ReturnsAsync(new List<Subscription>());

            var result = await _controller.GetSubscriptionsByDealer(1);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetSubscriptionsByDealer_ReturnsOkObject()
        {
            _subscriptionServiceMock
                .Setup(x => x.GetSubscriptionsByDealerAsync(1))
                .ReturnsAsync(new List<Subscription>());

            var result = await _controller.GetSubscriptionsByDealer(1);

            var ok = result as OkObjectResult;

            Assert.That(ok, Is.Not.Null);
        }

        [Test]
        public async Task GetSubscriptionsByDealer_ReturnsStatusCode200()
        {
            _subscriptionServiceMock
                .Setup(x => x.GetSubscriptionsByDealerAsync(1))
                .ReturnsAsync(new List<Subscription>());

            var result = await _controller.GetSubscriptionsByDealer(1);

            var ok = result as OkObjectResult;

            Assert.That(ok!.StatusCode ?? 200, Is.EqualTo(200));
        }
    }
}