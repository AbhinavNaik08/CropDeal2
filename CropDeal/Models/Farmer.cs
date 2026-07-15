namespace CropDeal.Models
{
    public class Farmer
    {
        public int Id { get; set; }

        public string  UserId { get; set; }=string.Empty;

        public ApplicationUser? User { get; set; }

        public ICollection<Crop>? Crops { get; set; }

        public ICollection<BankAccount>? BankAccounts { get; set; }
    }
}