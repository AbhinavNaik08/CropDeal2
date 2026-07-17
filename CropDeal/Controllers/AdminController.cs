using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CropDeal.Models;
using CropDeal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CropDeal.DTOs.Admin;

namespace CropDeal.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? string.Empty,
                    IsLockedOut = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
                })
                .ToList();

            return Ok(users);
        }

        [HttpPost("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            return Ok("User deactivated");
        }

        [HttpPost("activate/{userId}")]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            return Ok("User activated");
        }

        [HttpGet("report")]
        public async Task<IActionResult> GenerateReport()
        {
            var farmers = await _context.Farmers.CountAsync();
            var dealers = await _context.Dealers.CountAsync();
            var crops = await _context.Crops.CountAsync();
            var transactions = await _context.Transactions.CountAsync();

            return Ok(new
            {
                TotalFarmers = farmers,
                TotalDealers = dealers,
                TotalCrops = crops,
                TotalTransactions = transactions
            });
        }
    }
}