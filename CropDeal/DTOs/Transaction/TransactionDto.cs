namespace CropDeal.DTOs.Transaction
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int CropId { get; set; }
        public string? CropName {get;set;}
        public int DealerId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }

}