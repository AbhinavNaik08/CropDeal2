namespace CropDeal.Models
{
    public class Dealer
    {
        public int Id { get; set; }

        public string UserId { get; set; } =  string.Empty;

        public ApplicationUser? User { get; set; }

        public ICollection<Subscription>? Subscriptions { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }   
}