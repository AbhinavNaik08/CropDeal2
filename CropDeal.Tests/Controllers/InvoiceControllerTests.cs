using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.Exceptions;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class InvoiceControllerTests
    {
        private Mock<IInvoiceService> _invoiceServiceMock;
        private InvoiceController _controller;

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
            _invoiceServiceMock = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_invoiceServiceMock.Object);
        }

        [Test]
        public async Task CreateInvoice_Success()
        {
            SetUser("dealer-user-1", "Dealer");

            var invoice = new Invoice { Id = 1, TransactionId = 100, Date = DateTime.UtcNow };

            _invoiceServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _invoiceServiceMock.Setup(s => s.CreateInvoiceAsync(100, 1)).ReturnsAsync(invoice);

            var result = await _controller.CreateInvoice(100);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Value, Is.EqualTo(invoice));
        }

        [Test]
        public async Task CreateInvoice_AsAdmin_Success()
        {
            SetUser("admin-user-1", "Admin");

            var invoice = new Invoice { Id = 1, TransactionId = 100, Date = DateTime.UtcNow };

            _invoiceServiceMock.Setup(s => s.CreateInvoiceAsync(100, null)).ReturnsAsync(invoice);

            var result = await _controller.CreateInvoice(100);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _invoiceServiceMock.Verify(s => s.GetDealerIdByUserIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CreateInvoice_WrongOwner_Forbidden()
        {
            SetUser("dealer-user-1", "Dealer");

            _invoiceServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _invoiceServiceMock
                .Setup(s => s.CreateInvoiceAsync(100, 1))
                .ThrowsAsync(new ForbiddenException("You do not have permission to invoice this transaction."));

            Assert.ThrowsAsync<ForbiddenException>(() => _controller.CreateInvoice(100));
        }

        [Test]
        public async Task GetInvoiceById_NotFound()
        {
            SetUser("dealer-user-1", "Dealer");

            _invoiceServiceMock.Setup(s => s.GetInvoiceByIdAsync(99)).ReturnsAsync((Invoice?)null);

            var result = await _controller.GetInvoiceById(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetMyInvoices_Success()
        {
            SetUser("dealer-user-1", "Dealer");

            var invoices = new List<Invoice>
            {
                new Invoice { Id = 1, TransactionId = 100 },
                new Invoice { Id = 2, TransactionId = 101 }
            };

            _invoiceServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _invoiceServiceMock.Setup(s => s.GetInvoicesByDealerAsync(1)).ReturnsAsync(invoices);

            var result = await _controller.GetMyInvoices();

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Value, Is.EqualTo(invoices));
        }

        [Test]
        public async Task GetInvoicesByDealer_Success()
        {
            SetUser("admin-user-1", "Admin");

            var invoices = new List<Invoice> { new Invoice { Id = 1, TransactionId = 100 } };

            _invoiceServiceMock.Setup(s => s.GetInvoicesByDealerAsync(5)).ReturnsAsync(invoices);

            var result = await _controller.GetInvoicesByDealer(5);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Value, Is.EqualTo(invoices));
        }
    }
}