namespace CropDeal.Models
{
    public class Crop
    {
        public int Id { get; set; }

        public int FarmerId { get; set; }

        public Farmer? Farmer { get; set; }

        public string CropName { get; set; }=string.Empty;

        public string CropType { get; set; }=string.Empty;

        public int Quantity { get; set; }

        public decimal ExpectedPrice { get; set; }

        public string Location { get; set; }= string.Empty;

        public ICollection<Subscription>?  Subscriptions { get; set; }
    }
}