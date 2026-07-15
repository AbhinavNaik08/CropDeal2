using CropDeal.DTOs.Auth;

namespace CropDeal.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);

        Task<string> LoginAsync(LoginDto loginDto);
    }
}   