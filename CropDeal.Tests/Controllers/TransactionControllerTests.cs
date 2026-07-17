using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class TransactionControllerTests
    {
        private Mock<ITransactionService> _transactionServiceMock;
        private TransactionController _controller;

        [SetUp]
        public void Setup()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _controller = new TransactionController(_transactionServiceMock.Object);
        }

        [Test]
        public async Task GetTransactionsByDealer_ReturnsOk()
        {
            _transactionServiceMock
                .Setup(x => x.GetTransactionsByDealerAsync(1))
                .ReturnsAsync(new List<Transaction>());

            var result = await _controller.GetTransactionsByDealer(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetTransactionsByDealer_CallsService()
        {
            _transactionServiceMock
                .Setup(x => x.GetTransactionsByDealerAsync(1))
                .ReturnsAsync(new List<Transaction>());

            await _controller.GetTransactionsByDealer(1);

            _transactionServiceMock.Verify(
                x => x.GetTransactionsByDealerAsync(1),
                Times.Once);
        }

        [Test]
        public async Task GetTransactionsByDealer_ReturnsResult()
        {
            _transactionServiceMock
                .Setup(x => x.GetTransactionsByDealerAsync(1))
                .ReturnsAsync(new List<Transaction>());

            var result = await _controller.GetTransactionsByDealer(1);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetTransactionsByDealer_ReturnsOkObject()
        {
            _transactionServiceMock
                .Setup(x => x.GetTransactionsByDealerAsync(1))
                .ReturnsAsync(new List<Transaction>());

            var result = await _controller.GetTransactionsByDealer(1);

            var ok = result as OkObjectResult;

            Assert.That(ok, Is.Not.Null);
        }
    }
}