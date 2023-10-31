using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.ProductOptionGroup;

public class CreateProductOptionGroupDto
{
    [Required]
    public int? ProductOptionId { get; set; }
    [Required]
    public string Value { get; set; }
}