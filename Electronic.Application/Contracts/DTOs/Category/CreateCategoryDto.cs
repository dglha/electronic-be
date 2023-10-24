using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.Category;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "The {0} field is required.")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "The {0} field is required.")]
    [Range(1, 100)]
    public int? DisplayOrder { get; set; }
    
    public bool IsPublished { get; set; }
    
    [Required(ErrorMessage = "The {0} field is required.")]
    public bool? IncludeInMenu { get; set; }
    
    public long? ParentCategoryId { get; set; }
}