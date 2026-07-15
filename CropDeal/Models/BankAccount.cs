namespace CropDeal.Models
{
    public class BankAccount
    {
        public int Id { get; set; }

        public int FarmerId { get; set; }

        public Farmer? Farmer { get; set; }

        public string AccountNumber { get; set; }=string.Empty;
    }
}