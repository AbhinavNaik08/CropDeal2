using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.DTOs.Auth;
using CropDeal.Exceptions;

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
        public async Task Register_Success()
        {
            var dto = new RegisterDto
            {
                FullName = "Farmer One",
                Email = "farmer1@codelearnfast.com",
                Password = "Farmer@123",
                Role = "Farmer"
            };

            _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync("fake-jwt-token");

            var result = await _controller.Register(dto);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);

            var value = ok!.Value;
            var tokenProp = value!.GetType().GetProperty("token")!.GetValue(value);
            Assert.That(tokenProp, Is.EqualTo("fake-jwt-token"));
        }

        [Test]
        public void Register_InvalidData_ThrowsBadRequest()
        {
            var dto = new RegisterDto
            {
                FullName = "Bad User",
                Email = "invalid",
                Password = "123",
                Role = "Farmer"
            };

            _authServiceMock
                .Setup(s => s.RegisterAsync(dto))
                .ThrowsAsync(new BadRequestException("Passwords must be at least 6 characters."));

            Assert.ThrowsAsync<BadRequestException>(() => _controller.Register(dto));
        }

        [Test]
        public async Task Login_Success()
        {
            var dto = new LoginDto
            {
                Email = "dealer1@codelearnfast.com",
                Password = "Dealer@123"
            };

            _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync("fake-jwt-token");

            var result = await _controller.Login(dto);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);

            var value = ok!.Value;
            var tokenProp = value!.GetType().GetProperty("token")!.GetValue(value);
            Assert.That(tokenProp, Is.EqualTo("fake-jwt-token"));
        }

        [Test]
        public void Login_WrongCredentials_ThrowsUnauthorized()
        {
            var dto = new LoginDto
            {
                Email = "wrong@codelearnfast.com",
                Password = "WrongPassword"
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(dto))
                .ThrowsAsync(new UnauthorizedException("Invalid email or password"));

            Assert.ThrowsAsync<UnauthorizedException>(() => _controller.Login(dto));
        }
    }
}