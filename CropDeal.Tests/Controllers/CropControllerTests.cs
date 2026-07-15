using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CropDeal.Controllers;
using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.DTOs.Crop;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class CropControllerTests
    {
        private Mock<ICropService> _cropServiceMock;
        private CropController _controller;

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
            _cropServiceMock = new Mock<ICropService>();
            _controller = new CropController(_cropServiceMock.Object);
            SetUser("farmer-user-1", "Farmer");
        }

        [Test]
        public async Task PublishCrop_Success()
        {
            var dto = new CreateCropDto
            {
                CropName = "Maize",
                CropType = "Kharif",
                Quantity = 30,
                ExpectedPrice = 15m,
                Location = "Nashik"
            };

            var createdCrop = new Crop { Id = 5, FarmerId = 1, CropName = "Maize" };

            _cropServiceMock.Setup(s => s.GetFarmerIdByUserIdAsync("farmer-user-1")).ReturnsAsync(1);
            _cropServiceMock
                .Setup(s => s.PublishCropAsync(It.Is<Crop>(c => c.FarmerId == 1)))
                .ReturnsAsync(createdCrop);

            var result = await _controller.PublishCrop(dto);

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            Assert.That(ok!.Value, Is.EqualTo(createdCrop));
        }

        [Test]
        public async Task PublishCrop_NoProfile_BadRequest()
        {
            var dto = new CreateCropDto { CropName = "Rice" };

            _cropServiceMock.Setup(s => s.GetFarmerIdByUserIdAsync("farmer-user-1")).ReturnsAsync((int?)null);

            var result = await _controller.PublishCrop(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAllCrops_Success()
        {
            var crops = new List<Crop>
            {
                new Crop { Id = 1, FarmerId = 10, CropName = "Wheat", CropType = "Rabi", Quantity = 50, ExpectedPrice = 20m, Location = "Pune" }
            };

            _cropServiceMock.Setup(s => s.GetAllCropsAsync()).ReturnsAsync(crops);

            var result = await _controller.GetAllCrops();

            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);

            var returnedCrops = ok!.Value as IEnumerable<CropDto>;
            Assert.That(returnedCrops, Is.Not.Null);
            Assert.That(returnedCrops!.First().CropName, Is.EqualTo("Wheat"));
        }

        [Test]
        public async Task GetCropById_NotFound()
        {
            _cropServiceMock.Setup(s => s.GetCropByIdAsync(99)).ReturnsAsync((Crop?)null);

            var result = await _controller.GetCropById(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateCrop_WrongOwner_Forbidden()
        {
            var dto = new UpdateCropDto { CropName = "Bajra" };
            var existingCrop = new Crop { Id = 1, FarmerId = 99, CropName = "Bajra" };

            _cropServiceMock.Setup(s => s.GetCropByIdAsync(1)).ReturnsAsync(existingCrop);
            _cropServiceMock.Setup(s => s.GetFarmerIdByUserIdAsync("farmer-user-1")).ReturnsAsync(1);

            var result = await _controller.UpdateCrop(1, dto);

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task UpdateCrop_NotFound()
        {
            var dto = new UpdateCropDto { CropName = "Bajra" };

            _cropServiceMock.Setup(s => s.GetCropByIdAsync(1)).ReturnsAsync((Crop?)null);

            var result = await _controller.UpdateCrop(1, dto);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}