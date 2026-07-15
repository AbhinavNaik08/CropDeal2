namespace CropDeal.DTOs.Crop
{
    public class CropDto
    {
        public int Id { get; set; }

        public string CropName { get; set; } = string.Empty;

        public string CropType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal ExpectedPrice { get; set; }

        public string Location { get; set; } = string.Empty;

        public int FarmerId { get; set; }
    }
}