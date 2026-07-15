using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Exceptions;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _paymentServiceMock;
        private PaymentController _controller;

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
            _paymentServiceMock = new Mock<IPaymentService>();
            _controller = new PaymentController(_paymentServiceMock.Object);
        }

        [Test]
        public async Task ProcessPayment_Success()
        {
            SetUser("dealer-user-1", "Dealer");

            _paymentServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _paymentServiceMock
                .Setup(s => s.ProcessPaymentAsync(100, 1))
                .ReturnsAsync("Payment Successful");

            var result = await _controller.ProcessPayment(100);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);

            var value = ok!.Value;
            var messageProp = value!.GetType().GetProperty("message")!.GetValue(value);
            Assert.That(messageProp, Is.EqualTo("Payment Successful"));
        }

        [Test]
        public async Task ProcessPayment_AsAdmin_Success()
        {
            SetUser("admin-user-1", "Admin");

            _paymentServiceMock
                .Setup(s => s.ProcessPaymentAsync(100, null))
                .ReturnsAsync("Payment Successful");

            var result = await _controller.ProcessPayment(100);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _paymentServiceMock.Verify(s => s.GetDealerIdByUserIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ProcessPayment_NoProfile_BadRequest()
        {
            SetUser("dealer-user-1", "Dealer");

            _paymentServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync((int?)null);

            var result = await _controller.ProcessPayment(100);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void ProcessPayment_WrongOwner_Forbidden()
        {
            SetUser("dealer-user-1", "Dealer");

            _paymentServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _paymentServiceMock
                .Setup(s => s.ProcessPaymentAsync(100, 1))
                .ThrowsAsync(new ForbiddenException("You do not have permission to pay for this transaction."));

            Assert.ThrowsAsync<ForbiddenException>(() => _controller.ProcessPayment(100));
        }

        [Test]
        public void ProcessPayment_NotFound()
        {
            SetUser("dealer-user-1", "Dealer");

            _paymentServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _paymentServiceMock
                .Setup(s => s.ProcessPaymentAsync(999, 1))
                .ThrowsAsync(new NotFoundException("Transaction not found."));

            Assert.ThrowsAsync<NotFoundException>(() => _controller.ProcessPayment(999));
        }
    }
}