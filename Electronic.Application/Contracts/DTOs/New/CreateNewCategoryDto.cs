using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.New;

public class CreateNewCategoryDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Slug { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public int? DisplayOrder { get; set; }
    [Required]
    public bool? IsPublished { get; set; }
}