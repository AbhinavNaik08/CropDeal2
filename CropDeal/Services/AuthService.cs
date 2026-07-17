using Microsoft.AspNetCore.Identity;
using CropDeal.Models;
using CropDeal.Interfaces;
using CropDeal.DTOs.Auth;
using CropDeal.Helpers;
using CropDeal.Data;
using CropDeal.Exceptions;


namespace CropDeal.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly ApplicationDbContext _context;

        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            JwtTokenGenerator jwtTokenGenerator,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _context = context;
            _emailService = emailService;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            if(registerDto.Role != "Farmer" && registerDto.Role != "Dealer")
            {
                throw new BadRequestException("Invalid role. Role must be either 'Farmer' or 'Dealer'.");
            }
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException(errors);
            }
            if (!await _roleManager.RoleExistsAsync(registerDto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(registerDto.Role));
            }

            await _userManager.AddToRoleAsync(user, registerDto.Role);

            if (registerDto.Role == "Farmer")
            {
                var farmer = new Farmer
                {
                    UserId = user.Id
                };

                _context.Farmers.Add(farmer);
            }
            else if (registerDto.Role == "Dealer")
            {
                var dealer = new Dealer
                {
                    UserId = user.Id
                };

                _context.Dealers.Add(dealer);
            }

            await _context.SaveChangesAsync();

            var subject = "Welcome to CropDeal!";

            var body =
                $"Hello {user.FullName},\n\n" +
                $"Welcome to CropDeal!\n\n" +
                $"Your {registerDto.Role} account has been created successfully.\n\n" +
                $"You can now log in and start using the platform.\n\n" +
                $"Thank you for choosing CropDeal!\n\n" +
                $"Regards,\n" +
                $"CropDeal Team";

            try
            {
                await _emailService.SendEmailAsync(
                    user.Email!,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send welcome email to {user.Email}: {ex.Message}");
            }

            return _jwtTokenGenerator.GenerateToken(user, registerDto.Role);
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                throw new UnauthorizedException("Invalid email or password");

            if(await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException("User account is locked. Please contact support.");

            var validPassword =
                await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!validPassword)
                throw new UnauthorizedException("Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);

            var role = roles.FirstOrDefault() ?? "User";

            return _jwtTokenGenerator.GenerateToken(user, role);
        }
    }
}