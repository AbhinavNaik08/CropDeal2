using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using CropDeal.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
namespace CropDeal.Controllers
{
    [AllowAnonymous ]
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuthController : ControllerBase
    {
        
        private readonly IAuthService _authService;

    //dependecy injection of auth service
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var token = await _authService.RegisterAsync(registerDto);

            return Ok(new
            {
                message = "User registered successfully",
                token
            });
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);

            return Ok(new
            {
                message = "Login successful",
                token
            });
        }

       
    }
}