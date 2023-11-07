using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.Product;

public class UpdateProductPriceRequestDto
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}")]
    public decimal Price { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}")]
    public decimal SpecialPrice { get; set; }
    public DateTime SpecialPriceStartDate { get; set; }
    public DateTime SpecialPriceEndDate { get; set; }
}