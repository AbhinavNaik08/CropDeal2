namespace CropDeal.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }

        public Transaction? Transaction { get; set; }

        public DateTime Date { get; set; }
    }
}