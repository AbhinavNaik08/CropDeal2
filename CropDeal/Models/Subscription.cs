namespace CropDeal.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        public int DealerId { get; set; }

        public Dealer? Dealer { get; set; }

        public int CropId { get; set; }

        public Crop? Crop { get; set; }
    }
}