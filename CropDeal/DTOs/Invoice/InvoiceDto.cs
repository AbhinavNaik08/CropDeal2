namespace CropDeal.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }
        public int? DealerId { get; set; }
        public decimal? Amount { get; set; }
    }
}