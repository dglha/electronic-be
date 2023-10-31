using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.ProductOption;

public class AddProductOptionValueDto
{
    [Required]
    public int OptionId { get; set; }
    
    [Required]
    public List<string?> Values { get; set; }
}