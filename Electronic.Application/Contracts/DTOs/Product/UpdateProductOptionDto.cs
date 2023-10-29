using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.Product;

public class UpdateProductOptionDto
{
    [Required]
    public int? OptionId { get; set; }
    
    [Required]
    public string Value { get; set; }
}