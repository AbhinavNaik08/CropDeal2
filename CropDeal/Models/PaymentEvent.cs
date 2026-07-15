namespace CropDeal.Models
{
    public class PaymentEvent
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }

        public Transaction? Transaction { get; set; }

        public string EventType { get; set; }=string.Empty;

        public DateTime Timestamp { get; set; }
    }
}