namespace CropDeal.DTOs.Crop
{
    public class CreateCropDto
    {
 
        public string CropName { get; set; }

        public string CropType { get; set; }

        public int Quantity { get; set; }

        public decimal ExpectedPrice { get; set; }

        public string Location { get; set; }
    }
}