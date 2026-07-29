using System.ComponentModel.DataAnnotations;

namespace CropDeal.DTOs.Crop
{
    public class CreateCropDto
    {
        [Required(ErrorMessage = "Crop name is required.")]
        [StringLength(20, ErrorMessage = "Crop name cannot exceed 20 characters.")]
        public string CropName { get; set; }
        
        [Required(ErrorMessage = "Crop type is required.")]
        [StringLength(20, ErrorMessage = "Crop type cannot exceed 20 characters.")]
        public string CropType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        public int Quantity { get; set; }


        [Range(0.01, double.MaxValue, ErrorMessage = "Expected price must be a positive value.")]
        public decimal ExpectedPrice { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(30, ErrorMessage = "Location cannot exceed 30 characters.")]
        public string Location { get; set; }
    }
}