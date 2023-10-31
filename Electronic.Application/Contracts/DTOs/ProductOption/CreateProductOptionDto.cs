using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.ProductOption;

public class CreateProductOptionDto
{
    [Required]
    public string Name { get; set; }
}