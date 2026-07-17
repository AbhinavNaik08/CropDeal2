using System.Security.Claims;
using CropDeal.Controllers;
using CropDeal.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private PaymentController _controller;

        [SetUp]
        public void Setup()
        {
            _paymentServiceMock = new Mock<IPaymentService>();
            _controller = new PaymentController(_paymentServiceMock.Object);

            // Give the controller an empty HttpContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Test]
        public async Task ProcessPayment_ReturnsUnauthorized()
        {
            var result = await _controller.ProcessPayment(1);

            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task ProcessPayment_ReturnsResult()
        {
            var result = await _controller.ProcessPayment(1);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task ProcessPayment_DoesNotCallService_WhenUnauthorized()
        {
            await _controller.ProcessPayment(1);

            _paymentServiceMock.Verify(
                x => x.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<int?>()),
                Times.Never);
        }
    }
}