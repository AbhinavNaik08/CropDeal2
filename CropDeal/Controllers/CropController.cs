using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using CropDeal.Models;
using Microsoft.AspNetCore.Authorization;
using CropDeal.DTOs.Crop;

namespace CropDeal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CropController : ControllerBase
    {
        private readonly ICropService _cropService;

        public CropController(ICropService cropService)
        {
            _cropService = cropService;
        }

        private async Task<int?> ResolveCallerFarmerIdAsync()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _cropService.GetFarmerIdByUserIdAsync(userId);
        }

        [Authorize(Roles = "Farmer")]
        [HttpPost]
        public async Task<IActionResult> PublishCrop(CreateCropDto dto)
        {
            var farmerId = await ResolveCallerFarmerIdAsync();

            if (farmerId == null)
                return BadRequest("No farmer profile found for this account.");

            var crop = new Crop
            {
                FarmerId = farmerId.Value,
                CropName = dto.CropName,
                CropType = dto.CropType,
                Quantity = dto.Quantity,
                ExpectedPrice = dto.ExpectedPrice,
                Location = dto.Location
            };

            var createdCrop = await _cropService.PublishCropAsync(crop);

            return Ok(createdCrop);
        }

        [Authorize(Roles = "Farmer,Dealer,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllCrops()
        {
            var crops = await _cropService.GetAllCropsAsync();

            var result = crops.Select(c => new CropDto
            {
                Id = c.Id,
                FarmerId = c.FarmerId,
                CropName = c.CropName,
                CropType = c.CropType,
                Quantity = c.Quantity,
                ExpectedPrice = c.ExpectedPrice,
                Location = c.Location
            });

            return Ok(result);
        }

        [Authorize(Roles = "Farmer,Dealer,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCropById(int id)
        {
            var crop = await _cropService.GetCropByIdAsync(id);

            if (crop == null)
                return NotFound();

            return Ok(crop);
        }

        [Authorize(Roles = "Farmer,Dealer,Admin")]
        [HttpGet("farmer/{farmerId}")]
        public async Task<IActionResult> GetCropsByFarmer(int farmerId)
        {
            var crops = await _cropService.GetCropsByFarmerAsync(farmerId);

            return Ok(crops);
        }

        [Authorize(Roles = "Farmer,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCrop(int id, UpdateCropDto dto)
        {
            var existingCrop = await _cropService.GetCropByIdAsync(id);

            if (existingCrop == null)
                return NotFound("Crop not found.");

            if (!User.IsInRole("Admin"))
            {
                var farmerId = await ResolveCallerFarmerIdAsync();

                if (farmerId == null || farmerId != existingCrop.FarmerId)
                    return Forbid();
            }

            var crop = new Crop
            {
                Id = id,
                CropName = dto.CropName,
                CropType = dto.CropType,
                Quantity = dto.Quantity,
                ExpectedPrice = dto.ExpectedPrice,
                Location = dto.Location
            };

            var updatedCrop = await _cropService.UpdateCropAsync(crop);

            if (updatedCrop == null)
            {
                return NotFound("Crop not found.");
            }

            return Ok(updatedCrop);
        }
    }
}