using CropDeal.Controllers;
using CropDeal.DTOs.Auth;
using CropDeal.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Test]
        public async Task Register_ReturnsOk()
        {
            var dto = new RegisterDto
            {
                FullName = "John",
                Email = "john@test.com",
                Password = "Password@123",
                Role = "Farmer"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync("token");

            var result = await _controller.Register(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Login_ReturnsOk()
        {
            var dto = new LoginDto
            {
                Email = "john@test.com",
                Password = "Password@123"
            };

            _authServiceMock.Setup(x => x.LoginAsync(dto))
                .ReturnsAsync("token");

            var result = await _controller.Login(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Register_CallsService()
        {
            var dto = new RegisterDto
            {
                FullName = "John",
                Email = "john@test.com",
                Password = "Password@123",
                Role = "Farmer"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync("token");

            await _controller.Register(dto);

            _authServiceMock.Verify(x => x.RegisterAsync(dto), Times.Once);
        }

        [Test]
        public async Task Login_CallsService()
        {
            var dto = new LoginDto
            {
                Email = "john@test.com",
                Password = "Password@123"
            };

            _authServiceMock.Setup(x => x.LoginAsync(dto))
                .ReturnsAsync("token");

            await _controller.Login(dto);

            _authServiceMock.Verify(x => x.LoginAsync(dto), Times.Once);
        }

        [Test]
        public async Task Register_ReturnsResult()
        {
            var dto = new RegisterDto
            {
                FullName = "John",
                Email = "john@test.com",
                Password = "Password@123",
                Role = "Farmer"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync("token");

            var result = await _controller.Register(dto);

            Assert.That(result, Is.Not.Null);
        }
    }
}