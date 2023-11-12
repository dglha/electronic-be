using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.Brand;

public class UpdateBrandDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public bool? IsPublished { get; set; }
}