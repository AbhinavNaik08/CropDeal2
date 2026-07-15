namespace CropDeal.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int CropId { get; set; }

        public Crop? Crop { get; set; }

        public int DealerId { get; set; }

        public Dealer? Dealer { get; set; }

        public decimal Amount { get; set; }

        public ICollection<PaymentEvent>? PaymentEvents { get; set; }

        public Invoice? Invoice { get; set; }

        public int Quantity { get; set; }
    }
}