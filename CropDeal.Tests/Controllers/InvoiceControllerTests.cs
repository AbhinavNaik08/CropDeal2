using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class InvoiceControllerTests
    {
        private Mock<IInvoiceService> _invoiceServiceMock;
        private InvoiceController _controller;

        [SetUp]
        public void Setup()
        {
            _invoiceServiceMock = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_invoiceServiceMock.Object);
        }

        [Test]
        public async Task GetInvoiceById_ReturnsOk()
        {
            _invoiceServiceMock.Setup(x => x.GetInvoiceByIdAsync(1))
                .ReturnsAsync(new Invoice());

            var result = await _controller.GetInvoiceById(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetInvoiceById_ReturnsNotFound()
        {
            _invoiceServiceMock.Setup(x => x.GetInvoiceByIdAsync(1))
                .ReturnsAsync((Invoice?)null);

            var result = await _controller.GetInvoiceById(1);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetInvoiceById_CallsService()
        {
            _invoiceServiceMock.Setup(x => x.GetInvoiceByIdAsync(1))
                .ReturnsAsync(new Invoice());

            await _controller.GetInvoiceById(1);

            _invoiceServiceMock.Verify(x => x.GetInvoiceByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetInvoicesByDealer_ReturnsOk()
        {
            _invoiceServiceMock.Setup(x => x.GetInvoicesByDealerAsync(1))
                .ReturnsAsync(new List<Invoice>());

            var result = await _controller.GetInvoicesByDealer(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetInvoicesByDealer_CallsService()
        {
            _invoiceServiceMock.Setup(x => x.GetInvoicesByDealerAsync(1))
                .ReturnsAsync(new List<Invoice>());

            await _controller.GetInvoicesByDealer(1);

            _invoiceServiceMock.Verify(x => x.GetInvoicesByDealerAsync(1), Times.Once);
        }
    }
}