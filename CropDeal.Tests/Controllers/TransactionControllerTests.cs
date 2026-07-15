using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.DTOs.Transaction;
using CropDeal.Exceptions;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class TransactionControllerTests
    {
        private Mock<ITransactionService> _transactionServiceMock;
        private TransactionController _controller;

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
            _transactionServiceMock = new Mock<ITransactionService>();
            _controller = new TransactionController(_transactionServiceMock.Object);
            SetUser("dealer-user-1", "Dealer");
        }

        [Test]
        public async Task CreateTransaction_Success()
        {
            var dto = new CreateTransactionDto { CropId = 1, Quantity = 10 };

            var createdTransaction = new Transaction
            {
                Id = 100,
                CropId = 1,
                DealerId = 1,
                Quantity = 10,
                Amount = 200m
            };

            _transactionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _transactionServiceMock
                .Setup(s => s.CreateTransactionAsync(It.Is<Transaction>(t => t.DealerId == 1 && t.CropId == 1)))
                .ReturnsAsync(createdTransaction);

            var result = await _controller.CreateTransaction(dto);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Value, Is.EqualTo(createdTransaction));
        }

        [Test]
        public void CreateTransaction_CropNotFound()
        {
            var dto = new CreateTransactionDto { CropId = 999, Quantity = 10 };

            _transactionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _transactionServiceMock
                .Setup(s => s.CreateTransactionAsync(It.IsAny<Transaction>()))
                .ThrowsAsync(new NotFoundException("Crop not found"));

            Assert.ThrowsAsync<NotFoundException>(() => _controller.CreateTransaction(dto));
        }

        [Test]
        public void CreateTransaction_QuantityTooHigh_BadRequest()
        {
            var dto = new CreateTransactionDto { CropId = 1, Quantity = 500 };

            _transactionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _transactionServiceMock
                .Setup(s => s.CreateTransactionAsync(It.IsAny<Transaction>()))
                .ThrowsAsync(new BadRequestException("Requested quantity exceeds available crop quantity"));

            Assert.ThrowsAsync<BadRequestException>(() => _controller.CreateTransaction(dto));
        }

        [Test]
        public async Task GetMyTransactions_Success()
        {
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, DealerId = 1, Amount = 100m },
                new Transaction { Id = 2, DealerId = 1, Amount = 200m }
            };

            _transactionServiceMock.Setup(s => s.GetDealerIdByUserIdAsync("dealer-user-1")).ReturnsAsync(1);
            _transactionServiceMock.Setup(s => s.GetTransactionsByDealerAsync(1)).ReturnsAsync(transactions);

            var result = await _controller.GetMyTransactions();

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Value, Is.EqualTo(transactions));
        }
    }
}