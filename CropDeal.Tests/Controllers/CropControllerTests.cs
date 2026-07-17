using CropDeal.Controllers;
using CropDeal.DTOs.Crop;
using CropDeal.Interfaces;
using CropDeal.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CropDeal.Tests.Controllers
{
    [TestFixture]
    public class CropControllerTests
    {
        private Mock<ICropService> _cropServiceMock;
        private CropController _controller;

        [SetUp]
        public void Setup()
        {
            _cropServiceMock = new Mock<ICropService>();
            _controller = new CropController(_cropServiceMock.Object);
        }

        [Test]
        public async Task GetAllCrops_ReturnsOk()
        {
            _cropServiceMock.Setup(x => x.GetAllCropsAsync())
                .ReturnsAsync(new List<Crop>());

            var result = await _controller.GetAllCrops();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetCropById_ReturnsOk()
        {
            _cropServiceMock.Setup(x => x.GetCropByIdAsync(1))
                .ReturnsAsync(new Crop
                {
                    Id = 1,
                    CropName = "Rice",
                    CropType = "Grain",
                    Quantity = 100,
                    ExpectedPrice = 5000,
                    Location = "Bangalore",
                    FarmerId = 1
                });

            var result = await _controller.GetCropById(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetCropById_ReturnsNotFound()
        {
            _cropServiceMock.Setup(x => x.GetCropByIdAsync(1))
                .ReturnsAsync((Crop?)null);

            var result = await _controller.GetCropById(1);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetCropsByFarmer_ReturnsOk()
        {
            _cropServiceMock.Setup(x => x.GetCropsByFarmerAsync(1))
                .ReturnsAsync(new List<Crop>());

            var result = await _controller.GetCropsByFarmer(1);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task UpdateCrop_ReturnsNotFound()
        {
            _cropServiceMock.Setup(x => x.GetCropByIdAsync(1))
                .ReturnsAsync((Crop?)null);

            var dto = new UpdateCropDto();

            var result = await _controller.UpdateCrop(1, dto);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetAllCrops_CallsService()
        {
            _cropServiceMock.Setup(x => x.GetAllCropsAsync())
                .ReturnsAsync(new List<Crop>());

            await _controller.GetAllCrops();

            _cropServiceMock.Verify(x => x.GetAllCropsAsync(), Times.Once);
        }

        [Test]
        public async Task GetCropById_CallsService()
        {
            _cropServiceMock.Setup(x => x.GetCropByIdAsync(1))
                .ReturnsAsync(new Crop());

            await _controller.GetCropById(1);

            _cropServiceMock.Verify(x => x.GetCropByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetCropsByFarmer_CallsService()
        {
            _cropServiceMock.Setup(x => x.GetCropsByFarmerAsync(1))
                .ReturnsAsync(new List<Crop>());

            await _controller.GetCropsByFarmer(1);

            _cropServiceMock.Verify(x => x.GetCropsByFarmerAsync(1), Times.Once);
        }
    }
}