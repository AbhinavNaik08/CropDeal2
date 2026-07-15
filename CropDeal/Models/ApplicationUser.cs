using Microsoft.AspNetCore.Identity;

namespace CropDeal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public Farmer? Farmer { get; set; }

        public Dealer? Dealer { get; set; }
    }
}   